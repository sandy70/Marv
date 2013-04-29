using LibPipeline;
using MapControl;
using System.Windows.Interactivity;

namespace MapViewer
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.BingHybridMenuItem.Click += BingHybridMenuItem_Click;
            this.AssociatedObject.BingRoadsMenuItem.Click += BingRoadsMenuItem_Click;
            this.AssociatedObject.OsmRoadsMenuItem.Click += OsmRoadsMenuItem_Click;

            EarthquakeService.GetRecentEarthquakes((o, args) =>
                {
                    this.AssociatedObject.Earthquakes = args.MultiLocation;
                });
        }

        private void BingHybridMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.Map.TileLayer = TileLayer.BingHybrid;
        }

        private void BingRoadsMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.Map.TileLayer = TileLayer.BingRoads;
        }

        private void OsmRoadsMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.Map.TileLayer = TileLayer.OsmRoads;
        }
    }
}