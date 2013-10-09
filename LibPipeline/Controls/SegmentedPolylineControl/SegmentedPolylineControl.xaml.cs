using Marv.Common;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LibPipeline
{
    public partial class SegmentedPolylineControl : PolylineControlBase
    {
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
        DependencyProperty.Register("PolylineParts", typeof(IEnumerable<MultiLocation>), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SimplifiedPolylinePartsProperty =
        DependencyProperty.Register("SimplifiedPolylineParts", typeof(IEnumerable<MultiLocation>), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ToleranceProperty =
        DependencyProperty.Register("Tolerance", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ValueLevelsProperty =
        DependencyProperty.Register("ValueLevels", typeof(SortedSequence<double>), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueMemberPathProperty =
        DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SegmentedPolylineControl), new PropertyMetadata("Value"));

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SegmentedPolylineControl()
        {
            InitializeComponent();
            this.ValueLevels = new SortedSequence<double> { 0.00, 0.25, 0.5, 0.75, 1.00 };
        }

        public Brush DisabledStroke
        {
            get { return (Brush)GetValue(DisabledStrokeProperty); }
            set { SetValue(DisabledStrokeProperty, value); }
        }

        public IDoubleToBrushMap DoubleToBrushMap
        {
            get { return (IDoubleToBrushMap)GetValue(DoubleToBrushMapProperty); }
            set { SetValue(DoubleToBrushMapProperty, value); }
        }

        public double NameLatitude
        {
            get { return (double)GetValue(NameLatitudeProperty); }
            set { SetValue(NameLatitudeProperty, value); }
        }

        public Location NameLocation
        {
            get { return (Location)GetValue(NameLocationProperty); }
            set { SetValue(NameLocationProperty, value); }
        }

        public Point NameLocationPoint
        {
            get { return (Point)GetValue(NameLocationPointProperty); }
            set { SetValue(NameLocationPointProperty, value); }
        }

        public double NameLongitude
        {
            get { return (double)GetValue(NameLongitudeProperty); }
            set { SetValue(NameLongitudeProperty, value); }
        }

        public IEnumerable<MultiLocation> PolylineParts
        {
            get { return (IEnumerable<MultiLocation>)GetValue(PolylinePartsProperty); }
            set { SetValue(PolylinePartsProperty, value); }
        }

        public IEnumerable<MultiLocation> SimplifiedPolylineParts
        {
            get { return (IEnumerable<MultiLocation>)GetValue(SimplifiedPolylinePartsProperty); }
            set { SetValue(SimplifiedPolylinePartsProperty, value); }
        }

        public double Tolerance
        {
            get { return (double)GetValue(ToleranceProperty); }
            set { SetValue(ToleranceProperty, value); }
        }

        public SortedSequence<double> ValueLevels
        {
            get { return (SortedSequence<double>)GetValue(ValueLevelsProperty); }
            set { SetValue(ValueLevelsProperty, value); }
        }

        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public void UpdatePolylineParts()
        {
            var oldBinIndex = -1;
            var multiLocationParts = new List<MultiLocation>();

            MultiLocation multiLocation = null;

            foreach (var location in this.Locations)
            {
                var newBinIndex = this.ValueLevels.GetBinIndex(location.Value);

                if (newBinIndex != oldBinIndex)
                {
                    if (multiLocation == null)
                    {
                        multiLocation = new MultiLocation();
                        multiLocation.Add(location);
                        multiLocation.Stroke = this.DoubleToBrushMap.Map(location.Value);
                    }
                    else
                    {
                        var mid = Utils.Mid(multiLocation.Last(), location);

                        multiLocation.Add(mid);

                        multiLocation = new MultiLocation();
                        multiLocation.Add(mid);
                        multiLocation.Add(location);
                        multiLocation.Stroke = this.DoubleToBrushMap.Map(location.Value);
                    }

                    multiLocationParts.Add(multiLocation);
                }
                else
                {
                    multiLocation.Add(location);
                }

                oldBinIndex = newBinIndex;
            }

            if (this.Locations.Name == "BU-384")
            {
                logger.Debug("We will break here.");
            }

            this.PolylineParts = multiLocationParts;
        }

        public void UpdateSimplifiedPolylineParts()
        {
            var mapView = this.FindParent<MapView>();

            var multiLocations = new List<MultiLocation>();

            if (this.PolylineParts != null)
            {
                foreach (var multiLocation in this.PolylineParts)
                {
                    var simplifiedMultiLocation = new MultiLocation(multiLocation.Reduce(mapView, this.Tolerance));

                    if (this.IsEnabled)
                    {
                        simplifiedMultiLocation.Stroke = this.DoubleToBrushMap.Map(multiLocation[1].Value);
                    }
                    else
                    {
                        simplifiedMultiLocation.Stroke = this.DisabledStroke;
                    }

                    multiLocations.Add(simplifiedMultiLocation);
                }
            }

            this.SimplifiedPolylineParts = multiLocations;
        }

        public void UpdateVisual()
        {
            this.MapPanel.InvalidateVisual();
            this.MapPanel.UpdateLayout();
        }

        protected override void OnChangedLocations()
        {
            base.OnChangedLocations();

            if (this.Locations != null)
            {
                this.CursorLocation = this.Locations.First();
                this.Locations.ValueChanged += Locations_ValueChanged;
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
            control.NameLocation = control.NameLocationPoint;
        }

        private static void ChangedNameLongitude(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            control.NameLocation = new Location { Latitude = control.NameLocation.Latitude, Longitude = control.NameLongitude };
        }

        private void Locations_ValueChanged(object sender, ValueEventArgs<double> e)
        {
            logger.Trace("");

            this.UpdatePolylineParts();
            this.UpdateSimplifiedPolylineParts();
        }
    }
}