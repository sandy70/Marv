using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Marv.Map;

namespace Marv.Controls.Map
{
    public partial class SegmentedPolylineControl
    {
        public static readonly DependencyProperty DisabledStrokeProperty =
            DependencyProperty.Register("DisabledStroke", typeof (Brush), typeof (SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty DoubleToBrushMapProperty =
            DependencyProperty.Register("DoubleToBrushMap", typeof (IDoubleToBrushMap), typeof (SegmentedPolylineControl), new PropertyMetadata(new DoubleToBrushMap()));

        public static readonly DependencyProperty NameLatitudeProperty =
            DependencyProperty.Register("NameLatitude", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(0.0, ChangedNameLatitude));

        public static readonly DependencyProperty NameLocationProperty =
            DependencyProperty.Register("NameLocation", typeof (Location), typeof (SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty NameLongitudeProperty =
            DependencyProperty.Register("NameLongitude", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(0.0, ChangedNameLongitude));

        public static readonly DependencyProperty PolylinePartsProperty =
            DependencyProperty.Register("PolylineParts", typeof (IEnumerable<LocationCollection>), typeof (SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SimplifiedPolylinePartsProperty =
            DependencyProperty.Register("SimplifiedPolylineParts", typeof (IEnumerable<LocationCollectionViewModel>), typeof (SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(3.0));

        public static readonly DependencyProperty ToleranceProperty =
            DependencyProperty.Register("Tolerance", typeof (double), typeof (SegmentedPolylineControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ValueLevelsProperty =
            DependencyProperty.Register("ValueLevels", typeof (Sequence<double>), typeof (SegmentedPolylineControl), new PropertyMetadata(null));

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

        public IEnumerable<LocationCollection> PolylineParts
        {
            get { return (IEnumerable<LocationCollection>) this.GetValue(PolylinePartsProperty); }
            set { this.SetValue(PolylinePartsProperty, value); }
        }

        public IEnumerable<LocationCollectionViewModel> SimplifiedPolylineParts
        {
            get { return (IEnumerable<LocationCollectionViewModel>) this.GetValue(SimplifiedPolylinePartsProperty); }

            set { this.SetValue(SimplifiedPolylinePartsProperty, value); }
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
            this.ValueLevels = new Sequence<double>
            {
                0.00, 0.25, 0.5, 0.75, 1.00
            };

            this.Loaded += SegmentedPolylineControl_Loaded;
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

        public void RaiseSelectionChanged(Location location)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, location);
            }
        }

        public void UpdatePolylineParts()
        {
            var oldBinIndex = -1;
            var polylineParts = new List<LocationCollection>();
            LocationCollection locationCollection = null;

            foreach (var location in this.Locations)
            {
                var newBinIndex = this.ValueLevels.GetBinIndex(location.Value);
                
                if (newBinIndex != oldBinIndex)
                {
                    if (locationCollection == null)
                    {
                        locationCollection = new LocationCollection();
                        locationCollection.Add(location);
                    }
                    else
                    {
                        var mid = Marv.Map.Utils.Mid(locationCollection.Last(), location);
                        locationCollection.Add(mid);
                        locationCollection = new LocationCollection();
                        locationCollection.Add(mid);
                        locationCollection.Add(location);
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
            var mapView = this.FindParent<MapView>();
            var simplifiedLocationCollections = new List<LocationCollectionViewModel>();

            if (this.PolylineParts != null)
            {
                simplifiedLocationCollections.AddRange(this.PolylineParts.Select(locationCollection => new LocationCollectionViewModel
                {
                    Locations = locationCollection.ToPoints(mapView).Reduce(this.Tolerance).ToLocations(mapView).ToLocationCollection(),
                    Stroke = this.IsEnabled ? this.DoubleToBrushMap.Map(locationCollection[1].Value) : this.DisabledStroke
                }));
            }

            this.SimplifiedPolylineParts = simplifiedLocationCollections;
        }

        protected override void OnChangedLocations()
        {
            base.OnChangedLocations();

            if (this.Locations != null)
            {
                this.CursorLocation = this.Locations.First();
                this.UpdatePolylineParts();
            }

            this.UpdateSimplifiedPolylineParts();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "IsEnabled")
            {
                this.UpdateSimplifiedPolylineParts();
            }
        }

        private void SegmentedPolylineControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdatePolylineParts();
            this.UpdateSimplifiedPolylineParts();
        }

        public event EventHandler<Location> SelectionChanged;
    }
}