using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Caching;
using MapControl;
using Marv;
using Marv.Controls.Map;
using NLog;
using OfficeOpenXml;
using Telerik.Windows.Controls;
using LocationCollection = Marv.Map.LocationCollection;

namespace Marv
{
    public partial class MainWindow
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

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public Dictionary<LocationCollection, Dict<int, string, double>> MultiLocationValueTimeSeriesForMultiLocation = new Dictionary<LocationCollection, Dict<int, string, double>>();

        public IGraphValueReader GraphValueReader { get; set; }

        public Dict<int, string, string, double> GraphValues { get; set; }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            var cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MARV");
            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, cacheDirectory);

            this.MapView.TileLayer = TileLayers.BingMapsAerial;
        }

        public static Dict<int, string, double> CalculateMultiLocationValueTimeSeriesAndWrite(LocationCollection multiLocation, Graph graph = null)
        {
            logger.Info("Computing belief for line {0}.", multiLocation.Name);

            var vertexKey = "B08";
            var vertexName = graph.Vertices[vertexKey].Name;
            var stateKey = "Fail";
            // var quantity = "Mean";

            var multiLocationValueTimeSeries = new Dict<int, string, double>();
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
                //var excelCol = 1;

                //excelWorkSheet.SetValue(++excelRow, excelCol, location.Name);

                //var fileName = GetFileNameForModelValue(multiLocation.Name, location.Name);

                //try
                //{
                //    var modelValue = Odb.ReadValueSingle<Dict<int, string, string, double>>(fileName, x => true);

                //    foreach (var year in modelValue.Keys)
                //    {
                //        excelWorkSheet.SetValue(1, ++excelCol, year);

                //        var graphValue = modelValue[year];
                //        var vertexValue = graphValue[vertexKey];
                //        var stateValue = vertexValue[stateKey];

                //        if (!multiLocationValueTimeSeries.ContainsKey(year))
                //        {
                //            multiLocationValueTimeSeries[year] = new Dictionary<string, double>();
                //        }

                //        multiLocationValueTimeSeries[year][location.Name] = stateValue;

                //        // var extractedValue = graph.GetMean(vertexKey, vertexValue);
                //        // var extractedValue = graph.GetStandardDeviation(vertexKey, vertexValue);
                //        var extractedValue = stateValue;
                //        excelWorkSheet.SetValue(excelRow, excelCol, extractedValue);
                //    }
                //}
                //catch (OdbDataNotFoundException)
                //{
                //    logger.Info("Belief not found for point {0}.", location);
                //}

                //logger.Info("Completed {0} of {1}", ++nCompleted, nLocations);
            }

            excelPackage.Save();

            var fName = GetFileNameForMultiLocationValueTimeSeries(multiLocation, vertexKey, stateKey);
            Odb.Write(fName, multiLocationValueTimeSeries);

            return multiLocationValueTimeSeries;
        }

        public static Task<Dict<int, string, double>> CalculateMultiLocationValueTimeSeriesAndWriteAsync(LocationCollection multiLocation, Graph graph)
        {
            return Task.Run(() => { return CalculateMultiLocationValueTimeSeriesAndWrite(multiLocation, graph); });
        }

        public static string GetFileNameForModelValue(string multiLocationName, string locationName)
        {
            return Path.Combine("ModelValues", multiLocationName, locationName + ".db");
        }

        public static string GetFileNameForMultiLocationValueTimeSeries(LocationCollection multiLocation, string vertexKey, string stateKey)
        {
            return Path.Combine("MultiLocationValueTimeSeries", multiLocation.Name, vertexKey + "_" + stateKey + ".db");
        }

        public void ReadGraphValues()
        {
            //var multiLocation = this.Polylines.SelectedItem;
            //var location = multiLocation.SelectedItem;

            try
            {
                //this.GraphValues = this.GraphValueReader.Read(multiLocation.Name, location.Name);
            }
            catch (GraphValueNotFoundException exception)
            {
                logger.Warn(exception.Message);

                this.Notifications.Push(new NotificationTimed
                {
                    Name = "Belief Not Found",
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
                    var fileName = GetFileNameForMultiLocationValueTimeSeries(multiLocation, "B08", "Fail");
                    this.MultiLocationValueTimeSeriesForMultiLocation[multiLocation] = Odb.ReadValueSingle<Dict<int, string, double>>(fileName, x => true);
                }
                catch (OdbDataNotFoundException)
                {
                    logger.Warn("Belief not found for line {0}.", multiLocation.Name);
                }
            }
        }

        public static void RunAndWrite(string networkFileName, string inputFileName, string multiLocationName, string locationName, int startYear, int endYear)
        {
            var graph = Graph.Read(networkFileName);
            var graphEvidence = AdcoInput.GetGraphEvidence(graph, inputFileName, multiLocationName, locationName);

            // var graphValueTimeSeries = graph.Run(graphEvidence, startYear, endYear);

            var fileName = GetFileNameForModelValue(multiLocationName, locationName);

            try
            {
                // Odb.Write(fileName, graphValueTimeSeries);
            }
            catch (IOException exp)
            {
                logger.Warn(exp.Message);
            }
        }

        public static Task RunAndWriteAsync(string networkFileName, string inputFileName, string multiLocationName, string locationName, int startYear, int endYear)
        {
            return Task.Run(() => { RunAndWrite(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear); });
        }

        public void UpdateGraphValue()
        {
            if (this.Graph != null)
            {
                if (this.GraphValues != null)
                {
                    if (this.GraphValues.ContainsKey(this.SelectedYear))
                    {
                        //this.Graph.Belief = this.GraphValues[this.SelectedYear];
                    }
                    else
                    {
                        this.Graph.Vertices.SetBelief(0);
                    }
                }
                else
                {
                    this.Graph.Vertices.SetBelief(0);
                }
            }
        }

        public void UpdateMultiLocationValues()
        {
            //foreach (var multiLocation in this.Polylines)
            //{
            //    if ((int) multiLocation.Properties["StartYear"] > this.SelectedYear)
            //    {
            //        multiLocation.IsEnabled = false;
            //    }
            //    else
            //    {
            //        multiLocation.IsEnabled = true;

            //        if (this.Polylines.SelectedItem.IsEnabled == false)
            //        {
            //            // this.Polylines.Select(multiLocation);
            //        }

            //        try
            //        {
            //            multiLocation.Value = this.MultiLocationValueTimeSeriesForMultiLocation[multiLocation][this.SelectedYear];
            //        }
            //        catch (KeyNotFoundException)
            //        {
            //            logger.Warn("Belief not found for line {0} for year {1}", multiLocation.Name, this.SelectedYear);
            //        }
            //    }
            //}
        }
    }
}