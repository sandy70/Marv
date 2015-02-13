﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marv.Common;
using Marv.Controls;
using Marv.Controls.Map;
using Marv.Properties;
using Telerik.Windows.Controls;
using Utils = Marv.Common.Utils;

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

        private LocationCollection criticalLocations;
        private bool isGraphControlVisible = true;
        private bool isMapViewVisible = true;
        private bool isMenuVisible;
        private bool isReferenceLocationsVisible = true;
        private bool isYearSliderVisible = true;
        private ILineData lineData;
        private Dict<string, int, double> locationValues;
        private LocationCollection locations;
        private LocationCollection referenceLocations;
        private Location selectedLocation;
        private Dict<string, double> selectedYearLocationValues;

        public LocationCollection CriticalLocations
        {
            get { return this.criticalLocations; }

            set
            {
                if (value.Equals(this.criticalLocations))
                {
                    return;
                }

                this.criticalLocations = value;
                this.RaisePropertyChanged();
            }
        }

        public Graph Graph
        {
            get { return (Graph) this.GetValue(GraphProperty); }
            set { this.SetValue(GraphProperty, value); }
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

        public bool IsMenuVisible
        {
            get { return this.isMenuVisible; }

            set
            {
                if (value.Equals(this.isMenuVisible))
                {
                    return;
                }

                this.isMenuVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsReferenceLocationsVisible
        {
            get { return this.isReferenceLocationsVisible; }

            set
            {
                if (value.Equals(this.isReferenceLocationsVisible))
                {
                    return;
                }

                this.isReferenceLocationsVisible = value;
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
            get { return (NotificationCollection) this.GetValue(NotificationsProperty); }
            set { this.SetValue(NotificationsProperty, value); }
        }

        public LocationCollection ReferenceLocations
        {
            get { return this.referenceLocations; }

            set
            {
                if (value.Equals(this.referenceLocations))
                {
                    return;
                }

                this.referenceLocations = value;
                this.RaisePropertyChanged();
            }
        }

        public Location SelectedLocation
        {
            get { return this.selectedLocation; }

            set
            {
                if (value == null || value.Equals(this.selectedLocation))
                {
                    return;
                }

                this.selectedLocation = value;
                this.RaisePropertyChanged();
            }
        }

        public string SelectedSectionId
        {
            get { return (string) this.GetValue(SelectedSectionIdProperty); }
            set { this.SetValue(SelectedSectionIdProperty, value); }
        }

        public int SelectedYear
        {
            get { return (int) this.GetValue(SelectedYearProperty); }
            set { this.SetValue(SelectedYearProperty, value); }
        }

        public Dict<string, double> SelectedYearLocationValues
        {
            get { return this.selectedYearLocationValues; }

            set
            {
                if (value.Equals(this.selectedYearLocationValues))
                {
                    return;
                }

                this.selectedYearLocationValues = value;
                this.RaisePropertyChanged();
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
            this.Loaded += this.MainWindow_Loaded;
        }

        private void GraphControl_GraphChanged(object sender, Graph newGraph, Graph oldGraph)
        {
            if (this.LineData == null)
            {
                this.LineData = new LineData();
                this.LineData.SetEvidence("Section 1", new Dict<int, string, VertexEvidence>());
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M &&
                (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {
                this.IsMenuVisible = !this.IsMenuVisible;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var notifiers = this.GetChildren<INotifier>();

            foreach (var notifier in notifiers)
            {
                notifier.NotificationClosed -= this.notifier_NotificationClosed;
                notifier.NotificationClosed += this.notifier_NotificationClosed;

                notifier.NotificationOpened -= this.notifier_NotificationOpened;
                notifier.NotificationOpened += this.notifier_NotificationOpened;
            }

            this.GraphControl.GraphChanged -= this.GraphControl_GraphChanged;
            this.GraphControl.GraphChanged += this.GraphControl_GraphChanged;

            this.MapView.Loaded -= MapView_Loaded;
            this.MapView.Loaded += MapView_Loaded;

            this.PolylineControl.SelectionChanged -= this.PolylineControl_SelectionChanged;
            this.PolylineControl.SelectionChanged += this.PolylineControl_SelectionChanged;

            this.YearSlider.ValueChanged -= this.YearSlider_ValueChanged;
            this.YearSlider.ValueChanged += this.YearSlider_ValueChanged;

            this.CriticalLocations = LocationCollection.ReadCsv(Settings.Default.CriticalLocationsFileName);
            this.Graph = Graph.Read(Settings.Default.NetworkFileName);
            this.LineData = FolderLineData.Read(Settings.Default.LineDataFileName);
            this.Locations = LocationCollection.ReadCsv(Settings.Default.LocationsFileName);
            this.locationValues = Utils.ReadJson<Dict<string, int, double>>(Settings.Default.LocationValuesFileName);
            this.ReferenceLocations = LocationCollection.ReadCsv(Settings.Default.ReferenceLocationsFileName);
            this.SelectedYear = this.LineData.StartYear;

            this.UpdateGraphValue();
            this.UpdateLocationValues();
        }

        private void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            this.MapView.Bounds = this.Locations.Bounds.GetPadded(0.1);
        }

        private void MenuWindowNetwork_Click(object sender, RoutedEventArgs e)
        {
            this.IsGraphControlVisible = !this.IsGraphControlVisible;
        }

        private void PolylineControl_SelectionChanged(object sender, Location location)
        {
            this.UpdateGraph();
            this.UpdateGraphValue();
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateGraph()
        {
            //if (this.SelectedLocation == null)
            //{
            //    return;
            //}

            //var crossing = (this.SelectedLocation["Crossing"] as string).ToLower();
            //var weight = this.SelectedLocation["Weight"];

            //if (crossing.Contains("river") || crossing.Contains("rail") || crossing.Contains("highway"))
            //{
            //    this.Graph = this.casingGraphs[(double) weight];
            //}
            //else
            //{
            //    this.Graph = this.graphs[(double) weight];
            //}
        }

        private void UpdateGraphValue()
        {
            if (this.SelectedLocation == null)
            {
                return;
            }

            try
            {
                var graphBelief = this.LineData.GetBelief(this.SelectedLocation.Key)[this.SelectedYear];
                var graphEvidence = this.LineData.GetEvidence(this.SelectedLocation.Key)[this.SelectedYear];

                this.Graph.Belief = graphBelief;
                this.Graph.SetEvidence(graphEvidence);
            }
            catch (Exception exp)
            {
                this.Graph.Belief = null;
                this.Graph.Evidence = null;
            }
        }

        private void UpdateLocationValues()
        {
            if (this.locationValues != null)
            {
                this.SelectedYearLocationValues = this.locationValues[null, this.SelectedYear];
            }
        }

        private void YearSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdateGraphValue();
            this.UpdateLocationValues();
        }

        private void notifier_NotificationClosed(object sender, Notification notification)
        {
            this.Notifications.Remove(notification);
        }

        private void notifier_NotificationOpened(object sender, Notification notification)
        {
            this.Notifications.Add(notification);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}