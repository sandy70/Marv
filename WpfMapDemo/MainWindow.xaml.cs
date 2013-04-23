using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using MapControl;

namespace WpfMapDemo
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty LocationProperty =
        DependencyProperty.Register("Location", typeof(Location), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<Location>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(Location), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();

            List<Location> locations = new List<Location>();
            double lat = 40;
            double lon = -85;
            var random = new Random();
            double scale = 100;

            for (int i = 0; i < 1000; i++)
            {
                lat += random.NextDouble() / scale;
                lon += random.NextDouble() / scale;

                locations.Add(new Location
                {
                    Latitude = lat,
                    Longitude = lon
                });
            }

            this.Locations = locations;

            this.Location = new Location { Latitude = 40, Longitude = -85 };

            this.KeyDown += (o, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F11)
                {
                    this.Location = new Location { Latitude = 40, Longitude = -135 };
                    Console.WriteLine("Key pressed!");
                }
            };

            this.SelectedLocation = this.Locations.First();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public Location Location
        {
            get { return (Location)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
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
    }
}