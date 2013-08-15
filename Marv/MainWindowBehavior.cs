using LibBn;
using LibPipeline;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows;

namespace Marv
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closing += AssociatedObject_Closing;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var graph in this.AssociatedObject.Graphs)
            {
                graph.Write(graph.FileName);
            }

            Properties.Settings.Default.Save();
        }

        private async void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            var window = this.AssociatedObject;

            if (e.KeyboardDevice.IsKeyDown(Key.R) &&
                   (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
                int nLocations = window.MultiLocations.SelectedItem.Count();
                int nCompleted = 0;

                foreach (var location in window.MultiLocations.SelectedItem)
                {
                    var locationValue = await window.LocationValueStore.GetLocationValueAsync(location);
                    window.MultiLocations.SelectedItem.SelectedItem = location;
                    Console.WriteLine("Completed " + ++nCompleted + " of " + nLocations + " on " + DateTime.Now);
                }
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.M) &&
               (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
                window.IsMenuVisible = !window.IsMenuVisible;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.O) &&
                   (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".net",
                    Filter = "Hugin Network Files (.net)|*.net",
                    Multiselect = true
                };

                if (dialog.ShowDialog() == true)
                {
                    window.Graphs.Clear();

                    foreach (var fileName in dialog.FileNames)
                    {
                        window.Graphs.Add(await BnGraph.ReadAsync<BnVertexViewModel>(fileName));
                    }
                }
            }
        }

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.MultiLocations = new SelectableCollection<MultiLocation>();

            await window.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    window.PopupControl.ShowTextIndeterminate("Reading Profile");
                    var locations = await ExcelReader.ReadLocationsWithPropertiesAsync(Properties.Settings.Default.ProfileFileName);

                    var random = new Random();

                    foreach(var location in locations)
                    {
                        location.Value = random.NextDouble();
                    }

                    window.MultiLocations.Add(locations);
                    window.PopupControl.Hide();

                    // window.MapView.ZoomToExtent(window.ProfileLocations.Bounds());

                    // window.PopupControl.ShowTextIndeterminate("Reading tally");
                    // window.MultiLocations.Add(await ExcelReader.ReadLocationsWithPropertiesAsync(Properties.Settings.Default.TallyFileName));
                    // window.PopupControl.Hide();
                }));

            KocDataReader kocDataReader = new KocDataReader();
            window.VertexValuesByYear = kocDataReader.ReadVertexValuesForAllYears();
            window.SelectedVertexValues = window.VertexValuesByYear.First().Value;
            kocDataReader.ReadVertexInputsForAllYears(window.InputManager);

            //window.AutoCompleteBox.SelectionChanged += ComboBox_SelectionChanged;
            //window.SensorListener.NewEvidenceAvailable += SensorListener_NewEvidenceAvailable;
            window.RetractAllButton.Click += RetractAllButton_Click;
            window.EditNetworkFilesMenuItem.Click += EditNetworkFilesMenuItem_Click;
            window.EditSettingsMenuItem.Click += EditSettingsMenuItem_Click;
            window.NetworkFilesAddButton.Click += NetworkFilesAddButton_Click;
            window.NetworkFilesRemoveButton.Click += NetworkFilesRemoveButton_Click;
            window.RunModelButton.Click += RunModelButton_Click;
            window.RunModelMenuItem.Click += RunModelMenuItem_Click;

            var earthquakes = await Utils.ReadEarthquakesAsync();

            foreach (var eq in earthquakes)
            {
                Console.WriteLine("Earthquake of magnitude " + eq.Value + " at " + eq + " on " + eq["Date"]);
            }
        }

        private void RunModelMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            window.TransitionControl.SelectElement("RunModelControl");
        }

        private async void RunModelButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            int nLocations = window.MultiLocations.SelectedItem.Count();
            int startIndex = 2000;
            int nCompleted = startIndex;

            foreach (var location in window.MultiLocations.SelectedItem.Skip(startIndex))
            {
                var locationValue = await window.GetLocationValueAsync(location);
                window.MultiLocations.SelectedItem.SelectedItem = location;
                window.ModelProgressBar.Value = (double)nCompleted++ / (double)nLocations * 100;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.Graphs["nnphscc"];

            window.MultiPoints.Clear();

            //foreach (var selectedItem in window.AutoCompleteBox.SelectedItems)
            //{
            //    var selectedVertex = selectedItem as BnVertex;
            //    var points = new ObservableCollection<Point>();

            //    for (int year = window.StartYear; year <= window.EndYear; year++)
            //    {
            //        var vertexValue = window.SelectedLocationValue[year]["nnphscc"][selectedVertex.Key];

            //        points.Add(new Point
            //        {
            //            X = year,
            //            Y = graph.GetVertex(selectedVertex.Key).GetMean(vertexValue)
            //        });
            //    }

            //    window.MultiPoints.Add(new MultiPoint { Points = points });
            //}
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

        private void NetworkFilesAddButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            var dialog = new OpenFileDialog
            {
                DefaultExt = ".net",
                Filter = "Hugin Network Files (.net)|*.net",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    window.NetworkFileNames.Add(fileName);
                }
            }
        }

        private void NetworkFilesRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.NetworkFileNames.RemoveSelected();
        }

        private void RetractAllButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            foreach (var graph in window.Graphs)
            {
                graph.ClearEvidence();
                graph.UpdateBeliefs();
                graph.UpdateValue();
            }
        }

        private void SensorListener_NewEvidenceAvailable(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            window.AddInput(e.Value);

            var success = window.TryUpdateNetwork();

            if (!success)
            {
                window.PopupControl.ShowText("Inconsistent evidence entered.");
                window.InputManager.RemoveVertexInput(BnInputType.User, window.SelectedYear, e.Value.Key);
                window.TryUpdateNetwork();
            }
        }
    }
}