using LibPipeline;
using System;
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
            EarthquakeService.GetRecentEarthquakes((o, args) =>
                {
                    this.AssociatedObject.Earthquakes = args.MultiLocation;
                });
        }
    }
}