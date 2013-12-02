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

        public Dictionary<LocationCollection, MultiLocationValueTimeSeries> MultiLocationValueTimeSeriesForMultiLocation = new Dictionary<LocationCollection, MultiLocationValueTimeSeries>();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IGraphValueReader graphValueReader;
        private Dictionary<int, string, string, double> graphValues;

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            var cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MARV");
            MapControl.TileImageLoader.Cache = new ImageFileCache(MapControl.TileImageLoader.DefaultCacheName, cacheDirectory);

            this.MapView.TileLayer = TileLayers.BingMapsAerial;
        }

        public IGraphValueReader GraphValueReader
        {
            get { return graphValueReader; }
            set { graphValueReader = value; }
        }

        public Dictionary<int, string, string, double> GraphValues
        {
            get { return graphValues; }
            set { graphValues = value; }
        }

        public static MultiLocationValueTimeSeries CalculateMultiLocationValueTimeSeriesAndWrite(LocationCollection multiLocation, Graph graph = null)
        {
            logger.Info("Computing value for line {0}.", multiLocation.Name);

            var vertexKey = "B08";
            var vertexName = graph.Vertices[vertexKey].Name;
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
            catch (InvalidOperationException)
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
                            multiLocationValueTimeSeries[year] = new Dictionary<string, double>();
                        }

                        multiLocationValueTimeSeries[year][location.Name] = stateValue;

                        // var extractedValue = graph.GetMean(vertexKey, vertexValue);
                        // var extractedValue = graph.GetStandardDeviation(vertexKey, vertexValue);
                        var extractedValue = stateValue;
                        excelWorkSheet.SetValue(excelRow, excelCol, extractedValue);
                    }
                }
                catch (OdbDataNotFoundException)
                {
                    logger.Info("Value not found for point {0}.", location);
                }

                logger.Info("Completed {0} of {1}", ++nCompleted, nLocations);
            }

            excelPackage.Save();

            var fName = MainWindow.GetFileNameForMultiLocationValueTimeSeries(multiLocation, vertexKey, stateKey);
            Odb.Write<MultiLocationValueTimeSeries>(fName, multiLocationValueTimeSeries);

            return multiLocationValueTimeSeries;
        }

        public static Task<MultiLocationValueTimeSeries> CalculateMultiLocationValueTimeSeriesAndWriteAsync(LocationCollection multiLocation, Graph graph)
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

        public static string GetFileNameForMultiLocationValueTimeSeries(LocationCollection multiLocation, string vertexKey, string stateKey)
        {
            return Path.Combine("MultiLocationValueTimeSeries", multiLocation.Name, vertexKey + "_" + stateKey + ".db");
        }

        public static void RunAndWrite(string networkFileName, string inputFileName, string multiLocationName, string locationName, int startYear, int endYear)
        {
            var graph = Graph.Read(networkFileName);
            var graphEvidence = AdcoInput.GetGraphEvidence(graph, inputFileName, multiLocationName, locationName);

            var graphValueTimeSeries = graph.Run(graphEvidence, startYear, endYear);

            var fileName = MainWindow.GetFileNameForModelValue(multiLocationName, locationName);

            try
            {
                Odb.Write<Dictionary<int, string, string, double>>(fileName, graphValueTimeSeries);
            }
            catch(IOException exp)
            {
                logger.Warn(exp.Message);
            }
        }

        public static Task RunAndWriteAsync(string networkFileName, string inputFileName, string multiLocationName, string locationName, int startYear, int endYear)
        {
            return Task.Run(() =>
                {
                    MainWindow.RunAndWrite(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);
                });
        }

        public void ReadGraphValues()
        {
            var multiLocation = this.Polylines.SelectedItem;
            var location = multiLocation.SelectedItem;

            try
            {
                this.GraphValues = this.GraphValueReader.Read(multiLocation.Name, location.Name);
            }
            catch (GraphValueNotFoundException exception)
            {
                logger.Warn(exception.Message);

                this.Notifications.Push(new NotificationTimed
                {
                    Name = "Value Not Found",
                    Description = exception.Message
                });

                this.GraphValues = null;
            }
        }

        public void ReadMultiLocationValueTimeSeriesForMultiLocation()
        {
            foreach (var multiLocation in this.Polylines)
            {
                try
                {
                    var fileName = MainWindow.GetFileNameForMultiLocationValueTimeSeries(multiLocation, "B08", "Fail");
                    this.MultiLocationValueTimeSeriesForMultiLocation[multiLocation] = Odb.ReadValueSingle<MultiLocationValueTimeSeries>(fileName, x => true);
                }
                catch (OdbDataNotFoundException)
                {
                    logger.Warn("Value not found for line {0}.", multiLocation.Name);
                }
            }
        }

        public void UpdateGraphValue()
        {
            if (this.SourceGraph != null)
            {
                if (this.GraphValues != null)
                {
                    if (this.GraphValues.ContainsKey(this.SelectedYear))
                    {
                        this.SourceGraph.Value = this.GraphValues[this.SelectedYear];
                    }
                    else
                    {
                        this.SourceGraph.SetValueToZero();
                    }
                }
                else
                {
                    this.SourceGraph.SetValueToZero();
                }
            }
        }

        public void UpdateMultiLocationValues()
        {
            foreach (var multiLocation in this.Polylines)
            {
                if ((int)multiLocation.Properties["StartYear"] > this.SelectedYear)
                {
                    multiLocation.IsEnabled = false;
                }
                else
                {
                    multiLocation.IsEnabled = true;

                    if (this.Polylines.SelectedItem.IsEnabled == false)
                    {
                        this.Polylines.SelectedItem = multiLocation;
                    }

                    try
                    {
                        multiLocation.Value = this.MultiLocationValueTimeSeriesForMultiLocation[multiLocation][this.SelectedYear];
                    }
                    catch (KeyNotFoundException)
                    {
                        logger.Warn("Value not found for line {0} for year {1}", multiLocation.Name, this.SelectedYear);
                    }
                }
            }
        }
    }
}