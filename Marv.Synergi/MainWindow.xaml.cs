using Marv.Common;
using Marv.Synergi.LineAndSectionOverviewService;
using Marv.Synergi.LoginService;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Marv.Synergi
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty LinesProperty =
        DependencyProperty.Register("Lines", typeof(IEnumerable<LineSummaryDTO>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public static readonly DependencyProperty SectionsProperty =
        DependencyProperty.Register("Sections", typeof(IEnumerable<SectionSummaryDTO>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SegmentDataProperty =
        DependencyProperty.Register("SegmentData", typeof(Dict<string, string>), typeof(MainWindow), new PropertyMetadata(new Dict<string, string>()));

        public static readonly DependencyProperty SelectedLineProperty =
        DependencyProperty.Register("SelectedLine", typeof(LineSummaryDTO), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedSectionProperty =
        DependencyProperty.Register("SelectedSection", typeof(SectionSummaryDTO), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty UserNameProperty =
        DependencyProperty.Register("UserName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

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

        public IEnumerable<SectionSummaryDTO> Sections
        {
            get { return (IEnumerable<SectionSummaryDTO>)GetValue(SectionsProperty); }
            set { SetValue(SectionsProperty, value); }
        }

        public Dict<string, string> SegmentData
        {
            get { return (Dict<string, string>)GetValue(SegmentDataProperty); }
            set { SetValue(SegmentDataProperty, value); }
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

                // The order here is taken from MarvToSynergiMap.xlsx
                var data = new[]
                {
                    "MAOP.MAOP",
                    "Chemistry.ChloridesPresent",
                    "Segment.CleaningPigRunsPerYear",
                    "Chemistry.CO2",
                    "Chemistry.CorrosionInhibition",
                    "ExternalCorrosion.CoveredPercent",
                    "ExternalCorrosion.DistanceFromTheSea",
                    "Chemistry.Fe",
                    "FlowParameters.GasDensity",
                    "FlowParameters.Gas_Velocity",
                    "Chemistry.Hydrocarbon",
                    "Segment.NominalOuterDiameter",
                    "PipeBook.Latitude",
                    "PipeBook.Longitude",
                    "DesignPressure.DesignPressure",
                    "FlowParameters.LiquidVelocity",
                    "ExternalCorrosion.ExternalSandMoistureContent",
                    "Chemistry.O2",
                    "FlowParameters.OilDensity",
                    "NormalOperation.NormalOperationPressure",
                    "Chemistry.pH",
                    "FlowParameters.PipeInclination",
                    "Segment.NominalWallThickness",
                    "FlowParameters.SandPresent",
                    "Segment.SMYS",
                    "ExternalCorrosion.SoilResistivity",
                    "Chemistry.Sulphides",
                    "Chemistry.WaterCut"
                };

                SegmentationService.SegmentationService segmentationService = new SegmentationService.SegmentationService();
                segmentationService.BRIXAuthenticationHeaderValue = new SegmentationService.BRIXAuthenticationHeader { value = ticket };

                try
                {
                    var segments = segmentationService.GetSegments(this.SelectedSection.SectionOid.ToString(), data, "m", CultureInfo.CurrentCulture.Name);
                    var nHeaders = segments.Headers.Count();
                    var nSegments = segments.Segments.Count();

                    logger.Info("nSegments" + nSegments);

                    var segmentData = new Dict<string, string>();
                    var properties = new Dynamic();

                    for (int s = 0; s < nSegments - 1; s++)
                    {
                        var segmentVm = segments.Segments[s];

                        for (int h = 0; h < nHeaders; h++)
                        {
                            var header = segments.Headers[h];
                            var propertyName = string.IsNullOrEmpty(header.Unit) ? header.Name : string.Format("{0} [{1}]", header.Name, header.Unit);
                            var propertyValue = segmentVm.Data[h];

                            segmentData[propertyName] = propertyValue;
                        }
                    }

                    this.SegmentData = segmentData;
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