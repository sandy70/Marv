using Caching;
using LibNetwork;
using LibPipeline;
using Smile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace Marv
{
    public partial class MainWindow : Window
    {
        public SensorListener SensorListener = new SensorListener();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            var cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MARV");
            MapControl.TileImageLoader.Cache = new ImageFileCache(MapControl.TileImageLoader.DefaultCacheName, cacheDirectory);

            this.MapView.TileLayer = TileLayers.MapBoxTerrain;
        }
    }
}