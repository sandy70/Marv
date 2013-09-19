using Caching;
using LibNetwork;
using LibPipeline;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace Marv
{
    public partial class MainWindow : Window
    {
        public SensorListener SensorListener = new SensorListener();
        public Dictionary<MultiLocation, MultiLocationValueTimeSeries> ValueTimeSeriesForMultiLocation = new Dictionary<MultiLocation, MultiLocationValueTimeSeries>();
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            var cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MARV");
            MapControl.TileImageLoader.Cache = new ImageFileCache(MapControl.TileImageLoader.DefaultCacheName, cacheDirectory);

            this.MapView.TileLayer = TileLayers.MapBoxSat;
        }

        public static MultiLocationValueTimeSeries ComputeMultiLocationValueTimeSeries(MultiLocation multiLocation)
        {
            Logger.Info("Computing value for line {0}.", multiLocation.Name);

            var vertexKey = "B08";
            var stateKey = "No fail";

            var multiLocationValueTimeSeries = new MultiLocationValueTimeSeries();
            var nCompleted = 0;
            var nLocations = multiLocation.Count;

            foreach (var location in multiLocation)
            {
                var fileName = MainWindow.GetFileNameForModelValue(multiLocation, location);

                try
                {
                    var modelValue = ObjectDataBase.ReadValueSingle<ModelValue>(fileName, x => true);

                    foreach (var year in modelValue.Keys)
                    {
                        var graphValue = modelValue[year];
                        var vertexValue = graphValue[vertexKey];
                        var stateValue = vertexValue[stateKey];

                        if (!multiLocationValueTimeSeries.ContainsKey(year))
                        {
                            multiLocationValueTimeSeries[year] = new MultiLocationValue();
                        }

                        multiLocationValueTimeSeries[year][location.Name] = stateValue;
                    }
                }
                catch (OdbDataNotFoundException exp)
                {
                    Logger.Info("Value not found for location {0}.", location);
                }

                Logger.Info("Completed {0} of {1}", ++nCompleted, nLocations);
            }

            return multiLocationValueTimeSeries;
        }

        public static Task<MultiLocationValueTimeSeries> ComputeMultiLocationValueTimeSeriesAsync(MultiLocation multiLocation)
        {
            return Task.Run(() =>
                {
                    return MainWindow.ComputeMultiLocationValueTimeSeries(multiLocation);
                });
        }

        public static string GetFileNameForModelValue(MultiLocation multiLocation, Location location)
        {
            return Path.Combine("ModelValues", multiLocation.Name, location.Name + ".db");
        }

        public static string GetFileNameForMultiLocationTimeSeries(MultiLocation multiLocation)
        {
            return Path.Combine("MultiLocationValueTimeSeries", multiLocation.Name, "B08.db");
        }

        public void ReadMultiLocationsValue()
        {
            foreach (var multiLocation in this.MultiLocations)
            {
                try
                {
                    this.ValueTimeSeriesForMultiLocation[multiLocation] = ObjectDataBase.ReadValueSingle<MultiLocationValueTimeSeries>(MainWindow.GetFileNameForMultiLocationTimeSeries(multiLocation), x => true);
                }
                catch (OdbDataNotFoundException exp)
                {
                    Logger.Info("Value not found for line {0}.", multiLocation.Name);
                }
            }
        }

        public void UpdateGraphValue()
        {
            if (this.SelectedLocationModelValue != null)
            {
                if (this.SelectedLocationModelValue.ContainsKey(this.SelectedYear))
                {
                    this.SourceGraph.Value = this.SelectedLocationModelValue[this.SelectedYear];
                }
                else
                {
                    this.SourceGraph.SetValueToZero();
                    this.PopupControl.ShowText("Pipeline inactive for this year.");
                }
            }
        }

        public void UpdateMultiLocationsValue()
        {
            foreach (var multiLocation in this.MultiLocations)
            {
                try
                {
                    multiLocation.Value = this.ValueTimeSeriesForMultiLocation[multiLocation][this.SelectedYear];
                }
                catch (KeyNotFoundException exp)
                {
                    Logger.Warn("Value not found for line {0} for year {1}", multiLocation.Name, this.SelectedYear);
                }
            }
        }
    }
}