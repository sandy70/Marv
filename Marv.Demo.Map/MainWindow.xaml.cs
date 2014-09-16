using System;
using System.Collections.Generic;
using System.Windows;
using Marv.Map;

namespace Marv.Demo.Map
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty LocationsProperty =
            DependencyProperty.Register("Locations", typeof (IEnumerable<Location>), typeof (MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();

            var locations = new LocationCollection();
            locations.Add(new Location {Latitude = 49, Longitude = -83});
            locations.Add(new Location {Latitude = 49.1, Longitude = -83.05});
            locations.Add(new Location {Latitude = 49.5, Longitude = -83.8});
            locations.Add(new Location {Latitude = 49.7, Longitude = -83.85});
            locations.Add(new Location {Latitude = 49.9, Longitude = -83.97});
            this.Locations = locations;

            this.PolylineControl.SelectionChanged += (o, e) => Console.WriteLine("Selection changed to: " + e.Value);
        }

        public IEnumerable<Location> Locations
        {
            get
            {
                return (IEnumerable<Location>) GetValue(LocationsProperty);
            }
            set
            {
                SetValue(LocationsProperty, value);
            }
        }
    }
}