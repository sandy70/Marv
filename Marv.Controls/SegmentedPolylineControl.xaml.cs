using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;

namespace Marv.Controls.Map
{
    public partial class SegmentedPolylineControl
    {
        public static readonly DependencyProperty CursorFillProperty =
            DependencyProperty.Register("CursorFill", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.YellowGreen)));

        public static readonly DependencyProperty CursorLocationProperty =
            DependencyProperty.Register("CursorLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CursorStrokeProperty =
            DependencyProperty.Register("CursorStroke", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public static readonly DependencyProperty DisabledStrokeProperty =
            DependencyProperty.Register("DisabledStroke", typeof (Brush), typeof (SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty DoubleToBrushMapProperty =
            DependencyProperty.Register("DoubleToBrushMap", typeof (IDoubleToBrushMap), typeof (SegmentedPolylineControl), new PropertyMetadata(new DoubleToBrushMap()));

        public static readonly DependencyProperty IsCursorVisibleProperty =
            DependencyProperty.Register("IsCursorVisible", typeof(bool), typeof(SegmentedPolylineControl), new PropertyMetadata(true));

        public static readonly DependencyProperty LocationsProperty =
            DependencyProperty.Register("Locations", typeof (LocationCollection), typeof (SegmentedPolylineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty NameLatitudeProperty =
            DependencyProperty.Register("NameLatitude", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(0.0, ChangedNameLatitude));

        public static readonly DependencyProperty NameLocationProperty =
            DependencyProperty.Register("NameLocation", typeof (Location), typeof (SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty NameLongitudeProperty =
            DependencyProperty.Register("NameLongitude", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(0.0, ChangedNameLongitude));

        public static readonly DependencyProperty PolylinePartsProperty =
            DependencyProperty.Register("PolylineParts", typeof (ObservableCollection<LocationCollection>), typeof (SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedLocationProperty =
            DependencyProperty.Register("SelectedLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null, OnSelectedLocationChanged));

        public static readonly DependencyProperty SimplifiedPolylinePartsProperty =
            DependencyProperty.Register("SimplifiedPolylineParts", typeof (ObservableCollection<LocationCollectionViewModel>), typeof (SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(3.0));

        public static readonly DependencyProperty ToleranceProperty =
            DependencyProperty.Register("Tolerance", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ValueLevelsProperty =
            DependencyProperty.Register("ValueLevels", typeof (Sequence<double>), typeof (SegmentedPolylineControl), new PropertyMetadata(null, ChangedValueLevels));

        private readonly Stack<Location> locationStack = new Stack<Location>();

        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };

        private bool isDragging;
        private MapView mapView;

        public Brush CursorFill
        {
            get { return (Brush) this.GetValue(CursorFillProperty); }
            set { this.SetValue(CursorFillProperty, value); }
        }

        public Location CursorLocation
        {
            get { return (Location) this.GetValue(CursorLocationProperty); }
            set { this.SetValue(CursorLocationProperty, value); }
        }

        public Brush CursorStroke
        {
            get { return (Brush) this.GetValue(CursorStrokeProperty); }
            set { this.SetValue(CursorStrokeProperty, value); }
        }

        public Brush DisabledStroke
        {
            get { return (Brush) this.GetValue(DisabledStrokeProperty); }
            set { this.SetValue(DisabledStrokeProperty, value); }
        }

        public IDoubleToBrushMap DoubleToBrushMap
        {
            get { return (IDoubleToBrushMap) this.GetValue(DoubleToBrushMapProperty); }
            set { this.SetValue(DoubleToBrushMapProperty, value); }
        }

        public bool IsCursorVisible
        {
            get { return (bool) this.GetValue(IsCursorVisibleProperty); }
            set { this.SetValue(IsCursorVisibleProperty, value); }
        }

        public LocationCollection Locations
        {
            get { return (LocationCollection) GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        public double NameLatitude
        {
            get { return (double) this.GetValue(NameLatitudeProperty); }
            set { this.SetValue(NameLatitudeProperty, value); }
        }

        public Location NameLocation
        {
            get { return (Location) this.GetValue(NameLocationProperty); }
            set { this.SetValue(NameLocationProperty, value); }
        }

        public double NameLongitude
        {
            get { return (double) this.GetValue(NameLongitudeProperty); }
            set { this.SetValue(NameLongitudeProperty, value); }
        }

        public ObservableCollection<LocationCollection> PolylineParts
        {
            get { return (ObservableCollection<LocationCollection>) this.GetValue(PolylinePartsProperty); }
            set { this.SetValue(PolylinePartsProperty, value); }
        }

        public Location SelectedLocation
        {
            get { return (Location) this.GetValue(SelectedLocationProperty); }
            set { this.SetValue(SelectedLocationProperty, value); }
        }

        public ObservableCollection<LocationCollectionViewModel> SimplifiedPolylineParts
        {
            get { return (ObservableCollection<LocationCollectionViewModel>) this.GetValue(SimplifiedPolylinePartsProperty); }

            set { this.SetValue(SimplifiedPolylinePartsProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush) this.GetValue(StrokeProperty); }
            set { this.SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public double Tolerance
        {
            get { return (double) this.GetValue(ToleranceProperty); }
            set { this.SetValue(ToleranceProperty, value); }
        }

        public Sequence<double> ValueLevels
        {
            get { return (Sequence<double>) this.GetValue(ValueLevelsProperty); }
            set { this.SetValue(ValueLevelsProperty, value); }
        }

        public SegmentedPolylineControl()
        {
            this.InitializeComponent();

            this.timer.Tick -= timer_Tick;
            this.timer.Tick += timer_Tick;

            this.ValueLevels = new Sequence<double>
            {
                0.00,
                0.25,
                0.5,
                0.75,
                1.00
            };

            this.Loaded += SegmentedPolylineControl_Loaded;
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            if (control.Locations != null)
            {
                control.CursorLocation = control.Locations.First();
                control.SelectedLocation = control.Locations.First();

                control.UpdatePolylineParts();

                control.Locations.ValueChanged += control.Locations_ValueChanged;
            }

            control.UpdateSimplifiedPolylineParts();
        }

        private static void ChangedNameLatitude(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            if (control != null)
            {
                control.NameLocation = new Location
                {
                    Latitude = control.NameLatitude,
                    Longitude = control.NameLocation.Longitude
                };
            }
        }

        private static void ChangedNameLongitude(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            if (control != null)
            {
                control.NameLocation = new Location
                {
                    Latitude = control.NameLocation.Latitude,
                    Longitude = control.NameLongitude
                };
            }
        }

        private static void ChangedValueLevels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            control.UpdatePolylineParts();
            control.UpdateSimplifiedPolylineParts();
        }

        private static void OnSelectedLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            if (control != null)
            {
                control.CursorLocation = control.SelectedLocation;
                control.IsCursorVisible = true;
            }
        }

        public void UpdateSimplifiedPolylineParts()
        {
            var mapView = this.GetParent<MapView>();
            var simplifiedPolylineParts = new ObservableCollection<LocationCollectionViewModel>();

            if (this.PolylineParts != null)
            {
                simplifiedPolylineParts.Add(this.PolylineParts.Select(locationCollection => new LocationCollectionViewModel
                {
                    Locations = locationCollection.ToPoints(mapView).Reduce(this.Tolerance).ToLocations(mapView).ToLocationCollection(),
                    Stroke = this.IsEnabled ? this.DoubleToBrushMap.Map(0) : this.DisabledStroke
                }));
            }

            this.SimplifiedPolylineParts = simplifiedPolylineParts;
        }

        public async Task UpdateSimplifiedPolylinePartsAsync()
        {
            if (this.PolylineParts != null)
            {
                var disabledStroke = this.DisabledStroke;
                var doubleToBrushMap = this.DoubleToBrushMap;
                var isEnabled = this.IsEnabled;
                var mapView = this.GetParent<MapView>();
                var polylineParts = this.PolylineParts;
                var tolerance = this.Tolerance;

                this.SimplifiedPolylineParts = new ObservableCollection<LocationCollectionViewModel>(await Task.Run(() => polylineParts.Select(locationCollection => new LocationCollectionViewModel
                {
                    Locations = locationCollection.Reduce(mapView, tolerance),
                    Stroke = isEnabled ? doubleToBrushMap.Map(0) : disabledStroke
                })));
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "IsEnabled")
            {
                this.UpdateSimplifiedPolylineParts();
            }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.OnDown();
            e.Handled = true;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                var position = e.GetPosition(this);
                this.SelectLocation(position);
            }
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private void Ellipse_TouchDown(object sender, TouchEventArgs e)
        {
            this.OnDown();
            e.Handled = true;
        }

        private void Ellipse_TouchMove(object sender, TouchEventArgs e)
        {
            if (this.isDragging)
            {
                var position = e.GetTouchPoint(this).Position;
                this.SelectLocation(position);
            }
        }

        private void Ellipse_TouchUp(object sender, TouchEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private async void Locations_ValueChanged(object sender, EventArgs e)
        {
            var locations = this.Locations;
            var valueLevels = this.ValueLevels;

            if (locations == null || valueLevels == null)
            {
                return;
            }

            this.PolylineParts = await Task.Run(() => this.Segment(locations, valueLevels));

            var mapView = this.GetParent<MapView>();
            var polylineParts = this.PolylineParts;
            var tolerance = this.Tolerance;
            var isEnabled = this.IsEnabled;
            var doubleToBrushMap = this.DoubleToBrushMap;
            var disabledStroke = this.DisabledStroke;
            var converter = new MapTransform(mapView);

            this.SimplifiedPolylineParts = new ObservableCollection<LocationCollectionViewModel>();

            foreach (var part in polylineParts)
            {
                this.SimplifiedPolylineParts.Add(await Task.Run(() => this.Reduce(part, converter, tolerance, doubleToBrushMap, isEnabled, disabledStroke)));
            }
        }

        private void MapPolyline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // We need this to raise the event on the templated parent.
            // It should be raised automatically but isn't. This is probably a bug.

            this.PolylineItemsControl.RaiseEvent(e);
        }

        private void MapPolyline_TouchDown(object sender, TouchEventArgs e)
        {
            // We need this to raise the event on the templated parent.
            // It should be raised automatically but isn't. This is probably a bug.

            this.PolylineItemsControl.RaiseEvent(e);
        }

        private void OnDown()
        {
            this.Ellipse.CaptureMouse();
            this.isDragging = true;
        }

        private void OnUp()
        {
            this.Ellipse.ReleaseMouseCapture();
            this.isDragging = false;
        }

        private void PolylineItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            this.SelectLocation(position);
        }

        private void PolylineItemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private void PolylineItemsControl_TouchDown(object sender, TouchEventArgs e)
        {
            var position = e.GetTouchPoint(this).Position;
            this.SelectLocation(position);
        }

        private void PolylineItemsControl_TouchUp(object sender, TouchEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private void RaiseSelectionChanged(Location location)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, location);
            }
        }

        private LocationCollectionViewModel Reduce(LocationCollection locationCollection, MapTransform converter, double tolerance, IDoubleToBrushMap doubleToBrushMap, bool isEnabled, Brush disabledStroke)
        {
            return new LocationCollectionViewModel
            {
                Locations = new LocationCollection(locationCollection.Reduce(converter, tolerance)),
                //Stroke = isEnabled ? doubleToBrushMap.Map(locationCollection[1].Value) : disabledStroke
            };
        }

        private ObservableCollection<LocationCollection> Segment(IEnumerable<Location> locations, Sequence<double> valueLevels)
        {
            var oldBinIndex = -1;
            var locationCollections = new ObservableCollection<LocationCollection>();
            LocationCollection locationCollection = null;

            foreach (var location in locations)
            {
                var newBinIndex = valueLevels.GetBinIndex(0);

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
                    locationCollections.Add(locationCollection);
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

            return locationCollections;
        }

        private void SegmentedPolylineControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdatePolylineParts();
            this.UpdateSimplifiedPolylineParts();

            this.mapView = this.GetParent<MapView>();

            if (this.mapView != null)
            {
                // this.mapView.ViewportMoved += this.mapView_ViewportMoved;
                this.mapView.ZoomLevelChanged += this.mapView_ZoomLevelChanged;
            }
        }

        private void SelectLocation(Point position)
        {
            var map = this.GetParent<MapControl.Map>();
            var mLocation = map.ViewportPointToLocation(position);
            var location = new Location
            {
                Latitude = mLocation.Latitude,
                Longitude = mLocation.Longitude
            };
            var nearestLocation = this.Locations.GetLocationNearestTo(location);

            this.CursorLocation = nearestLocation;

            this.locationStack.Push(nearestLocation);

            if (!this.timer.IsEnabled)
            {
                this.timer.Start();
            }
        }

        private void UpdatePolylineParts()
        {
            if (this.ValueLevels == null ||
                this.Locations == null)
            {
                return;
            }

            var oldBinIndex = -1;
            var polylineParts = new ObservableCollection<LocationCollection>();
            LocationCollection locationCollection = null;

            foreach (var location in this.Locations)
            {
                var newBinIndex = this.ValueLevels.GetBinIndex(0);

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

        private async void mapView_ZoomLevelChanged(object sender, int zoom)
        {
            await this.UpdateSimplifiedPolylinePartsAsync();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.locationStack.Count > 0)
            {
                var location = this.locationStack.Pop();

                this.SelectedLocation = location;
                this.RaiseSelectionChanged(location);

                this.locationStack.Clear();
                this.timer.Stop();
            }
        }

        public event EventHandler<Location> SelectionChanged;
    }
}