using LibPipeline;
using MapControl;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows.Interactivity;
using ImpromptuInterface;

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
            //this.AssociatedObject.OpenMenuItem.Click += OpenMenuItem_Click;

            //this.AssociatedObject.BingHybridMenuItem.Click += BingHybridMenuItem_Click;
            //this.AssociatedObject.BingRoadsMenuItem.Click += BingRoadsMenuItem_Click;
            //this.AssociatedObject.OsmRoadsMenuItem.Click += OsmRoadsMenuItem_Click;

            //EarthquakeService.GetRecentEarthquakes((o, args) =>
            //    {
            //        this.AssociatedObject.Earthquakes = args.MultiLocation;
            //    });

            this.AssociatedObject.Locations = MultiLocationReader.ReadExcel(@"D:\Data\Koc\Tally.xlsx", "Sheet1").AllActLike<ILocation>();

            foreach (var location in this.AssociatedObject.Locations)
            {
                Console.WriteLine(location.Latitude + ", " + location.Longitude);
            }
        }

        private void OpenMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel 2007/2010 Files (*.xlsx)|*.xlsx|Excel 97-2000 Files (*.xls)|*.xls",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    //this.AssociatedObject.MultiLocations.Add(MultiLocationReader.ReadExcel(fileName, "Sheet1"));
                }
            }
        }

        private void BingHybridMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.AssociatedObject.Map.TileLayer = TileLayer.BingHybrid;
        }

        private void BingRoadsMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.AssociatedObject.Map.TileLayer = TileLayer.BingRoads;
        }

        private void OsmRoadsMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.AssociatedObject.Map.TileLayer = TileLayer.OsmRoads;
        }
    }
}