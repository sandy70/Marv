using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;

namespace LibPipeline
{
    public partial class SegmentedPolylineControl : PolylineControlBase
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty DisabledStrokeProperty =
        DependencyProperty.Register("DisabledStroke", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty DoubleToBrushMapProperty =
        DependencyProperty.Register("DoubleToBrushMap", typeof(IDoubleToBrushMap), typeof(SegmentedPolylineControl), new PropertyMetadata(new DoubleToBrushMap()));

        public static readonly DependencyProperty NameLocationProperty =
        DependencyProperty.Register("NameLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SegmentsProperty =
        DependencyProperty.Register("Segments", typeof(ObservableCollection<MultiLocationSegment>), typeof(SegmentedPolylineControl), new PropertyMetadata(new ObservableCollection<MultiLocationSegment>(), ChangedSegments));

        public static readonly DependencyProperty ToleranceProperty =
        DependencyProperty.Register("Tolerance", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ValueMemberPathProperty =
        DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SegmentedPolylineControl), new PropertyMetadata("Value"));

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

        public Location NameLocation
        {
            get { return (Location)GetValue(NameLocationProperty); }
            set { SetValue(NameLocationProperty, value); }
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
                    segment.Stroke = this.DoubleToBrushMap.Map(segment.Value);
                }
            }
        }

        public void UpdateVisual()
        {
            this.MapPanel.InvalidateVisual();
            this.MapPanel.UpdateLayout();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "IsEnabled")
            {
                this.UpdateSegments();
            }
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

        private void Locations_ValueChanged(object sender, ValueEventArgs<double> e)
        {
            Logger.Trace(""):
        }

        private static void ChangedSegments(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            var nSegments = control.Segments.Count();

            if (nSegments > 0)
            {
                control.NameLocation = control.Segments.ElementAt(nSegments / 2).Middle;
            }
        }
    }
}