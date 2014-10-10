using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Marv.Map;
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

        private readonly Dict<double, Graph> casingGraphs = new Dict<double, Graph>();
        private readonly Dict<double, Graph> graphs = new Dict<double, Graph>();
        private bool isGraphControlVisible = true;
        private bool isLineDataChartVisible;
        private bool isLineDataControlVisible;
        private bool isMapViewVisible = true;
        private bool isVertexControlVisible;
        private ILineData lineData;
        private LocationCollection locations;
        private Location selectedLocation;

        public Graph Graph
        {
            get { return (Graph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
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

        public bool IsMapViewVisible
        {
            get { return this.isMapViewVisible; }

            set
            {
                if (value.Equals(this.isMapViewVisible))
                {
                    return;
                }

                this.isMapViewVisible = value;
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
            this.Loaded += MainWindow_Loaded;
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            this.LineDataChart.UpdateEvidence(vertexEvidence);
            this.LineDataControl.SetSelectedCells(vertexEvidence);
        }

        private void GraphControl_GraphChanged(object sender, ValueChangedArgs<Graph> e)
        {
            if (this.LineData == null)
            {
                this.LineData = new LineData();
                this.LineData.SetSectionEvidence("Section 1", new Dict<int, string, VertexEvidence>());
            }
        }

        private void LineDataControl_EvidenceChanged(object sender, CellModel cellModel, VertexEvidence vertexEvidence)
        {
            this.LineDataChart.UpdateEvidence(vertexEvidence, cellModel);
        }

        private void LineDataControl_NotificationClosed(object sender, Notification notification)
        {
            this.Notifications.Remove(notification);
        }

        private void LineDataControl_NotificationOpened(object sender, Notification notification)
        {
            this.Notifications.Add(notification);
        }

        private void LineDataControl_SectionBeliefsChanged(object sender, EventArgs e)
        {
            if (this.SelectedSectionId != null && this.SelectedYear > 0)
            {
                this.Graph.Belief = this.LineData.GetSectionBelief(this.SelectedSectionId)[this.SelectedYear];
            }
        }

        private void LineDataControl_SectionEvidencesChanged(object sender, EventArgs e)
        {
            this.LineDataChart.UpdateBasePoints();
        }

        private void LineDataControl_SelectedCellChanged(object sender, EventArgs e)
        {
            this.Graph.Belief = this.LineData.GetSectionBelief(this.SelectedSectionId)[this.SelectedYear];
            this.Graph.SetEvidence(this.LineData.GetSectionEvidence(this.SelectedSectionId)[this.SelectedYear]);
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            const string dataDirRoot = @"C:\Users\vkha\Data\WestPipeline";
            this.LineData = FolderLineData.Read(@"C:\Users\vkha\Data\WestPipeline\LineData\WestPipeline.marv-linedata");
            this.Locations = LocationCollection.ReadCsv(@"C:\Users\vkha\Data\WestPipeline\line.csv");
            this.SelectedYear = this.LineData.StartYear;

            var casingNetworkFiles = new Dict<double, string>
            {
                { 8.0, "CASED_PIPELINE_8.net" },
                { 8.8, "CASED_PIPELINE_88.net" },
                { 10.0, "CASED_PIPELINE_10.net" },
                { 11.0, "CASED_PIPELINE_11.net" },
                { 13.3, "CASED_PIPELINE_133.net" },
                { 14.3, "CASED_PIPELINE_143.net" },
            };

            var networkFiles = new Dict<double, string>
            {
                { 8.0, "MODEL_modified_08262014a_8.net" },
                { 8.8, "MODEL_modified_08262014a_88.net" },
                { 10.0, "MODEL_modified_08262014a_10.net" },
                { 11.0, "MODEL_modified_08262014a_11.net" },
                { 13.3, "MODEL_modified_08262014a_133.net" },
                { 14.3, "MODEL_modified_08262014a_143.net" },
            };

            var graphReadingNotification = new Notification
            {
                Description = "Reading networks...",
                Value = 0
            };

            this.Notifications.Add(graphReadingNotification);

            var total = casingNetworkFiles.Count + networkFiles.Count;
            var done = 0.0;

            foreach (var size in casingNetworkFiles.Keys)
            {
                var fileName = Path.Combine(dataDirRoot, "Networks", casingNetworkFiles[size]);
                this.casingGraphs[size] = await Task.Run(() => Graph.Read(fileName));

                done++;
                graphReadingNotification.Value = done / total;
            }

            foreach (var size in networkFiles.Keys)
            {
                var fileName = Path.Combine(dataDirRoot, "Networks", networkFiles[size]);
                this.graphs[size] = await Task.Run(() => Graph.Read(fileName));

                done++;
                graphReadingNotification.Value = done / total;
            }

            this.Notifications.Remove(graphReadingNotification);

            this.UpdateGraph();
            this.UpdateGraphValue();

            this.GraphControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;

            this.GraphControl.GraphChanged -= GraphControl_GraphChanged;
            this.GraphControl.GraphChanged += GraphControl_GraphChanged;

            this.LineDataControl.EvidenceChanged -= LineDataControl_EvidenceChanged;
            this.LineDataControl.EvidenceChanged += LineDataControl_EvidenceChanged;

            this.LineDataControl.NotificationClosed -= LineDataControl_NotificationClosed;
            this.LineDataControl.NotificationClosed += LineDataControl_NotificationClosed;

            this.LineDataControl.NotificationOpened -= LineDataControl_NotificationOpened;
            this.LineDataControl.NotificationOpened += LineDataControl_NotificationOpened;

            this.LineDataControl.SectionBeliefsChanged -= LineDataControl_SectionBeliefsChanged;
            this.LineDataControl.SectionBeliefsChanged += LineDataControl_SectionBeliefsChanged;

            this.LineDataControl.SectionEvidencesChanged -= LineDataControl_SectionEvidencesChanged;
            this.LineDataControl.SectionEvidencesChanged += LineDataControl_SectionEvidencesChanged;

            this.LineDataControl.SelectedCellChanged -= LineDataControl_SelectedCellChanged;
            this.LineDataControl.SelectedCellChanged += LineDataControl_SelectedCellChanged;

            this.PolylineControl.SelectionChanged -= PolylineControl_SelectionChanged;
            this.PolylineControl.SelectionChanged += PolylineControl_SelectionChanged;

            this.VertexControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.VertexControl.EvidenceEntered += GraphControl_EvidenceEntered;

            this.YearSlider.ValueChanged -= YearSlider_ValueChanged;
            this.YearSlider.ValueChanged += YearSlider_ValueChanged;
        }

        private void PolylineControl_SelectionChanged(object sender, Location location)
        {
            this.UpdateGraph();
            this.UpdateGraphValue();
        }

        private void UpdateGraph()
        {
            var crossing = (this.SelectedLocation["Crossing"] as string).ToLower();
            var weight = this.SelectedLocation["Weight"];

            if (crossing.Contains("river") || crossing.Contains("rail") || crossing.Contains("highway"))
            {
                this.Graph = this.casingGraphs[(double) weight];
            }
            else
            {
                this.Graph = this.graphs[(double) weight];
            }
        }

        private void UpdateGraphValue()
        {
            var graphBelief = this.LineData.GetSectionBelief(this.SelectedLocation.Key)[this.SelectedYear];
            var graphEvidence = this.LineData.GetSectionEvidence(this.SelectedLocation.Key)[this.SelectedYear];

            this.Graph.Belief = graphBelief;
            this.Graph.SetEvidence(graphEvidence);
        }

        private void YearSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdateGraphValue();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}