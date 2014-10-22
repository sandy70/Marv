﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common.Graph;
using Marv.Controls.Map;
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
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isMapViewVisible;
        private bool isVertexControlVisible = true;
        private bool isYearSliderVisible;
        private ILineData lineData;
        private IDoubleToBrushMap locationValueToBrushMap = new LocationValueToBrushMap();
        private Dict<string, int, double> locationValues;
        private LocationCollection locations;
        private Location selectedLocation;
        private LocationRect startExtent;
        private Sequence<double> valueLevels;

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

        public bool IsYearSliderVisible
        {
            get { return this.isYearSliderVisible; }

            set
            {
                if (value.Equals(this.isYearSliderVisible))
                {
                    return;
                }

                this.isYearSliderVisible = value;
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

        public IDoubleToBrushMap LocationValueToBrushMap
        {
            get { return this.locationValueToBrushMap; }

            set
            {
                if (value.Equals(this.locationValueToBrushMap))
                {
                    return;
                }

                this.locationValueToBrushMap = value;
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

        public LocationRect StartExtent
        {
            get { return this.startExtent; }

            set
            {
                if (value.Equals(this.startExtent))
                {
                    return;
                }

                this.startExtent = value;
                this.RaisePropertyChanged();
            }
        }

        public Sequence<double> ValueLevels
        {
            get { return this.valueLevels; }

            set
            {
                if (value.Equals(this.valueLevels))
                {
                    return;
                }

                this.valueLevels = value;
                this.RaisePropertyChanged();
            }
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

        private void GraphControl_GraphChanged(object sender, Graph newGraph, Graph oldGraph)
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
            if (this.SelectedLocation == null)
            {
                return;
            }

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
            if (this.SelectedLocation == null)
            {
                return;
            }

            var graphBelief = this.LineData.GetSectionBelief(this.SelectedLocation.Key)[this.SelectedYear];
            var graphEvidence = this.LineData.GetSectionEvidence(this.SelectedLocation.Key)[this.SelectedYear];

            this.Graph.Belief = graphBelief;
            this.Graph.SetEvidence(graphEvidence);
        }

        private void YearSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Locations.Value = this.locationValues[null, this.SelectedYear];
            this.UpdateGraphValue();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}