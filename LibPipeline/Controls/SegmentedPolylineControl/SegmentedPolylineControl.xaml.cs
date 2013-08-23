using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public partial class SegmentedPolylineControl : UserControl
    {
        public static readonly DependencyProperty CursorFillProperty =
        DependencyProperty.Register("CursorFill", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.YellowGreen)));

        public static readonly DependencyProperty CursorLocationProperty =
        DependencyProperty.Register("CursorLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CursorStrokeProperty =
        DependencyProperty.Register("CursorStroke", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public static readonly DependencyProperty DisabledStrokeProperty =
        DependencyProperty.Register("DisabledStroke", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty DoubleToBrushMapProperty =
        DependencyProperty.Register("DoubleToBrushMap", typeof(IDoubleToBrushMap), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsCursorVisibleProperty =
        DependencyProperty.Register("IsCursorVisible", typeof(bool), typeof(SegmentedPolylineControl), new PropertyMetadata(true));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(MultiLocation), typeof(SegmentedPolylineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty NameLocationProperty =
        DependencyProperty.Register("NameLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SegmentsProperty =
        DependencyProperty.Register("Segments", typeof(ObservableCollection<MultiLocationSegment>), typeof(SegmentedPolylineControl), new PropertyMetadata(new ObservableCollection<MultiLocationSegment>(), ChangedSegments));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register("Stroke", typeof(Brush), typeof(SegmentedPolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public static readonly DependencyProperty ToleranceProperty =
        DependencyProperty.Register("Tolerance", typeof(double), typeof(SegmentedPolylineControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ValueMemberPathProperty =
        DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SegmentedPolylineControl), new PropertyMetadata("Value"));

        public SegmentedPolylineControl()
        {
            InitializeComponent();
        }

        public Brush CursorFill
        {
            get { return (Brush)GetValue(CursorFillProperty); }
            set { SetValue(CursorFillProperty, value); }
        }

        public Location CursorLocation
        {
            get { return (Location)GetValue(CursorLocationProperty); }
            set { SetValue(CursorLocationProperty, value); }
        }

        public Brush CursorStroke
        {
            get { return (Brush)GetValue(CursorStrokeProperty); }
            set { SetValue(CursorStrokeProperty, value); }
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

        public bool IsCursorVisible
        {
            get { return (bool)GetValue(IsCursorVisibleProperty); }
            set { SetValue(IsCursorVisibleProperty, value); }
        }

        public MultiLocation Locations
        {
            get { return (MultiLocation)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
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

        public Location SelectedLocation
        {
            get { return (Location)GetValue(SelectedLocationProperty); }
            set { SetValue(SelectedLocationProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
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
                                    .ToSegments(doubleToBrushMap, stroke);
            }
        }

        public void UpdateVisual()
        {
            // this.MapPanel.InvalidateVisual();
            // this.MapPanel.UpdateLayout();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "IsEnabled")
            {
                this.UpdateSegments();
            }
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            if (control.Locations != null)
            {
                control.CursorLocation = control.Locations.First();
            }

            control.UpdateSegments();
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