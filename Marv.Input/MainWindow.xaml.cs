using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common;
using Microsoft.Win32;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (MainWindow), new PropertyMetadata(new NotificationCollection()));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (MainWindow), new PropertyMetadata(int.MinValue));

        private string chartTitle;
        private HorizontalAxisQuantity horizontalAxisQuantity = HorizontalAxisQuantity.Section;
        private bool isGraphControlVisible = true;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isVertexControlVisible = true;
        private ILineData lineData;
        private string lineDataFileName;
        private LocationCollection locations;
        private Location selectedLocation;

        public string ChartTitle
        {
            get { return this.chartTitle; }

            set
            {
                if (value.Equals(this.chartTitle))
                {
                    return;
                }

                this.chartTitle = value;
                this.RaisePropertyChanged();
            }
        }

        public Graph Graph
        {
            get { return (Graph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public HorizontalAxisQuantity HorizontalAxisQuantity
        {
            get { return this.horizontalAxisQuantity; }

            set
            {
                if (value.Equals(this.horizontalAxisQuantity))
                {
                    return;
                }

                this.horizontalAxisQuantity = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsGraphControlVisible
        {
            get { return this.isGraphControlVisible; }

            set
            {
                if (value.Equals(this.isGraphControlVisible))
                {
                    return;
                }

                this.isGraphControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLineDataChartVisible
        {
            get { return this.isLineDataChartVisible; }

            set
            {
                if (value.Equals(this.isLineDataChartVisible))
                {
                    return;
                }

                this.isLineDataChartVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLineDataControlVisible
        {
            get { return this.isLineDataControlVisible; }

            set
            {
                if (value.Equals(this.isLineDataControlVisible))
                {
                    return;
                }

                this.isLineDataControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsVertexControlVisible
        {
            get { return this.isVertexControlVisible; }

            set
            {
                if (value.Equals(this.isVertexControlVisible))
                {
                    return;
                }

                this.isVertexControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public ILineData LineData
        {
            get { return this.lineData; }

            set
            {
                if (value.Equals(this.lineData))
                {
                    return;
                }

                this.lineData = value;
                this.RaisePropertyChanged();
            }
        }

        public LocationCollection Locations
        {
            get { return this.locations; }

            set
            {
                if (value.Equals(this.locations))
                {
                    return;
                }

                this.locations = value;
                this.RaisePropertyChanged();
            }
        }

        public NotificationCollection Notifications
        {
            get { return (NotificationCollection) GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        public Location SelectedLocation
        {
            get { return this.selectedLocation; }

            set
            {
                if (value.Equals(this.selectedLocation))
                {
                    return;
                }

                this.selectedLocation = value;
                this.RaisePropertyChanged();
            }
        }

        public string SelectedSectionId
        {
            get { return (string) GetValue(SelectedSectionIdProperty); }
            set { SetValue(SelectedSectionIdProperty, value); }
        }

        public int SelectedYear
        {
            get { return (int) GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();

            this.Loaded -= MainWindow_Loaded;
            this.Loaded += MainWindow_Loaded;

            this.Loaded -= MainWindow_Loaded_LineDataControl;
            this.Loaded += MainWindow_Loaded_LineDataControl;
        }

        private object GetChartCategory()
        {
            return this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? this.SelectedSectionId : this.SelectedYear as object;
        }

        private object GetChartCategory(string sectionId, int year)
        {
            return this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? sectionId : year as object;
        }

        private Dict<object, VertexEvidence> GetChartEvidence()
        {
            var vertexEvidences = new Dict<object, VertexEvidence>();

            var isHorizontalAxisSections = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section;

            var categories = isHorizontalAxisSections
                                 ? this.LineData.SectionIds
                                 : Enumerable.Range(this.LineData.StartYear, this.LineData.EndYear - this.LineData.StartYear + 1).Select(i => i as object);

            foreach (var category in categories)
            {
                var sectionId = isHorizontalAxisSections ? category as string : this.SelectedSectionId;
                var year = isHorizontalAxisSections ? this.SelectedYear : (int) category;

                if (sectionId == null)
                {
                    continue;
                }

                var vertexEvidence = this.LineData.GetEvidence(sectionId)[year][this.Graph.SelectedVertex.Key];

                vertexEvidences[category] = vertexEvidence;
            }

            return vertexEvidences;
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            if (vertexEvidence == null)
            {
                this.LineDataChart.RemoveUserEvidence(this.GetChartCategory());
                this.LineDataControl.ClearSelectedCell();
            }
            else
            {
                this.LineDataChart.SetUserEvidence(this.GetChartCategory(), vertexEvidence);
                this.LineDataControl.SetSelectedCells(vertexEvidence);
            }
        }

        private void GraphControl_GraphChanged(object sender, Graph oldGraph, Graph newGraph)
        {
            if (this.LineData == null)
            {
                this.LineData = new LineData();
                this.LineData.SetEvidence("Section 1", new Dict<int, string, VertexEvidence>());
            }
        }

        private async void GraphControl_SelectionChanged(object sender, Vertex e)
        {
            if (this.LineData != null)
            {
                var selectedVertex = this.Graph.SelectedVertex;

                this.LineDataControl.ClearRows();

                foreach (var sectionId in this.LineData.SectionIds)
                {
                    var sectionEvidence = await this.LineData.GetEvidenceAsync(sectionId);

                    if (selectedVertex != null)
                    {
                        this.LineDataControl.AddRow(sectionId, sectionEvidence[null, selectedVertex.Key]);
                    }
                }

                if (this.Graph.SelectedVertex == null)
                {
                    return;
                }

                var intervals = this.Graph.Network.GetIntervals(this.Graph.SelectedVertex.Key);

                this.LineDataChart.SetVerticalAxis(selectedVertex.SafeMax, selectedVertex.SafeMin, intervals);
                this.LineDataChart.SetUserEvidence(this.GetChartEvidence());
            }
        }

        private void LineDataChart_EvidenceGenerated(object sender, EvidenceGeneratedEventArgs e)
        {
            var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(e.EvidenceString);

            var sectionId = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? e.Category as string : this.SelectedSectionId;
            var year = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? this.SelectedYear : (int) e.Category;

            if (vertexEvidence.Type != VertexEvidenceType.Invalid)
            {
                var sectionEvidence = this.LineData.GetEvidence(sectionId);
                sectionEvidence[year][this.Graph.SelectedVertex.Key] = vertexEvidence;
                this.LineData.SetEvidence(sectionId, sectionEvidence);

                this.LineDataChart.SetUserEvidence(e.Category, vertexEvidence);
                this.LineDataControl.SetCell(sectionId, year, vertexEvidence);
            }
        }

        private void LineDataChart_HorizontalAxisQuantityChanged(object sender, HorizontalAxisQuantity e)
        {
            var vertexEvidences = this.GetChartEvidence();

            if (vertexEvidences == null || vertexEvidences.Count > 0)
            {
                this.LineDataChart.SetUserEvidence(vertexEvidences);
                this.UpdateChartTitle();
            }
        }

        private void LineDataOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Marv.LineData.FileDescription + "|*." + Marv.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.lineDataFileName = dialog.FileName;

                var directoryName = Path.GetDirectoryName(dialog.FileName);

                if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
                {
                    // This is a folder line data
                    this.LineData = FolderLineData.Read(dialog.FileName);
                }
                else
                {
                    this.LineData = Marv.LineData.Read(dialog.FileName);
                }
            }
        }

        private void LineDataSaveAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = Marv.LineData.FileDescription + "|*." + Marv.LineData.FileExtension,
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                this.lineDataFileName = dialog.FileName;
                this.LineData.Write(this.lineDataFileName);
            }
        }

        private void LineDataSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineDataSaveAs();
        }

        private void LineDataSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.lineDataFileName == null)
            {
                this.LineDataSaveAs();
            }
            else
            {
                this.LineData.Write(this.lineDataFileName);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.PropertyChanged -= MainWindow_PropertyChanged;
            this.PropertyChanged += MainWindow_PropertyChanged;

            this.GraphControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;

            this.GraphControl.GraphChanged -= GraphControl_GraphChanged;
            this.GraphControl.GraphChanged += GraphControl_GraphChanged;

            this.GraphControl.SelectionChanged -= GraphControl_SelectionChanged;
            this.GraphControl.SelectionChanged += GraphControl_SelectionChanged;

            this.LineDataChart.EvidenceGenerated -= LineDataChart_EvidenceGenerated;
            this.LineDataChart.EvidenceGenerated += LineDataChart_EvidenceGenerated;

            this.LineDataChart.HorizontalAxisQuantityChanged -= LineDataChart_HorizontalAxisQuantityChanged;
            this.LineDataChart.HorizontalAxisQuantityChanged += LineDataChart_HorizontalAxisQuantityChanged;

            this.VertexControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.VertexControl.EvidenceEntered += GraphControl_EvidenceEntered;
        }

        private async void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LineData")
            {
                if (this.LineData != null)
                {
                    this.LineDataControl.ClearRows();

                    foreach (var sectionId in this.LineData.SectionIds)
                    {
                        this.LineDataControl.AddRow(sectionId, (await this.LineData.GetEvidenceAsync(sectionId))[null, this.Graph.SelectedVertex.Key]);
                    }
                }
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RunLineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var sectionId in this.LineData.SectionIds)
            {
                var sectionEvidence = this.lineData.GetEvidence(sectionId);
                this.lineData.SetBelief(sectionId, this.Graph.Network.Run(sectionEvidence));
            }
        }

        private void RunSectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sectionEvidence = this.LineData.GetEvidence(this.SelectedSectionId);

            var sectionBelief = this.Graph.Network.Run(sectionEvidence);

            this.Graph.Belief = sectionBelief[this.SelectedYear];
            this.LineData.SetBelief(this.SelectedSectionId, sectionBelief);
        }

        private void UpdateChartTitle()
        {
            this.ChartTitle = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Section ? "Year: " + this.SelectedYear : "Section: " + this.SelectedSectionId;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}