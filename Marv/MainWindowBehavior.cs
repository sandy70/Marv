using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Map;
using Marv.Controls;
using Marv.LineAndSectionOverviewService;
using Marv.LoginService;
using Marv.Properties;
using NLog;
using Telerik.Windows;
using Telerik.Windows.Controls.TransitionControl;
using BRIXAuthenticationHeader = Marv.LineAndSectionOverviewService.BRIXAuthenticationHeader;

namespace Marv
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private object _lock = new object();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closing += this.AssociatedObject_Closing;
            this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
            this.AssociatedObject.Loaded += this.AssociatedObject_Loaded_LoginSynergi;
            this.AssociatedObject.Loaded += this.AssociatedObject_Loaded_ReadNetwork;
            this.AssociatedObject.KeyDown += this.AssociatedObject_KeyDown;
        }

        private void AssociatedObject_Closing(object sender, CancelEventArgs e)
        {
            this.AssociatedObject.SourceGraph.Write(this.AssociatedObject.NetworkFileName);
            Settings.Default.Save();
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            var window = this.AssociatedObject;

            if (e.KeyboardDevice.IsKeyDown(Key.R) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.M) &&
                     (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
                window.IsMenuVisible = !window.IsMenuVisible;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.O) &&
                     (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
            }
        }

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            // Change this line when switching between ADCO and CNPC and any future readers.
            window.GraphValueReader = new GraphValueReaderAdco();

            window.SelectedYearChanged += this.window_SelectedYearChanged;

            window.EditNetworkFilesMenuItem.Click += this.EditNetworkFilesMenuItem_Click;
            window.EditSettingsMenuItem.Click += this.EditSettingsMenuItem_Click;

            window.LocationRunModelMenuItem.Click += this.LocationRunModelMenuItem_Click;
            window.PipelineRunModelMenuItem.Click += this.PipelineRunModelMenuItem_Click;
            window.NetworkRunModelMenuItem.Click += this.NetworkRunModelMenuItem_Click;

            window.PipelineComputeValueMenuItem.Click += this.PipelineComputeValueMenuItem_Click;
            window.NetworkComputeValue.Click += this.NetworkComputeValue_Click;

            window.BackButton.Click += this.BackButton_Click;
            window.RetractAllButton.Click += this.RetractAllButton_Click;
            window.TransitionControl.StatusChanged += this.TransitionControl_StatusChanged;

            window.LinesListBox.SelectionChanged += this.LinesListBox_SelectionChanged;
            window.SectionsListBox.SelectionChanged += this.SectionsListBox_SelectionChanged;

            window.SynergiRunButton.Click += this.SynergiRunButton_Click;

            // Change map types
            window.BingMapsAerialMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.BingMapsAerial;
            window.BingMapsRoadsMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.BingMapsRoads;
            window.MapBoxAerialMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxAerial;
            window.MapBoxRoadsMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxRoads;
            window.MapBoxTerrainMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxTerrain;

            window.EarthquakeControl.ScalingFunc = x => Marv.Common.Utils.Clamp(Math.Pow(x, 1.2)*10, 1, 150);

            window.Earthquakes = new ModelCollection<Location>(await Marv.Common.Map.Utils.ReadEarthquakesAsync(new Progress<double>()));
        }

        private void AssociatedObject_Loaded_LoginSynergi(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var loginService = new BRIXLoginService();

            try
            {
                window.SynergiModel.Ticket = loginService.LogIn(window.SynergiModel.UserName, window.SynergiModel.Password);

                var lineAndSectionOverviewService = new LineAndSectionOverviewService.LineAndSectionOverviewService();
                lineAndSectionOverviewService.BRIXAuthenticationHeaderValue = new BRIXAuthenticationHeader {value = window.SynergiModel.Ticket};
                window.SynergiModel.Lines = new SelectableCollection<LineSummaryDTO>(lineAndSectionOverviewService.GetLines().Where(x => x.Name == "BU-498"));
            }
            catch (Exception)
            {
                logger.Error("Unable to log in to Synergi Pipeline");
            }
        }

        private async void AssociatedObject_Loaded_ReadNetwork(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var notification = new NotificationIndeterminate
            {
                Name = "Reading Network",
                Description = "Reading network from file " + window.NetworkFileName
            };

            window.Notifications.Push(notification);

            // Read source graph
            window.SourceGraph = await Graph.ReadAsync(window.NetworkFileName);

            // Set display graph
            window.DisplayGraph = window.SourceGraph.GetSubGraph(window.SourceGraph.DefaultGroup);

            // Close notification
            notification.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            var sourceGraph = window.SourceGraph;

            window.DisplayGraph = sourceGraph.GetSubGraph(sourceGraph.DefaultGroup);
            window.IsBackButtonVisible = false;
        }

        private void ChartControlCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.IsChartControlVisible = false;
        }

        private void EditNetworkFilesMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            window.TransitionControl.SelectElement("EditNetworkFilesControl");
        }

        private void EditSettingsMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            window.TransitionControl.SelectElement("SettingsControl");
        }

        private void LinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = this.AssociatedObject;
            var selectedLine = window.SynergiModel.Lines.SelectedItem;
            var ticket = window.SynergiModel.Ticket;

            if (selectedLine != null)
            {
                var lineAndSectionOverviewService = new LineAndSectionOverviewService.LineAndSectionOverviewService();
                lineAndSectionOverviewService.BRIXAuthenticationHeaderValue = new BRIXAuthenticationHeader {value = ticket};

                window.SynergiModel.Sections = new SelectableCollection<SectionSummaryDTO>(lineAndSectionOverviewService.GetSections(selectedLine.LineOid));
            }
        }

        private async void LocationRunModelMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var networkFileName = window.NetworkFileName;
            var inputFileName = window.InputFileName;

            var multiLocation = window.Polylines.SelectedItem;

            var multiLocationName = multiLocation.Name;
            var locationName = multiLocation.SelectedItem.Name;

            var startYear = (int) multiLocation.Properties["StartYear"];
            var endYear = window.EndYear;

            await MainWindow.RunAndWriteAsync(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);
        }

        private async void NetworkComputeValue_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var multiLocations = window.Polylines;

            var multiLocationValueTimeSeriesForMultiLocation = new Dictionary<LocationCollection, Dictionary<int, string, double>>();

            await Task.Run(() =>
            {
                foreach (var multiLocation in multiLocations)
                {
                    multiLocationValueTimeSeriesForMultiLocation[multiLocation] = MainWindow.CalculateMultiLocationValueTimeSeriesAndWrite(multiLocation, graph);
                }
            });

            window.MultiLocationValueTimeSeriesForMultiLocation = multiLocationValueTimeSeriesForMultiLocation;
            window.UpdateMultiLocationValues();
        }

        private async void NetworkRunModelMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var networkFileName = window.NetworkFileName;
            var inputFileName = window.InputFileName;

            var multiLocations = window.Polylines;

            var endYear = window.EndYear;

            await Task.Run(() =>
            {
                foreach (var multiLocation in multiLocations)
                {
                    var multiLocationName = multiLocation.Name;

                    var startYear = (int) multiLocation.Properties["StartYear"];

                    var nCompleted = 0;
                    var nLocations = multiLocation.Count;

                    foreach (var location in multiLocation)
                    {
                        var locationName = location.Name;

                        MainWindow.RunAndWrite(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);

                        logger.Info("Ran model and wrote for point {0} on line {1} ({2} of {3})", locationName, multiLocationName, ++nCompleted, nLocations);
                    }
                }
            });
        }

        private async void PipelineComputeValueMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var multiLocation = window.Polylines.SelectedItem;

            window.MultiLocationValueTimeSeriesForMultiLocation[multiLocation] = await MainWindow.CalculateMultiLocationValueTimeSeriesAndWriteAsync(multiLocation, graph);
            window.UpdateMultiLocationValues();
        }

        private async void PipelineRunModelMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var networkFileName = window.NetworkFileName;
            var inputFileName = window.InputFileName;

            var multiLocation = window.Polylines.SelectedItem;
            var multiLocationName = multiLocation.Name;

            var startYear = (int) multiLocation.Properties["StartYear"];
            var endYear = window.EndYear;

            var nCompleted = 0;
            var nLocations = multiLocation.Count;

            await Task.Run(() =>
            {
                foreach (var location in multiLocation)
                {
                    var locationName = location.Name;

                    MainWindow.RunAndWrite(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);

                    logger.Info("Ran model and wrote for point {0} on line {1} ({2} of {3})", locationName, multiLocationName, ++nCompleted, nLocations);
                }
            });
        }

        private void RetractAllButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            window.SourceGraph.Value = window.SourceGraph.ClearEvidence();
        }

        private void SectionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = this.AssociatedObject;
            var selectedSection = window.SynergiModel.Sections.SelectedItem;
            var ticket = window.SynergiModel.Ticket;

            if (selectedSection != null)
            {
                window.Polylines["BU-498"].Select(selectedSection.Name);
            }

            if (selectedSection != null)
            {
                var dataTable = new DataTable();

                // The order here is taken from MarvToSynergiMap.xlsx
                var data = new[]
                {
                    "MAOP.MAOP",
                    "Chemistry.ChloridesPresent",
                    "Segment.CleaningPigRunsPerYear",
                    "Chemistry.CO2",
                    "Chemistry.CorrosionInhibition",
                    "ExternalCorrosion.CoveredPercent",
                    "ExternalCorrosion.DistanceFromTheSea",
                    "Chemistry.Fe",
                    "FlowParameters.GasDensity",
                    "FlowParameters.Gas_Velocity",
                    "Chemistry.Hydrocarbon",
                    "Segment.NominalOuterDiameter",
                    "PipeBook.Latitude",
                    "PipeBook.Longitude",
                    "DesignPressure.DesignPressure",
                    "FlowParameters.LiquidVelocity",
                    "ExternalCorrosion.ExternalSandMoistureContent",
                    "Chemistry.O2",
                    "FlowParameters.OilDensity",
                    "NormalOperation.NormalOperationPressure",
                    "Chemistry.pH",
                    "FlowParameters.PipeInclination",
                    "Segment.NominalWallThickness",
                    "FlowParameters.SandPresent",
                    "Segment.SMYS",
                    "ExternalCorrosion.SoilResistivity",
                    "Chemistry.Sulphides",
                    "Chemistry.WaterCut"
                };

                var segmentationService = new SegmentationService.SegmentationService();
                segmentationService.BRIXAuthenticationHeaderValue = new SegmentationService.BRIXAuthenticationHeader {value = ticket};

                try
                {
                    var segments = segmentationService.GetSegments(selectedSection.SectionOid.ToString(), data, "m", CultureInfo.CurrentCulture.Name);
                    var nHeaders = segments.Headers.Count();
                    var nSegments = segments.Segments.Count();

                    logger.Info("nSegments" + nSegments);

                    var segmentData = new Dictionary<string, string>();
                    var properties = new Dynamic();

                    for (var s = 0; s < nSegments - 1; s++)
                    {
                        var segmentVm = segments.Segments[s];

                        for (var h = 0; h < nHeaders; h++)
                        {
                            var header = segments.Headers[h];
                            var propertyName = string.IsNullOrEmpty(header.Unit) ? header.Name : string.Format("{0} [{1}]", header.Name, header.Unit);
                            var propertyValue = segmentVm.Data[h];

                            segmentData[propertyName] = propertyValue;
                        }
                    }

                    window.SynergiModel.SegmentData = segmentData;
                }
                catch (Exception exception)
                {
                    logger.Warn(exception.Message);
                }

                //dgSegment.AutoGenerateColumns = true;
                //dgSegment.DataSource = dataTable;
            }
        }

        private async void SynergiRunButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var networkFileName = window.NetworkFileName;
            var inputFileName = window.InputFileName;

            var multiLocation = window.Polylines.SelectedItem;

            var multiLocationName = multiLocation.Name;
            var locationName = multiLocation.SelectedItem.Name;

            var startYear = (int) multiLocation.Properties["StartYear"];
            var endYear = window.EndYear;

            var notification = new NotificationIndeterminate
            {
                Name = "Running Model",
                Description = "Running model for location: " + locationName
            };

            window.Notifications.Push(notification);

            await MainWindow.RunAndWriteAsync(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);

            notification.Close();

            window.ReadGraphValues();
            window.UpdateGraphValue();
        }

        private void TransitionControl_StatusChanged(object sender, TransitionStatusChangedEventArgs e)
        {
            var window = this.AssociatedObject;

            if (e.Status == TransitionStatus.Completed)
            {
                window.MapView.ZoomTo(window.Polylines.GetBounds());
            }
        }

        private void window_SelectedYearChanged(object sender, double e)
        {
            var window = this.AssociatedObject;
            window.UpdateGraphValue();
            window.UpdateMultiLocationValues();
        }
    }
}