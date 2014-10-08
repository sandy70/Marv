using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        private bool isGraphControlVisible = true;
        private bool isLineDataChartVisible;
        private bool isLineDataControlVisible;
        private bool isMapViewVisible = true;
        private bool isVertexControlVisible;
        private ILineData lineData;
        private LocationCollection locations;

        public Graph Graph
        {
            get
            {
                return (Graph) GetValue(GraphProperty);
            }

            set
            {
                SetValue(GraphProperty, value);
            }
        }

        public bool IsGraphControlVisible
        {
            get
            {
                return this.isGraphControlVisible;
            }

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
            get
            {
                return this.isLineDataChartVisible;
            }

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
            get
            {
                return this.isLineDataControlVisible;
            }

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
            get
            {
                return this.isMapViewVisible;
            }

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
            get
            {
                return this.isVertexControlVisible;
            }

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
            get
            {
                return this.lineData;
            }

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
            get
            {
                return this.locations;
            }

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
            get
            {
                return (NotificationCollection) GetValue(NotificationsProperty);
            }

            set
            {
                SetValue(NotificationsProperty, value);
            }
        }

        public string SelectedSectionId
        {
            get
            {
                return (string) GetValue(SelectedSectionIdProperty);
            }
            set
            {
                SetValue(SelectedSectionIdProperty, value);
            }
        }

        public int SelectedYear
        {
            get
            {
                return (int) GetValue(SelectedYearProperty);
            }
            set
            {
                SetValue(SelectedYearProperty, value);
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
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
            this.LineData = new LineData();
            this.LineData.SetSectionEvidence("Section 1", new Dict<int, string, VertexEvidence>());
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Locations = LocationCollection.ReadCsv(@"C:\Users\vkha\Data\CNPC\line.csv");

            this.GraphControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;

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

            this.VertexControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.VertexControl.EvidenceEntered += GraphControl_EvidenceEntered;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}