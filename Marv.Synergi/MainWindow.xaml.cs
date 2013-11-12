using Marv.Common;
using Marv.Synergi.LineAndSectionOverviewService;
using Marv.Synergi.LoginService;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Linq;

namespace Marv.Synergi
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty DataTableProperty =
        DependencyProperty.Register("DataTable", typeof(DataTable), typeof(MainWindow), new PropertyMetadata(new DataTable()));

        public static readonly DependencyProperty DataViewProperty =
        DependencyProperty.Register("DataView", typeof(DataView), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty LinesProperty =
        DependencyProperty.Register("Lines", typeof(IEnumerable<LineSummaryDTO>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public static readonly DependencyProperty PropertiesProperty =
        DependencyProperty.Register("Properties", typeof(Dynamic), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SectionsProperty =
        DependencyProperty.Register("Sections", typeof(IEnumerable<SectionSummaryDTO>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedLineProperty =
        DependencyProperty.Register("SelectedLine", typeof(LineSummaryDTO), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedSectionProperty =
        DependencyProperty.Register("SelectedSection", typeof(SectionSummaryDTO), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty UserNameProperty =
        DependencyProperty.Register("UserName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public ViewModelCollection<ViewModel> Segments
        {
            get { return (ViewModelCollection<ViewModel>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        public static readonly DependencyProperty SegmentsProperty =
        DependencyProperty.Register("Segments", typeof(ViewModelCollection<ViewModel>), typeof(MainWindow), new PropertyMetadata(new ViewModelCollection<ViewModel>()));

        public Dict<string, object> SegmentData
        {
            get { return (Dict<string, object>)GetValue(SegmentDataProperty); }
            set { SetValue(SegmentDataProperty, value); }
        }

        public static readonly DependencyProperty SegmentDataProperty =
        DependencyProperty.Register("SegmentData", typeof(Dict<string, object>), typeof(MainWindow), new PropertyMetadata(new Dict<string, object>()));

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string ticket;

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();

            InitializeComponent();

            this.LinesListBox.SelectionChanged += LinesListBox_SelectionChanged;
            this.LoginButton.Click += LoginButton_Click;
            this.SectionsListBox.SelectionChanged += SectionsListBox_SelectionChanged;
        }

        public DataTable DataTable
        {
            get { return (DataTable)GetValue(DataTableProperty); }
            set { SetValue(DataTableProperty, value); }
        }

        public DataView DataView
        {
            get { return (DataView)GetValue(DataViewProperty); }
            set { SetValue(DataViewProperty, value); }
        }

        public IEnumerable<LineSummaryDTO> Lines
        {
            get { return (IEnumerable<LineSummaryDTO>)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public Dynamic Properties
        {
            get { return (Dynamic)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }

        public IEnumerable<SectionSummaryDTO> Sections
        {
            get { return (IEnumerable<SectionSummaryDTO>)GetValue(SectionsProperty); }
            set { SetValue(SectionsProperty, value); }
        }

        public LineSummaryDTO SelectedLine
        {
            get { return (LineSummaryDTO)GetValue(SelectedLineProperty); }
            set { SetValue(SelectedLineProperty, value); }
        }

        public SectionSummaryDTO SelectedSection
        {
            get { return (SectionSummaryDTO)GetValue(SelectedSectionProperty); }
            set { SetValue(SelectedSectionProperty, value); }
        }

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        private void LinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedLine != null)
            {
                LineAndSectionOverviewService.LineAndSectionOverviewService lineAndSectionOverviewService = new LineAndSectionOverviewService.LineAndSectionOverviewService();
                lineAndSectionOverviewService.BRIXAuthenticationHeaderValue = new LineAndSectionOverviewService.BRIXAuthenticationHeader { value = this.ticket };

                this.Sections = lineAndSectionOverviewService.GetSections(this.SelectedLine.LineOid);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginService.BRIXLoginService loginService = new BRIXLoginService();
            ticket = loginService.LogIn(this.UserName, this.Password);

            LineAndSectionOverviewService.LineAndSectionOverviewService lineAndSectionOverviewService = new LineAndSectionOverviewService.LineAndSectionOverviewService();
            lineAndSectionOverviewService.BRIXAuthenticationHeaderValue = new LineAndSectionOverviewService.BRIXAuthenticationHeader { value = ticket };
            this.Lines = lineAndSectionOverviewService.GetLines();
        }

        private void SectionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedSection != null)
            {
                var dataTable = new DataTable();

                var data = new[]
                           {
                               "MAOP.MAOP"
                               , "DesignPressure.DesignPressure"
                               , "DesignTemperature.MinDesignTemp"
                               , "Segment.NominalWallThickness"
                               , "Segment.MaterialGrade"
                               , "Segment.SMYS"
                               , "Segment.SMTS"
                               , "NormalOperation.MinOperationTemp"
                               , "TestPressure.TestPressure"
                               , "DesignFactor.DesignFactor"
                               , "DesignFactor.Name"
                               , "GeoProfileUTM.TopOfPipe"
                           };

                SegmentationService.SegmentationService segmentationService = new SegmentationService.SegmentationService();
                segmentationService.BRIXAuthenticationHeaderValue = new SegmentationService.BRIXAuthenticationHeader { value = ticket };

                try
                {
                    var segments = segmentationService.GetSegments(this.SelectedSection.SectionOid.ToString(), data, "m", CultureInfo.CurrentCulture.Name);
                    var nHeaders = segments.Headers.Count();
                    var nSegments = segments.Segments.Count();

                    logger.Info("nSegments" + nSegments);

                    for (int s = 0; s < nSegments - 1; s++)
                    {
                        var segmentVm = segments.Segments[s];
                        var viewModel = new ViewModel();

                        for (int h = 0; h < nHeaders; h++)
                        {
                            var header = segments.Headers[h];
                            var propertyName = string.IsNullOrEmpty(header.Unit) ? header.Name : string.Format("{0} [{1}]", header.Name, header.Unit);
                            var propertyValue = segmentVm.Data[h];

                            viewModel[propertyName] = propertyValue;
                            this.SegmentData[propertyName] = propertyValue;
                        }

                        this.Segments.Add(viewModel);
                    }

                    foreach (var header in segments.Headers)
                    {
                        // var propertyName = string.IsNullOrEmpty(header.Unit) ? header.Name : string.Format("{0} [{1}]", header.Name, header.Unit);

                        dataTable.Columns.Add(string.IsNullOrEmpty(header.Unit) ? header.Name : string.Format("{0} [{1}]", header.Name, header.Unit));
                    }

                    foreach (var segmentViewModel in segments.Segments)
                    {
                        var row = dataTable.NewRow();

                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            row[i] = segmentViewModel.Data[i];
                        }

                        dataTable.Rows.Add(row);
                    }

                    this.DataView = dataTable.DefaultView;
                }
                catch (Exception exception)
                {
                    logger.Warn(exception.Message);
                }

                //dgSegment.AutoGenerateColumns = true;
                //dgSegment.DataSource = dataTable;
            }
        }
    }
}