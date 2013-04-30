using MapControl;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace MapViewer
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty EarthquakesProperty =
        DependencyProperty.Register("Earthquakes", typeof(MultiLocation), typeof(MainWindow), new PropertyMetadata(null, ChangedEarthquakes));

        public static readonly DependencyProperty MultiLocationProperty =
        DependencyProperty.Register("MultiLocation", typeof(MultiLocation), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty MultiLocationsProperty =
        DependencyProperty.Register("MultiLocations", typeof(ObservableCollection<MultiLocation>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<MultiLocation>(), ChangedMultiLocations));

        public static readonly DependencyProperty SelectedMultiLocationProperty =
        DependencyProperty.Register("SelectedMultiLocation", typeof(MultiLocation), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
        }

        public MultiLocation Earthquakes
        {
            get { return (MultiLocation)GetValue(EarthquakesProperty); }
            set { SetValue(EarthquakesProperty, value); }
        }

        public MultiLocation MultiLocation
        {
            get { return (MultiLocation)GetValue(MultiLocationProperty); }
            set { SetValue(MultiLocationProperty, value); }
        }

        public ObservableCollection<MultiLocation> MultiLocations
        {
            get { return (ObservableCollection<MultiLocation>)GetValue(MultiLocationsProperty); }
            set { SetValue(MultiLocationsProperty, value); }
        }

        public MultiLocation SelectedMultiLocation
        {
            get { return (MultiLocation)GetValue(SelectedMultiLocationProperty); }
            set { SetValue(SelectedMultiLocationProperty, value); }
        }

        private static void ChangedEarthquakes(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindow = d as MainWindow;

            foreach (var location in mainWindow.Earthquakes.Locations)
            {
                Console.WriteLine(location.Latitude + ", " + location.Longitude);
            }
        }

        private static void ChangedMultiLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;
            Console.WriteLine("Count: " + window.MultiLocations.Count);
        }
    }
}