using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;

namespace Marv.Controls.Map
{
    public partial class PolylineControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CursorFillProperty =
            DependencyProperty.Register("CursorFill", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.YellowGreen)));

        public static readonly DependencyProperty CursorStrokeProperty =
            DependencyProperty.Register("CursorStroke", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public static readonly DependencyProperty DisabledStrokeProperty =
            DependencyProperty.Register("DisabledStroke", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof (bool), typeof (PolylineControl), new PropertyMetadata(false));

        public static readonly DependencyProperty LocationValuesProperty =
            DependencyProperty.Register("LocationValues", typeof (Dict<string, double>), typeof (PolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty LocationsProperty =
            DependencyProperty.Register("Locations", typeof (IEnumerable<Location>), typeof (PolylineControl), new PropertyMetadata(null, LocationsChanged));

        public static readonly DependencyProperty SelectedLocationProperty =
            DependencyProperty.Register("SelectedLocation", typeof (Location), typeof (PolylineControl), new PropertyMetadata(null, ChangedSelectedLocation));

        public static readonly DependencyProperty SkeletonZoomLevelProperty =
            DependencyProperty.Register("SkeletonZoomLevel", typeof (double), typeof (PolylineControl), new PropertyMetadata(15.0));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue)));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (PolylineControl), new PropertyMetadata(3.0));

        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };

        private Location cursorLocation;
        private Brush displayStroke = new SolidColorBrush(Colors.DeepSkyBlue);
        private IEnumerator<Location> locationsEnumerator;
        private MapView mapView;
        private ObservableCollection<LocationCollection> polylineParts;
        private IEnumerable<Location> simplifiedLocations;
        private ObservableCollection<LocationCollectionViewModel> simplifiedPolylineParts;
        private Sequence<double> valueLevels = new Sequence<double> { 0.00, 0.25, 0.50, 0.75, 1.00 };
        private LocationCollection visibleLocations = new LocationCollection();

        public Brush CursorFill
        {
            get { return (Brush) this.GetValue(CursorFillProperty); }
            set { this.SetValue(CursorFillProperty, value); }
        }

        public Location CursorLocation
        {
            get { return this.cursorLocation; }

            set
            {
                if (value.Equals(this.cursorLocation))
                {
                    return;
                }

                this.cursorLocation = value;
                this.RaisePropertyChanged();
            }
        }

        public Brush CursorStroke
        {
            get { return (Brush) this.GetValue(CursorStrokeProperty); }
            set { this.SetValue(CursorStrokeProperty, value); }
        }

        public Brush DisabledStroke
        {
            get { return (Brush) GetValue(DisabledStrokeProperty); }
            set { SetValue(DisabledStrokeProperty, value); }
        }

        public Brush DisplayStroke
        {
            get { return this.displayStroke; }

            set
            {
                if (value.Equals(this.displayStroke))
                {
                    return;
                }

                this.displayStroke = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return (bool) this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        public Dict<string, double> LocationValues
        {
            get { return (Dict<string, double>) GetValue(LocationValuesProperty); }
            set { SetValue(LocationValuesProperty, value); }
        }

        public LocationCollection Locations
        {
            get { return (LocationCollection) this.GetValue(LocationsProperty); }

            set { this.SetValue(LocationsProperty, value); }
        }

        public ObservableCollection<LocationCollection> PolylineParts
        {
            get { return this.polylineParts; }

            set
            {
                if (value.Equals(this.polylineParts))
                {
                    return;
                }

                this.polylineParts = value;
                this.RaisePropertyChanged();
            }
        }

        public Location SelectedLocation
        {
            get { return (Location) this.GetValue(SelectedLocationProperty); }

            set { this.SetValue(SelectedLocationProperty, value); }
        }

        public IEnumerable<Location> SimplifiedLocations
        {
            get { return this.simplifiedLocations; }

            set
            {
                if (value.Equals(this.simplifiedLocations))
                {
                    return;
                }

                this.simplifiedLocations = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<LocationCollectionViewModel> SimplifiedPolylineParts
        {
            get { return this.simplifiedPolylineParts; }

            set
            {
                if (value.Equals(this.simplifiedPolylineParts))
                {
                    return;
                }

                this.simplifiedPolylineParts = value;
                this.RaisePropertyChanged();
            }
        }

        public double SkeletonZoomLevel
        {
            get { return (double) GetValue(SkeletonZoomLevelProperty); }
            set { SetValue(SkeletonZoomLevelProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush) this.GetValue(StrokeProperty); }
            set { this.SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double) this.GetValue(StrokeThicknessProperty); }
            set { this.SetValue(StrokeThicknessProperty, value); }
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

        public LocationCollection VisibleLocations
        {
            get { return this.visibleLocations; }

            set
            {
                if (value.Equals(this.visibleLocations))
                {
                    return;
                }

                this.visibleLocations = value;
                this.RaisePropertyChanged();
            }
        }

        public PolylineControl()
        {
            this.InitializeComponent();

            this.Loaded -= this.PolylineControl_Loaded;
            this.Loaded += this.PolylineControl_Loaded;
        }

        private static void ChangedSelectedLocation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PolylineControl;
            control.CursorLocation = control.SelectedLocation;
            control.RaiseSelectionChanged(control.SelectedLocation);
        }

        private static void LocationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PolylineControl;

            if (control.Locations != null)
            {
                if (control.Locations != null)
                {
                    control.CursorLocation = control.Locations.First();
                    control.IsSelected = control.CursorLocation != null;
                }
            }
        }

        public void UpdatePolylineParts()
        {
            if (this.Locations == null || this.ValueLevels == null)
            {
                return;
            }

            var oldBinIndex = -1;
            var polylineParts = new ObservableCollection<LocationCollection>();
            LocationCollection locationCollection = null;

            foreach (var location in this.Locations)
            {
                var newBinIndex = this.ValueLevels.GetBinIndex(location.Value);

                if (newBinIndex != oldBinIndex)
                {
                    if (locationCollection == null)
                    {
                        locationCollection = new LocationCollection { location };
                    }
                    else
                    {
                        var mid = Common.Utils.Mid(locationCollection.Last(), location);
                        locationCollection.Add(mid);
                        locationCollection = new LocationCollection { mid, location };
                    }

                    polylineParts.Add(locationCollection);
                }
                else
                {
                    if (locationCollection != null)
                    {
                        locationCollection.Add(location);
                    }
                }

                oldBinIndex = newBinIndex;
            }

            this.PolylineParts = polylineParts;
        }

        public void UpdateSimplifiedPolylineParts()
        {
            var mapView = this.GetParent<MapView>();
            var simplifiedPolylineParts = new ObservableCollection<LocationCollectionViewModel>();

            if (this.PolylineParts != null)
            {
                simplifiedPolylineParts.Add(this.PolylineParts.Select(locationCollection => new LocationCollectionViewModel
                {
                    Locations = locationCollection.ToPoints(mapView).Reduce(5).ToLocations(mapView).ToLocationCollection(),
                    Stroke = this.IsEnabled ? this.GetStroke(locationCollection[1].Value) : this.DisabledStroke
                }));
            }

            this.SimplifiedPolylineParts = simplifiedPolylineParts;
        }

        private Location GetNearestLocation(Point position)
        {
            var location = this.mapView.ViewportPointToLocation(position);
            var nearestLocation = this.Locations.GetLocationNearestTo(location);
            return nearestLocation;
        }

        private Brush GetStroke(double value)
        {
            if (value < 0.2)
            {
                return new SolidColorBrush(Colors.Green);
            }

            if (value < 0.8)
            {
                return new SolidColorBrush(Colors.Yellow);
            }

            return new SolidColorBrush(Colors.Red);
        }

        private void PolylineControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateDisplayStroke();
        }

        private void PolylineControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.mapView = this.GetParent<MapView>();
            this.UpdateDisplayStroke();

            this.IsEnabledChanged -= PolylineControl_IsEnabledChanged;
            this.IsEnabledChanged += PolylineControl_IsEnabledChanged;

            this.mapView.ViewportMoved -= mapView_ViewportMoved;
            this.mapView.ViewportMoved += mapView_ViewportMoved;

            this.mapView.ZoomLevelChanged -= this.mapView_ZoomLevelChanged;
            this.mapView.ZoomLevelChanged += this.mapView_ZoomLevelChanged;

            this.timer.Tick -= timer_Tick;
            this.timer.Tick += timer_Tick;

            this.Ellipse.MouseDown -= this.Ellipse_MouseDown;
            this.Ellipse.MouseDown += this.Ellipse_MouseDown;

            this.Ellipse.MouseMove -= this.Ellipse_MouseMove;
            this.Ellipse.MouseMove += this.Ellipse_MouseMove;

            this.Ellipse.MouseUp -= this.Ellipse_MouseUp;
            this.Ellipse.MouseUp += this.Ellipse_MouseUp;

            this.Ellipse.TouchDown -= this.Ellipse_TouchDown;
            this.Ellipse.TouchDown += this.Ellipse_TouchDown;

            this.Ellipse.TouchMove -= this.Ellipse_TouchMove;
            this.Ellipse.TouchMove += this.Ellipse_TouchMove;

            this.Ellipse.TouchUp -= this.Ellipse_TouchUp;
            this.Ellipse.TouchUp += this.Ellipse_TouchUp;

            this.MapPolyline.MouseDown -= this.MapPolyline_MouseDown;
            this.MapPolyline.MouseDown += this.MapPolyline_MouseDown;

            this.MapPolyline.TouchDown -= this.MapPolyline_TouchDown;
            this.MapPolyline.TouchDown += this.MapPolyline_TouchDown;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RaiseSelectionChanged(Location location)
        {
            this.SelectedLocation = location;

            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, location);
            }
        }

        private void UpdateDisplayStroke()
        {
            this.DisplayStroke = this.IsEnabled ? this.Stroke : this.DisabledStroke;
        }

        private void UpdateVisibleLocations()
        {
            if (this.mapView.ZoomLevel > this.SkeletonZoomLevel)
            {
                this.timer.Stop();

                this.locationsEnumerator = this.Locations.GetEnumerator();

                this.timer.Start();
            }
            else
            {
                this.timer.Stop();
                this.VisibleLocations = new LocationCollection();
            }
        }

        private void mapView_ViewportMoved(object sender, Location e)
        {
            this.UpdateVisibleLocations();
        }

        private void mapView_ZoomLevelChanged(object sender, int e)
        {
            if (this.Locations == null)
            {
                return;
            }

            this.SimplifiedLocations = this.Locations
                                           .ToPoints(this.mapView)
                                           .Reduce(5)
                                           .ToLocations(this.mapView);

            this.UpdateVisibleLocations();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var scaledBounds = this.mapView.Bounds.Scale(3);

            for (var i = 0; i < 1000; i++)
            {
                if (this.locationsEnumerator.MoveNext())
                {
                    var location = this.locationsEnumerator.Current;

                    if (scaledBounds.Contains(location))
                    {
                        this.VisibleLocations.AddUnique(location);
                    }
                    else
                    {
                        this.VisibleLocations.Remove(location);
                    }
                }
                else
                {
                    this.timer.Stop();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<Location> SelectionChanged;
    }
}