using LibPipeline;
using Marv.Common;
using NLog;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows;

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

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.SourceGraph = await Graph.ReadAsync<Vertex>(window.NetworkFileName);
            window.DisplayGraph = window.SourceGraph.GetSubGraph(window.SourceGraph.DefaultGroup);

            window.MultiLocations = new SelectableCollection<MultiLocation>();

            try
            {
                window.MultiLocations = AdcoInput.Read(window.InputFileName);

                window.ReadMultiLocationValueTimeSeriesForMultiLocation();
                window.UpdateMultiLocationValues();

                window.ReadGraphValueTimeSeries();
                window.UpdateGraphValue();
            }
            catch (IOException exp)
            {
                logger.Warn(exp.Message);

                var notification = new TimedNotification
                {
                    Name = "Unable to read file.",
                    Description = exp.Message,
                };

                window.Notifications.Push<INotification>(notification);
            }

            window.EditNetworkFilesMenuItem.Click += EditNetworkFilesMenuItem_Click;
            window.EditSettingsMenuItem.Click += EditSettingsMenuItem_Click;

            window.LocationRunModelMenuItem.Click += LocationRunModelMenuItem_Click;
            window.PipelineRunModelMenuItem.Click += PipelineRunModelMenuItem_Click;
            window.NetworkRunModelMenuItem.Click += NetworkRunModelMenuItem_Click;

            window.PipelineComputeValueMenuItem.Click += PipelineComputeValueMenuItem_Click;
            window.NetworkComputeValue.Click += NetworkComputeValue_Click;

            window.BackButton.Click += BackButton_Click;
            window.RetractAllButton.Click += RetractAllButton_Click;

            // Change map types
            window.BingMapsAerialMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.BingMapsAerial;
            window.BingMapsRoadsMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.BingMapsRoads;
            window.MapBoxAerialMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxAerial;
            window.MapBoxRoadsMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxRoads;
            window.MapBoxTerrainMenuItem.Click += (o1, e1) => window.MapView.TileLayer = TileLayers.MapBoxTerrain;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            var sourceGraph = window.SourceGraph;

            window.DisplayGraph = sourceGraph.GetSubGraph(sourceGraph.DefaultGroup);
            window.IsBackButtonVisible = false;
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

        private async void LocationRunModelMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var networkFileName = window.NetworkFileName;
            var inputFileName = window.InputFileName;

            var multiLocation = window.MultiLocations.SelectedItem;

            var multiLocationName = multiLocation.Name;
            var locationName = multiLocation.SelectedItem.Name;

            var startYear = (int)multiLocation["StartYear"];
            var endYear = window.EndYear;

            await MainWindow.RunAndWriteAsync(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);
        }

        private async void NetworkComputeValue_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var multiLocations = window.MultiLocations;

            var multiLocationValueTimeSeriesForMultiLocation = new Dictionary<MultiLocation, MultiLocationValueTimeSeries>();

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

            var multiLocations = window.MultiLocations;

            var endYear = window.EndYear;

            await Task.Run(() =>
            {
                foreach (var multiLocation in multiLocations)
                {
                    var multiLocationName = multiLocation.Name;

                    var startYear = (int)multiLocation["StartYear"];

                    var nCompleted = 0;
                    var nLocations = multiLocation.Count;

                    foreach (var location in multiLocation)
                    {
                        var locationName = location.Name;

                        MainWindow.RunAndWrite(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);

                        logger.Info("Ran model and wrote for location {0} on line {1} ({2} of {3})", locationName, multiLocationName, ++nCompleted, nLocations);
                    }
                }
            });
        }

        private async void PipelineComputeValueMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var multiLocation = window.MultiLocations.SelectedItem;

            window.MultiLocationValueTimeSeriesForMultiLocation[multiLocation] = await MainWindow.CalculateMultiLocationValueTimeSeriesAndWriteAsync(multiLocation, graph);
            window.UpdateMultiLocationValues();
        }

        private async void PipelineRunModelMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var networkFileName = window.NetworkFileName;
            var inputFileName = window.InputFileName;

            var multiLocation = window.MultiLocations.SelectedItem;
            var multiLocationName = multiLocation.Name;

            var startYear = (int)multiLocation["StartYear"];
            var endYear = window.EndYear;

            var nCompleted = 0;
            var nLocations = multiLocation.Count;

            await Task.Run(() =>
                {
                    foreach (var location in multiLocation)
                    {
                        var locationName = location.Name;

                        MainWindow.RunAndWrite(networkFileName, inputFileName, multiLocationName, locationName, startYear, endYear);

                        logger.Info("Ran model and wrote for location {0} on line {1} ({2} of {3})", locationName, multiLocationName, ++nCompleted, nLocations);
                    }
                });
        }

        private void RetractAllButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            window.SourceGraph.Value = window.SourceGraph.ClearEvidence();
        }
    }
}