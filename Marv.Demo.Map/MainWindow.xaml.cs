using System;
using System.Windows;
using Marv.Common.Map;

namespace Marv.Demo.Map
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var locations = new LocationCollection();
            locations.Add(new Location { Latitude = 49, Longitude = -83 });
            locations.Add(new Location { Latitude = 49.1, Longitude = -83.05 });
            locations.Add(new Location { Latitude = 49.5, Longitude = -83.8 });
            locations.Add(new Location { Latitude = 49.7, Longitude = -83.85 });
            locations.Add(new Location { Latitude = 49.9, Longitude = -83.97 });

            this.PolylineControl.DataContext = locations;

            this.PolylineControl.SelectionChanged += PolylineControl_SelectionChanged;
        }

        private void PolylineControl_SelectionChanged(object sender, Controls.ValueEventArgs<Location> e)
        {
            Console.WriteLine("Selected: " + e.Value);
        }
    }
}