using Marv.Common;
using System;
using System.Windows;

namespace Marv.Demo.Map
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(ViewModelCollection<Location>), typeof(MainWindow), new PropertyMetadata(null));

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

        public ViewModelCollection<Location> Locations
        {
            get { return (ViewModelCollection<Location>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        private void PolylineControl_SelectionChanged(object sender, Controls.ValueEventArgs<Location> e)
        {
            Console.WriteLine("Selected: " + e.Value);
        }
    }
}