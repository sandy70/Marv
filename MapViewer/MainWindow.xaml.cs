using System.Windows;
using LibPipeline;
using System;

namespace MapViewer
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty LevelProperty =
        DependencyProperty.Register("Level", typeof(double), typeof(MainWindow), new PropertyMetadata(1.0));

        public MainWindow()
        {
            InitializeComponent();

            int pixelX; int pixelY;
            TileSystem.LatLongToPixelXY(22.59, 54.89, 8, out pixelX, out pixelY);

            Console.WriteLine("X: " + pixelX + ", Y: " + pixelY);
        }

        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }
    }
}