using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public partial class PolylineControl : UserControl
    {
        public static readonly DependencyProperty CursorFillProperty =
        DependencyProperty.Register("CursorFill", typeof(Brush), typeof(PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.YellowGreen)));

        public static readonly DependencyProperty CursorLocationProperty =
        DependencyProperty.Register("CursorLocation", typeof(ILocation), typeof(PolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CursorStrokeProperty =
        DependencyProperty.Register("CursorStroke", typeof(Brush), typeof(PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public static readonly DependencyProperty IsCursorVisibleProperty =
        DependencyProperty.Register("IsCursorVisible", typeof(bool), typeof(PolylineControl), new PropertyMetadata(true));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<ILocation>), typeof(PolylineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(ILocation), typeof(PolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SimplifiedLocationsProperty =
        DependencyProperty.Register("SimplifiedLocations", typeof(IEnumerable<ILocation>), typeof(PolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register("Stroke", typeof(Brush), typeof(PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public PolylineControl()
        {
            InitializeComponent();
        }

        public Brush CursorFill
        {
            get { return (Brush)GetValue(CursorFillProperty); }
            set { SetValue(CursorFillProperty, value); }
        }

        public ILocation CursorLocation
        {
            get { return (ILocation)GetValue(CursorLocationProperty); }
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

        public IEnumerable<ILocation> Locations
        {
            get { return (IEnumerable<ILocation>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        public ILocation SelectedLocation
        {
            get { return (ILocation)GetValue(SelectedLocationProperty); }
            set { SetValue(SelectedLocationProperty, value); }
        }

        public IEnumerable<ILocation> SimplifiedLocations
        {
            get { return (IEnumerable<ILocation>)GetValue(SimplifiedLocationsProperty); }
            set { SetValue(SimplifiedLocationsProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipelineControl = d as PolylineControl;
            pipelineControl.SimplifiedLocations = pipelineControl.Locations;
            pipelineControl.SelectedLocation = pipelineControl.Locations.FirstOrDefault();
            pipelineControl.CursorLocation = pipelineControl.SelectedLocation;
        }
    }
}