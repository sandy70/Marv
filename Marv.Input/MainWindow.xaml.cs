using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isVertexControlVisible = false;
        private LineData lineData;

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

        public LineData LineData
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
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            this.LineDataChart.SetSelectedPoint(vertexEvidence);
            this.LineDataControl.SetSelectedCells(vertexEvidence);
        }

        private void GraphControl_GraphChanged(object sender, ValueChangedArgs<Graph> e)
        {
            this.LineData = new LineData();
            this.LineData.SectionEvidences["Section 1"] = new Dict<int, string, VertexEvidence>();
        }

        private void LineDataControl_NotificationIssued(object sender, Notification notification)
        {
            this.Notifications.Add(notification);
        }

        private void LineDataControl_SectionBeliefsChanged(object sender, EventArgs e)
        {
            this.Graph.Belief = this.LineData.SectionBeliefs[this.SelectedSectionId][this.SelectedYear];
        }

        private void LineDataControl_SectionEvidencesChanged(object sender, EventArgs e)
        {
            this.LineDataChart.UpdateBasePoints();
        }

        private void LineDataControl_SelectedCellChanged(object sender, EventArgs e)
        {
            this.Graph.Belief = this.LineData.SectionBeliefs[this.SelectedSectionId][this.SelectedYear];
            this.Graph.SetEvidence(this.LineData.SectionEvidences[this.SelectedSectionId][this.SelectedYear]);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.GraphControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;

            this.GraphControl.GraphChanged += GraphControl_GraphChanged;

            this.LineDataControl.NotificationIssued -= LineDataControl_NotificationIssued;
            this.LineDataControl.NotificationIssued += LineDataControl_NotificationIssued;

            this.LineDataControl.SectionBeliefsChanged -= LineDataControl_SectionBeliefsChanged;
            this.LineDataControl.SectionBeliefsChanged += LineDataControl_SectionBeliefsChanged;

            this.LineDataControl.SectionEvidencesChanged -= LineDataControl_SectionEvidencesChanged;
            this.LineDataControl.SectionEvidencesChanged += LineDataControl_SectionEvidencesChanged;

            this.LineDataControl.SelectedCellChanged -= LineDataControl_SelectedCellChanged;
            this.LineDataControl.SelectedCellChanged += LineDataControl_SelectedCellChanged;

            this.VertexControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.VertexControl.EvidenceEntered += GraphControl_EvidenceEntered;
        }
    }
}