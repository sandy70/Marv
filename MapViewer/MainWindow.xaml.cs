using MapControl;
using System;
using System.Windows;
using System.Windows.Shapes;
using LibPipeline;

namespace MapViewer
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty EarthquakesProperty =
        DependencyProperty.Register("Earthquakes", typeof(MultiLocation), typeof(MainWindow), new PropertyMetadata(null, ChangedEarthquakes));

        private static void ChangedEarthquakes(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindow = d as MainWindow;

            foreach (var location in mainWindow.Earthquakes.Locations)
            {
                Console.WriteLine(location.Latitude + ", " + location.Longitude);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public MultiLocation Earthquakes
        {
            get { return (MultiLocation)GetValue(EarthquakesProperty); }
            set { SetValue(EarthquakesProperty, value); }
        }
    }
}