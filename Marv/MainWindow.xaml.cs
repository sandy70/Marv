using Caching;
using Marv.Common;
using NLog;
using OfficeOpenXml;
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
        public static VertexCommand VertexBarChartCommand = new VertexCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Chart.png"
        };

        public static VertexCommand VertexChartCommand = new VertexCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Chart.png"
        };

        public static VertexCommand VertexChartPofCommand = new VertexCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Chart.png"
        };

        public Dictionary<string, Point> graphPositionForGroup = new Dictionary<string, Point>();
        public Dictionary<int, string, string, double> graphValueTimeSeries = new Dictionary<int, string, string, double>();
        public Dictionary<string, double> graphZoomForGroup = new Dictionary<string, double>();
        public Dictionary<MultiLocation, MultiLocationValueTimeSeries> MultiLocationValueTimeSeriesForMultiLocation = new Dictionary<MultiLocation, MultiLocationValueTimeSeries>();
        public string selectedGroup = null;
        public SensorListener SensorListener = new SensorListener();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            var cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MARV");
            MapControl.TileImageLoader.Cache = new ImageFileCache(MapControl.TileImageLoader.DefaultCacheName, cacheDirectory);

            this.MapView.TileLayer = TileLayers.BingMapsAerial;
        }

        public static MultiLocationValueTimeSeries CalculateMultiLocationValueTimeSeriesAndWrite(MultiLocation multiLocation, Graph graph = null)
        {
            logger.Info("Computing value for line {0}.", multiLocation.Name);

            var vertexKey = "B08";
            var vertexName = graph.GetVertex(vertexKey).Name;
            var stateKey = "Fail";
            // var quantity = "Mean";

            var multiLocationValueTimeSeries = new MultiLocationValueTimeSeries();
            var nCompleted = 0;
            var nLocations = multiLocation.Count;

            var excelFileName = Path.Combine("MultiLocationValueTimeSeries", multiLocation.Name + ".xlsx");
            var excelPackage = new ExcelPackage(new FileInfo(excelFileName));
            var excelWorkSheetName = vertexName + "_" + stateKey;
            // var excelWorkSheetName = vertexName + "_" + quantity;

            try
            {
                excelPackage.Workbook.Worksheets.Add(excelWorkSheetName);
            }
            catch (InvalidOperationException exp)
            {
                logger.Warn("The worksheet {0} already exists.", excelWorkSheetName);
            }

            var excelWorkSheet = excelPackage.Workbook.Worksheets[excelWorkSheetName];

            var excelRow = 1;

            foreach (var location in multiLocation)
            {
                var excelCol = 1;

                excelWorkSheet.SetValue(++excelRow, excelCol, location.Name);

                var fileName = MainWindow.GetFileNameForModelValue(multiLocation.Name, location.Name);

                try
                {
                    var modelValue = Odb.ReadValueSingle<Dictionary<int, string, string, double>>(fileName, x => true);

                    foreach (var year in modelValue.Keys)
                    {
                        excelWorkSheet.SetValue(1, ++excelCol, year);

                        var graphValue = modelValue[year];
                        var vertexValue = graphValue[vertexKey];
                        var stateValue = vertexValue[stateKey];

                        if (!multiLocationValueTimeSeries.ContainsKey(year))
                        {
                            multiLocationValueTimeSeries[year] = new MultiLocationValue();
                        }

                        multiLocationValueTimeSeries[year][location.Name] = stateValue;

                        // var extractedValue = graph.GetMean(vertexKey, vertexValue);
                        // var extractedValue = graph.GetStandardDeviation(vertexKey, vertexValue);
                        var extractedValue = stateValue;
                        excelWorkSheet.SetValue(excelRow, excelCol, extractedValue);
                    }
                }
                catch (OdbDataNotFoundException exp)
                {
                    logger.Info("Value not found for location {0}.", location);
                }

                logger.Info("Completed {0} of {1}", ++nCompleted, nLocations);
            }

            excelPackage.Save();

            var fName = MainWindow.GetFileNameForMultiLocationValueTimeSeries(multiLocation, vertexKey, stateKey);
            Odb.Write<MultiLocationValueTimeSeries>(fName, multiLocationValueTimeSeries);

            return multiLocationValueTimeSeries;
        }

        public static Task<MultiLocationValueTimeSeries> CalculateMultiLocationValueTimeSeriesAndWriteAsync(MultiLocation multiLocation, Graph graph)
        {
            return Task.Run(() =>
                {
                    return MainWindow.CalculateMultiLocationValueTimeSeriesAndWrite(multiLocation, graph);
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
            var graph = Graph.Read<Vertex>(networkFileName);
            var graphEvidence = AdcoInput.GetGraphEvidence(graph, inputFileName, multiLocationName, locationName);

            var graphValueTimeSeries = graph.Run(graphEvidence, startYear, endYear);

            var fileName = MainWindow.GetFileNameForModelValue(multiLocationName, locationName);
            Odb.Write<Dictionary<int, string, string, double>>(fileName, graphValueTimeSeries);
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
            logger.Trace("");

            var multiLocation = this.MultiLocations.SelectedItem;
            var location = multiLocation.SelectedItem;

            try
            {
                var fileName = MainWindow.GetFileNameForModelValue(multiLocation.Name, location.Name);
                this.graphValueTimeSeries = Odb.ReadValueSingle<Dictionary<int, string, string, double>>(fileName, x => true);
            }
            catch (OdbDataNotFoundException exp)
            {
                var message = "Value not found for location: " + location.Name + " on line: " + multiLocation.Name;

                logger.Warn(message);

                this.Notifications.Push(new TimedNotification
                {
                    Name = "Value Not Found",
                    Description = message
                });
            }
        }

        public void ReadGraphValueTimeSeriesCnpc()
        {
            foreach (var fileName in Directory.GetFiles(this.InputDir, "*.vertices"))
            {
                var year = Int32.Parse(Path.GetFileNameWithoutExtension(fileName));
                this.graphValueTimeSeries[year] = this.SourceGraph.ReadValueCsv(fileName);
            }
        }

        public void ReadMultiLocationValueTimeSeriesForMultiLocation()
        {
            foreach (var multiLocation in this.MultiLocations)
            {
                try
                {
                    var fileName = MainWindow.GetFileNameForMultiLocationValueTimeSeries(multiLocation, "B08", "Fail");
                    this.MultiLocationValueTimeSeriesForMultiLocation[multiLocation] = Odb.ReadValueSingle<MultiLocationValueTimeSeries>(fileName, x => true);
                }
                catch (OdbDataNotFoundException exp)
                {
                    logger.Info("Value not found for line {0}.", multiLocation.Name);
                }
            }
        }

        public void UpdateGraphValue()
        {
            if (this.graphValueTimeSeries != null)
            {
                if (this.graphValueTimeSeries.ContainsKey(this.SelectedYear))
                {
                    this.SourceGraph.Value = this.graphValueTimeSeries[this.SelectedYear];
                }
                else
                {
                    this.SourceGraph.SetValueToZero();

                    this.Notifications.Push(new TimedNotification
                    {
                        Name = "Value Not Found",
                        Description = "Pipeline is inactive for this year."
                    });
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
                        logger.Warn("Value not found for line {0} for year {1}", multiLocation.Name, this.SelectedYear);
                    }
                }
            }
        }
    }
}