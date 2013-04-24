using MapControl;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LibPipeline
{
    public partial class PipelineControl : MapPanel
    {
        public static readonly DependencyProperty CursorLocationProperty =
        DependencyProperty.Register("CursorLocation", typeof(Location), typeof(PipelineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<Location>), typeof(PipelineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(Location), typeof(PipelineControl), new PropertyMetadata(null, ChangedLocation));

        private static void ChangedLocation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipelineControl = d as PipelineControl;
            pipelineControl.CursorLocation = pipelineControl.SelectedLocation;
        }

        public PipelineControl()
        {
            InitializeComponent();
        }

        public Location CursorLocation
        {
            get { return (Location)GetValue(CursorLocationProperty); }
            set { SetValue(CursorLocationProperty, value); }
        }

        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        public Location SelectedLocation
        {
            get { return (Location)GetValue(SelectedLocationProperty); }
            set { SetValue(SelectedLocationProperty, value); }
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipelineControl = d as PipelineControl;
            pipelineControl.SelectedLocation = (e.NewValue as IEnumerable<Location>).FirstOrDefault();
        }
    }
}