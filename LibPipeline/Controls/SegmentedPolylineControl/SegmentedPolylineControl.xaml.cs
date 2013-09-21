using Marv.Common;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

        private static void ChangedNameLatitude(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            control.NameLocation = new Location { Latitude = control.NameLatitude, Longitude = control.NameLocation.Longitude };
        }

        public static readonly DependencyProperty NameLocationPointProperty =
        DependencyProperty.Register("NameLocationPoint", typeof(Point), typeof(SegmentedPolylineControl), new PropertyMetadata(new Point(), ChangedNameLocationPoint));

        public static readonly DependencyProperty NameLocationProperty =
        DependencyProperty.Register("NameLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty NameLongitudeProperty =
        DependencyProperty.Register("NameLongitude", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(0.0, ChangedNameLongitude));

        private static void ChangedNameLongitude(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            control.NameLocation = new Location { Latitude = control.NameLocation.Latitude, Longitude = control.NameLongitude };
        }

        public static readonly DependencyProperty SegmentsProperty =
        DependencyProperty.Register("Segments", typeof(ObservableCollection<MultiLocationSegment>), typeof(SegmentedPolylineControl), new PropertyMetadata(new ObservableCollection<MultiLocationSegment>(), ChangedSegments));

        public static readonly DependencyProperty ToleranceProperty =
        DependencyProperty.Register("Tolerance", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ValueMemberPathProperty =
        DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SegmentedPolylineControl), new PropertyMetadata("Value"));

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public SegmentedPolylineControl()
        {
            InitializeComponent();
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

        public ObservableCollection<MultiLocationSegment> Segments
        {
            get { return (ObservableCollection<MultiLocationSegment>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        public double Tolerance
        {
            get { return (double)GetValue(ToleranceProperty); }
            set { SetValue(ToleranceProperty, value); }
        }

        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public void UpdateSegments()
        {
            var mapView = this.FindParent<MapView>();

            IDoubleToBrushMap doubleToBrushMap;
            Brush stroke;

            if (this.IsEnabled)
            {
                doubleToBrushMap = this.DoubleToBrushMap;
                stroke = this.Stroke;
            }
            else
            {
                doubleToBrushMap = null;
                stroke = this.DisabledStroke;
            }

            if (mapView != null && this.Locations != null)
            {
                this.Segments = this.Locations
                                    .Within(mapView.Extent.GetPadded(mapView.Extent.MaxDimension / 4))
                                    .ToViewportPoints(mapView, this.ValueMemberPath)
                                    .Reduce(this.Tolerance)
                                    .ToLocations(mapView)
                                    .ToSegments();

                foreach (var segment in this.Segments)
                {
                    if (this.IsEnabled)
                    {
                        segment.Stroke = this.DoubleToBrushMap.Map(segment.Value);
                    }
                    else
                    {
                        segment.Stroke = this.DisabledStroke;
                    }
                }
            }
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
            }

            this.UpdateSegments();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "IsEnabled")
            {
                this.UpdateSegments();
            }
        }

        private static void ChangedNameLocationPoint(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;
            control.NameLocation = control.NameLocationPoint;
        }

        private static void ChangedSegments(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            var nSegments = control.Segments.Count();

            if (nSegments > 0)
            {
                var oldLocation = control.NameLocation;
                var newLocation = control.Segments.ElementAt(nSegments / 2).Middle;

                if (oldLocation == null)
                {
                    control.NameLocation = newLocation;
                }
                else
                {
                    var xAnimation = new DoubleAnimation
                    {
                        From = oldLocation.X,
                        To = newLocation.X,
                        Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    var yAnimation = new DoubleAnimation
                    {
                        From = oldLocation.Y,
                        To = newLocation.Y,
                        Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    control.BeginAnimation(SegmentedPolylineControl.NameLongitudeProperty, xAnimation);
                    control.BeginAnimation(SegmentedPolylineControl.NameLatitudeProperty, yAnimation);
                }

                // control.NameLocation = control.Segments.ElementAt(nSegments / 2).Middle;
            }
        }

        private void Locations_ValueChanged(object sender, ValueEventArgs<double> e)
        {
            Logger.Trace("");

            this.UpdateSegments();
        }
    }
}