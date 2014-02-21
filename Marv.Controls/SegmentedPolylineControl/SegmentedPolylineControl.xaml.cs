using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Marv.Common;
using Marv.Common.Map;
using NLog;
using Utils = Marv.Common.Map.Utils;

namespace Marv.Controls
{
    public partial class SegmentedPolylineControl
    {
        public Dictionary<LocationCollection, Brush> Brushes
        {
            get { return (Dictionary<LocationCollection, Brush>)GetValue(BrushesProperty); }
            set { SetValue(BrushesProperty, value); }
        }

        public static readonly DependencyProperty BrushesProperty =
        DependencyProperty.Register("Brushes", typeof(Dictionary<LocationCollection, Brush>), typeof(SegmentedPolylineControl), new PropertyMetadata(new Dictionary<LocationCollection, Brush>()));

        public static readonly DependencyProperty DisabledStrokeProperty =
        DependencyProperty.Register("DisabledStroke", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty DoubleToBrushMapProperty =
        DependencyProperty.Register("DoubleToBrushMap", typeof(IDoubleToBrushMap), typeof(SegmentedPolylineControl), new PropertyMetadata(new DoubleToBrushMap()));

        public static readonly DependencyProperty NameLatitudeProperty =
        DependencyProperty.Register("NameLatitude", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(0.0, ChangedNameLatitude));

        public static readonly DependencyProperty NameLocationPointProperty =
        DependencyProperty.Register("NameLocationPoint", typeof(Point), typeof(SegmentedPolylineControl), new PropertyMetadata(new Point(), ChangedNameLocationPoint));

        public static readonly DependencyProperty NameLocationProperty =
        DependencyProperty.Register("NameLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty NameLongitudeProperty =
        DependencyProperty.Register("NameLongitude", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(0.0, ChangedNameLongitude));

        public static readonly DependencyProperty PolylinePartsProperty =
        DependencyProperty.Register("PolylineParts", typeof(IEnumerable<LocationCollection>), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SimplifiedPolylinePartsProperty =
        DependencyProperty.Register("SimplifiedPolylineParts", typeof(IEnumerable<LocationCollection>), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ToleranceProperty =
        DependencyProperty.Register("Tolerance", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ValueLevelsProperty =
        DependencyProperty.Register("ValueLevels", typeof(Sequence<double>), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueMemberPathProperty =
        DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SegmentedPolylineControl), new PropertyMetadata("Value"));

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SegmentedPolylineControl()
        {
            this.InitializeComponent();
            this.ValueLevels = new Sequence<double> { 0.00, 0.25, 0.5, 0.75, 1.00 };
        }

        public Brush DisabledStroke
        {
            get { return (Brush)this.GetValue(DisabledStrokeProperty); }
            set { this.SetValue(DisabledStrokeProperty, value); }
        }
        public IDoubleToBrushMap DoubleToBrushMap
        {
            get { return (IDoubleToBrushMap)this.GetValue(DoubleToBrushMapProperty); }
            set { this.SetValue(DoubleToBrushMapProperty, value); }
        }

        public double NameLatitude
        {
            get { return (double)this.GetValue(NameLatitudeProperty); }
            set { this.SetValue(NameLatitudeProperty, value); }
        }

        public Location NameLocation
        {
            get { return (Location)this.GetValue(NameLocationProperty); }
            set { this.SetValue(NameLocationProperty, value); }
        }

        public Point NameLocationPoint
        {
            get { return (Point)this.GetValue(NameLocationPointProperty); }
            set { this.SetValue(NameLocationPointProperty, value); }
        }

        public double NameLongitude
        {
            get { return (double)this.GetValue(NameLongitudeProperty); }
            set { this.SetValue(NameLongitudeProperty, value); }
        }

        public IEnumerable<LocationCollection> PolylineParts
        {
            get { return (IEnumerable<LocationCollection>)this.GetValue(PolylinePartsProperty); }
            set { this.SetValue(PolylinePartsProperty, value); }
        }

        public IEnumerable<LocationCollection> SimplifiedPolylineParts
        {
            get { return (IEnumerable<LocationCollection>)this.GetValue(SimplifiedPolylinePartsProperty); }
            set { this.SetValue(SimplifiedPolylinePartsProperty, value); }
        }

        public double Tolerance
        {
            get { return (double)this.GetValue(ToleranceProperty); }
            set { this.SetValue(ToleranceProperty, value); }
        }

        public Sequence<double> ValueLevels
        {
            get { return (Sequence<double>)this.GetValue(ValueLevelsProperty); }
            set { this.SetValue(ValueLevelsProperty, value); }
        }

        public string ValueMemberPath
        {
            get { return (string)this.GetValue(ValueMemberPathProperty); }
            set { this.SetValue(ValueMemberPathProperty, value); }
        }

        public void UpdatePolylineParts()
        {
            
            var oldBinIndex = -1;
            var multiLocationParts = new List<LocationCollection>();

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
                        var mid = Utils.Mid(locationCollection.Last(), location);

                        locationCollection.Add(mid);

                        locationCollection = new LocationCollection();
                        locationCollection.Add(mid);
                        locationCollection.Add(location);
                    }

                    multiLocationParts.Add(locationCollection);
                }
                else
                {
                    locationCollection.Add(location);
                }

                oldBinIndex = newBinIndex;
            }

            this.PolylineParts = multiLocationParts;
        }

        public void UpdateSimplifiedPolylineParts()
        {
            var mapView = this.FindParent<MapView>();
            var simplifiedLocationCollections = new List<LocationCollectionViewModel>();

            if (this.PolylineParts != null)
            {
                foreach (var locationCollection in this.PolylineParts)
                {
                    var simplifiedLocationCollection = new LocationCollectionViewModel(locationCollection.ToPoints(mapView)
                                                                                      .Reduce(this.Tolerance)
                                                                                      .ToLocations(mapView));

                    if (this.IsEnabled)
                    {
                        simplifiedLocationCollection.Stroke = this.DoubleToBrushMap.Map(locationCollection[1].Value);
                    }
                    else
                    {
                        simplifiedLocationCollection.Stroke = this.DisabledStroke;
                    }

                    simplifiedLocationCollections.Add(simplifiedLocationCollection);
                }
            }

            this.SimplifiedPolylineParts = simplifiedLocationCollections;
        }

        public void UpdateVisual()
        {
            // this.MapPanel.InvalidateVisual();
            // this.MapPanel.UpdateLayout();
        }

        protected override void OnChangedLocations()
        {
            base.OnChangedLocations();

            if (this.Locations != null)
            {
                this.CursorLocation = this.Locations.First();
                this.Locations.ValueChanged += this.Locations_ValueChanged;
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

        private static void ChangedNameLatitude(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            control.NameLocation = new Location { Latitude = control.NameLatitude, Longitude = control.NameLocation.Longitude };
        }

        private static void ChangedNameLocationPoint(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            // control.NameLocation = control.NameLocationPoint;
        }

        private static void ChangedNameLongitude(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            control.NameLocation = new Location { Latitude = control.NameLocation.Latitude, Longitude = control.NameLongitude };
        }

        private void Locations_ValueChanged(object sender, EventArgs e)
        {
            logger.Trace("");

            this.UpdatePolylineParts();
            this.UpdateSimplifiedPolylineParts();
        }
    }
}