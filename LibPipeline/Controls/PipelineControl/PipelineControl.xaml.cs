using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfMap;

namespace LibPipeline
{
    public partial class PipelineControl : MapPanel
    {
        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<Location>), typeof(PipelineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(Location), typeof(PipelineControl), new PropertyMetadata(null));

        public PipelineControl()
        {
            InitializeComponent();
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
            pipelineControl.SelectedLocation = (e.NewValue as IEnumerable<Location>).First();
        }
    }
}