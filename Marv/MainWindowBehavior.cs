using Marv.Common;
using Marv.LineAndSectionOverviewService;
using Marv.LoginService;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows;
using Telerik.Windows.Controls.TransitionControl;

namespace Marv
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closing += AssociatedObject_Closing;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded_LoginSynergi;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded_ReadNetwork;
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_Closing(object sender, CancelEventArgs e)
        {
            this.AssociatedObject.SourceGraph.Write(this.AssociatedObject.NetworkFileName);
            Properties.Settings.Default.Save();
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

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            // Change this line when switching between ADCO and CNPC and any future readers.
            window.GraphValueReader = new GraphValueReaderAdco();

            window.SelectedYearChanged += window_SelectedYearChanged;

            window.EditNetworkFilesMenuItem.Click += EditNetworkFilesMenuItem_Click;
            window.EditSettingsMenuItem.Click += EditSettingsMenuItem_Click;

            window.LocationRunModelMenuItem.Click += LocationRunModelMenuItem_Click;
            window.PipelineRunModelMenuItem.Click += PipelineRunModelMenuItem_Click;
            window.NetworkRunModelMenuItem.Click += NetworkRunModelMenuItem_Click;

            window.PipelineComputeValueMenuItem.Click += PipelineComputeValueMenuItem_Click;
            window.NetworkComputeValue.Click += NetworkComputeValue_Click;

            window.BackButton.Click += BackButton_Click;
            window.ChartControlCloseButton.Click += ChartControlCloseButton_Click;
            window.RetractAllButton.Click += RetractAllButton_Click;
            window.TransitionControl.StatusChanged += TransitionControl_StatusChanged;

            window.LinesListBox.SelectionChanged += LinesListBox_SelectionChanged;
            window.SectionsListBox.SelectionChanged += SectionsListBox_SelectionChanged;

            // Change map types
            window.BingMapsAerialMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.BingMapsAerial;
            window.BingMapsRoadsMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.BingMapsRoads;
            window.MapBoxAerialMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxAerial;
            window.MapBoxRoadsMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxRoads;
            window.MapBoxTerrainMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxTerrain;
        }

        private void window_SelectedYearChanged(object sender, double e)
        {
            var window = this.AssociatedObject as MainWindow;
            window.UpdateGraphValue();
            window.UpdateMultiLocationValues();
        }

        private void AssociatedObject_Loaded_LoginSynergi(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject as MainWindow;

            LoginService.BRIXLoginService loginService = new BRIXLoginService();
            window.SynergiViewModel.Ticket = loginService.LogIn(window.SynergiViewModel.UserName, window.SynergiViewModel.Password);

            LineAndSectionOverviewService.LineAndSectionOverviewService lineAndSectionOverviewService = new LineAndSectionOverviewService.LineAndSectionOverviewService();
            lineAndSectionOverviewService.BRIXAuthenticationHeaderValue = new LineAndSectionOverviewService.BRIXAuthenticationHeader { value = window.SynergiViewModel.Ticket };
            window.SynergiViewModel.Lines = new SelectableCollection<LineSummaryDTO>(lineAndSectionOverviewService.GetLines());
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

            //window.ReadGraphValueTimeSeries();
            //window.UpdateGraphValue();
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

        private void LinesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var window = this.AssociatedObject;
            var selectedLine = window.SynergiViewModel.Lines.SelectedItem;
            var ticket = window.SynergiViewModel.Ticket;

            if (selectedLine != null)
            {
                LineAndSectionOverviewService.LineAndSectionOverviewService lineAndSectionOverviewService = new LineAndSectionOverviewService.LineAndSectionOverviewService();
                lineAndSectionOverviewService.BRIXAuthenticationHeaderValue = new LineAndSectionOverviewService.BRIXAuthenticationHeader { value = ticket };

                window.SynergiViewModel.Sections = new SelectableCollection<SectionSummaryDTO>(lineAndSectionOverviewService.GetSections(selectedLine.LineOid));
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

            var startYear = (int)multiLocation.Properties["StartYear"];
            var endYear = window.EndYear;

            await MainWindow.RunAndWriteAsync(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);
        }

        private async void NetworkComputeValue_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var multiLocations = window.Polylines;

            var multiLocationValueTimeSeriesForMultiLocation = new Dictionary<LocationCollection, MultiLocationValueTimeSeries>();

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

                    var startYear = (int)multiLocation.Properties["StartYear"];

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

            var startYear = (int)multiLocation.Properties["StartYear"];
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

        private void SectionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var window = this.AssociatedObject;
            var selectedSection = window.SynergiViewModel.Sections.SelectedItem;
            var ticket = window.SynergiViewModel.Ticket;

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

                SegmentationService.SegmentationService segmentationService = new SegmentationService.SegmentationService();
                segmentationService.BRIXAuthenticationHeaderValue = new SegmentationService.BRIXAuthenticationHeader { value = ticket };

                try
                {
                    var segments = segmentationService.GetSegments(selectedSection.SectionOid.ToString(), data, "m", CultureInfo.CurrentCulture.Name);
                    var nHeaders = segments.Headers.Count();
                    var nSegments = segments.Segments.Count();

                    logger.Info("nSegments" + nSegments);

                    var segmentData = new Dict<string, string>();
                    var properties = new Dynamic();

                    for (int s = 0; s < nSegments - 1; s++)
                    {
                        var segmentVm = segments.Segments[s];

                        for (int h = 0; h < nHeaders; h++)
                        {
                            var header = segments.Headers[h];
                            var propertyName = string.IsNullOrEmpty(header.Unit) ? header.Name : string.Format("{0} [{1}]", header.Name, header.Unit);
                            var propertyValue = segmentVm.Data[h];

                            segmentData[propertyName] = propertyValue;
                        }
                    }

                    window.SynergiViewModel.SegmentData = segmentData;
                }
                catch (Exception exception)
                {
                    logger.Warn(exception.Message);
                }

                //dgSegment.AutoGenerateColumns = true;
                //dgSegment.DataSource = dataTable;
            }
        }

        private void TransitionControl_StatusChanged(object sender, TransitionStatusChangedEventArgs e)
        {
            var window = this.AssociatedObject;

            if (e.Status == TransitionStatus.Completed)
            {
                window.MapView.ZoomTo(window.Polylines.GetBounds());
            }
        }
    }
}