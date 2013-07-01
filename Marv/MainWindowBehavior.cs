using LibBn;
using LibPipeline;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

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

            if (e.KeyboardDevice.IsKeyDown(Key.S) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt)))
            {
                this.AssociatedObject.IsSettingsVisible = !this.AssociatedObject.IsSettingsVisible;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.R) &&
                   (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
                int nLocations = window.ProfileLocations.Count();
                int nCompleted = 0;

                foreach (var location in window.ProfileLocations)
                {
                    await window.LocationValueStore.GetLocationValueAsync(location);
                    window.SelectedProfileLocation = location;
                    Console.WriteLine("Completed " + ++nCompleted + " of " + nLocations + " on " + DateTime.Now);
                }
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

            //await window.Dispatcher.BeginInvoke(new Action(async () =>
            //    {
            //        window.PopupControl.ShowTextIndeterminate("Reading Profile");
            //        window.ProfileLocations = await ExcelReader.ReadPropertyLocationsAsync<PropertyLocation>(Properties.Settings.Default.ProfileFileName);
            //        window.PopupControl.Hide();

            //        window.MapView.ZoomToExtent(window.ProfileLocations.Bounds());

            //        window.PopupControl.ShowTextIndeterminate("Reading tally");
            //        window.TallyLocations = await ExcelReader.ReadPropertyLocationsAsync<PropertyLocation>(Properties.Settings.Default.TallyFileName);
            //        window.PopupControl.Hide();
            //    }));

            await window.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (window.NetworkFileNames != null)
                    {
                        foreach (var fileName in window.NetworkFileNames)
                        {
                            window.Graphs.Add(await BnGraph.ReadAsync<BnVertexViewModel>(fileName));
                        }
                    }

                    //window.Graphs.Add(await BnGraph.ReadAsync<BnVertexViewModel>(@"D:\Data\SCC\NNpHSCC\NNpHSCC.net"));
                    //window.Graphs.Add(await BnGraph.ReadAsync<BnVertexViewModel>(@"D:\Data\SCC\NNpHSCC\NNpH_failure.net"));
                }));

            //KocDataReader kocDataReader = new KocDataReader();
            //window.VertexValuesByYear = kocDataReader.ReadVertexValuesForAllYears();
            //window.SelectedVertexValues = window.VertexValuesByYear.First().Value;
            //kocDataReader.ReadVertexInputsForAllYears(window.InputManager);

            //window.NearNeutralPhSccModel.Graphs = window.Graphs;
            //window.NearNeutralPhSccModel.StartYear = window.StartYear;
            //window.NearNeutralPhSccModel.EndYear = window.EndYear;

            //window.LocationValueStore.Model = window.NearNeutralPhSccModel;

            //window.AutoCompleteBox.SelectionChanged += ComboBox_SelectionChanged;
            //window.SensorListener.NewEvidenceAvailable += SensorListener_NewEvidenceAvailable;
            window.RetractAllButton.Click += RetractAllButton_Click;
            window.EditNetworkFilesMenuItem.Click += EditNetworkFilesMenuItem_Click;
            window.EditNetworkFileNamesControlBackButton.Click += EditNetworkFileNamesControlBackButton_Click;
        }

        private void EditNetworkFileNamesControlBackButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.IsEditNetworkFileNamesControlVisible = false;
            window.IsTabControlVisible = true;
        }

        private void EditNetworkFilesMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.IsTabControlVisible = false;
            window.IsEditNetworkFileNamesControlVisible = true;
        }

        private void RetractAllButton_Click(object sender, RoutedEventArgs e)
        {
            var window  = this.AssociatedObject;

            foreach (var graph in window.Graphs)
            {
                graph.ClearEvidence();
                graph.UpdateBeliefs();
                graph.UpdateValue();
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