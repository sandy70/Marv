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
        public Dictionary<MultiLocation, MultiLocationValueTimeSeries> MultiLocationValueTimeSeriesForMultiLocation = new Dictionary<MultiLocation, MultiLocationValueTimeSeries>();
        public SensorListener SensorListener = new SensorListener();
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            var cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MARV");
            MapControl.TileImageLoader.Cache = new ImageFileCache(MapControl.TileImageLoader.DefaultCacheName, cacheDirectory);

            this.MapView.TileLayer = TileLayers.BingMapsAerial;
        }

        public static MultiLocationValueTimeSeries CalculateMultiLocationValueTimeSeriesAndWrite(MultiLocation multiLocation)
        {
            Logger.Info("Computing value for line {0}.", multiLocation.Name);

            var vertexKey = "B08";
            var stateKey = "Fail";

            var multiLocationValueTimeSeries = new MultiLocationValueTimeSeries();
            var nCompleted = 0;
            var nLocations = multiLocation.Count;

            foreach (var location in multiLocation)
            {
                var fileName = MainWindow.GetFileNameForModelValue(multiLocation.Name, location.Name);

                try
                {
                    var modelValue = ObjectDataBase.ReadValueSingle<BnGraphValueTimeSeries>(fileName, x => true);

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

            var fName = MainWindow.GetFileNameForMultiLocationValueTimeSeries(multiLocation, vertexKey, stateKey);
            ObjectDataBase.Write<MultiLocationValueTimeSeries>(fName, multiLocationValueTimeSeries);

            return multiLocationValueTimeSeries;
        }

        public static Task<MultiLocationValueTimeSeries> CalculateMultiLocationValueTimeSeriesAndWriteAsync(MultiLocation multiLocation)
        {
            return Task.Run(() =>
                {
                    return MainWindow.CalculateMultiLocationValueTimeSeriesAndWrite(multiLocation);
                });
        }

        public static string GetFileNameForModelValue(string multiLocationName, string locationName)
        {
            return Path.Combine("ModelValues", multiLocationName, locationName + ".db");
        }

        public static string GetFileNameForMultiLocationValueTimeSeries(MultiLocation multiLocation, string vertexKey, string stateKey)
        {
            return Path.Combine("MultiLocationValueTimeSeries", multiLocation.Name, vertexKey + "_" + stateKey + ".db");
        }

        public static void RunAndWrite(string networkFileName, string inputFileName, string multiLocationName, string locationName, int startYear, int endYear)
        {
            var graph = BnGraph.Read<BnVertexViewModel>(networkFileName);
            var graphEvidence = AdcoInput.GetGraphEvidence(graph, inputFileName, multiLocationName, locationName);

            var graphValueTimeSeries = graph.Run(graphEvidence, startYear, endYear);

            var fileName = MainWindow.GetFileNameForModelValue(multiLocationName, locationName);
            ObjectDataBase.Write<BnGraphValueTimeSeries>(fileName, graphValueTimeSeries);
        }

        public static Task RunAndWriteAsync(string networkFileName, string inputFileName, string multiLocationName, string locationName, int startYear, int endYear)
        {
            return Task.Run(() =>
                {
                    MainWindow.RunAndWrite(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);
                });
        }

        public void ReadGraphValueTimeSeries()
        {
            Logger.Trace("");

            var multiLocation = this.MultiLocations.SelectedItem;
            var location = multiLocation.SelectedItem;

            try
            {
                var fileName = MainWindow.GetFileNameForModelValue(multiLocation.Name, location.Name);
                this.GraphValueTimeSeries = ObjectDataBase.ReadValueSingle<BnGraphValueTimeSeries>(fileName, x => true);
            }
            catch (OdbDataNotFoundException exp)
            {
                Logger.Warn("Value not found for location {0} on line {1}", location.Name, multiLocation.Name);

                this.PopupControl.ShowText("Value not found for this location. Run model first.");
            }
        }

        public void ReadMultiLocationValueTimeSeriesForMultiLocation()
        {
            foreach (var multiLocation in this.MultiLocations)
            {
                try
                {
                    var fileName = MainWindow.GetFileNameForMultiLocationValueTimeSeries(multiLocation, "B08", "Fail");
                    this.MultiLocationValueTimeSeriesForMultiLocation[multiLocation] = ObjectDataBase.ReadValueSingle<MultiLocationValueTimeSeries>(fileName, x => true);
                }
                catch (OdbDataNotFoundException exp)
                {
                    Logger.Info("Value not found for line {0}.", multiLocation.Name);
                }
            }
        }

        public void UpdateGraphValue()
        {
            if (this.GraphValueTimeSeries != null)
            {
                if (this.GraphValueTimeSeries.ContainsKey(this.SelectedYear))
                {
                    this.SourceGraph.Value = this.GraphValueTimeSeries[this.SelectedYear];
                }
                else
                {
                    this.SourceGraph.SetValueToZero();
                    this.PopupControl.ShowText("Pipeline inactive for this year.");
                }
            }
        }

        public void UpdateMultiLocationValues()
        {
            foreach (var multiLocation in this.MultiLocations)
            {
                if ((int)multiLocation["StartYear"] > this.SelectedYear)
                {
                    multiLocation.IsEnabled = false;
                }
                else
                {
                    multiLocation.IsEnabled = true;

                    if (this.MultiLocations.SelectedItem.IsEnabled == false)
                    {
                        this.MultiLocations.SelectedItem = multiLocation;
                    }

                    try
                    {
                        multiLocation.Value = this.MultiLocationValueTimeSeriesForMultiLocation[multiLocation][this.SelectedYear];
                    }
                    catch (KeyNotFoundException exp)
                    {
                        Logger.Warn("Value not found for line {0} for year {1}", multiLocation.Name, this.SelectedYear);
                    }
                }
            }
        }
    }
}