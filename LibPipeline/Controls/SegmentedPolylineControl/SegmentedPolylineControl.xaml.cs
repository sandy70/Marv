using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public partial class SegmentedPolylineControl : UserControl
    {
        private Task<IEnumerable<IPoint>> reductionTask = null;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static readonly DependencyProperty CursorFillProperty =
        DependencyProperty.Register("CursorFill", typeof(Brush), typeof(PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.YellowGreen)));

        public static readonly DependencyProperty CursorLocationProperty =
        DependencyProperty.Register("CursorLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CursorStrokeProperty =
        DependencyProperty.Register("CursorStroke", typeof(Brush), typeof(PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public static readonly DependencyProperty IsCursorVisibleProperty =
        DependencyProperty.Register("IsCursorVisible", typeof(bool), typeof(PolylineControl), new PropertyMetadata(true));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(MultiLocation), typeof(SegmentedPolylineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty SegmentsProperty =
        DependencyProperty.Register("Segments", typeof(ObservableCollection<MultiLocationSegment>), typeof(SegmentedPolylineControl), new PropertyMetadata(new ObservableCollection<MultiLocationSegment>()));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(Location), typeof(SegmentedPolylineControl), new PropertyMetadata(null));

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

            if (mapView != null && this.Locations != null)
            {
                this.Segments = this.Locations
                                    .Within(mapView.Extent.GetPadded(mapView.Extent.MaxDimension * 2))
                                    .ToViewportPoints(mapView, this.ValueMemberPath)
                                    .Reduce(this.Tolerance, 0.1)
                                    .ToLocations(mapView)
                                    .ToSegments();
            }
        }

        public async Task UpdateSegmentsAsync()
        {
            var mapView = this.FindParent<MapView>();

            if (mapView != null && this.Locations != null)
            {
                if (this.reductionTask == null)
                {
                    Console.WriteLine("No task running.");
                }
                else if(this.reductionTask.Status == TaskStatus.Running)
                {
                    Console.WriteLine("Task running.");
                    this.tokenSource.Cancel();
                    this.reductionTask.Wait();
                }

                this.tokenSource = new CancellationTokenSource();

                this.reductionTask = this.Locations
                                         .ToViewportPoints(mapView, this.ValueMemberPath)
                                         .ReduceAsync(this.Tolerance, 5, this.tokenSource.Token);

                var reducedPoints = await this.reductionTask;

                this.Segments = reducedPoints.ToLocations(mapView)
                                             .Within(mapView.Extent.GetPadded(mapView.Extent.MaxDimension))
                                             .ToSegments();
            }
        }

        public void UpdateVisual()
        {
            this.MapPanel.InvalidateVisual();
            this.MapPanel.UpdateLayout();
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SegmentedPolylineControl).UpdateSegments();
        }
    }
}