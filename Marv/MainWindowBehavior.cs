using LibBn;
using LibPipeline;
using System;
using System.Linq;
using System.Windows;
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

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.S) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt)))
            {
                this.AssociatedObject.IsSettingsVisible = !this.AssociatedObject.IsSettingsVisible;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.R) &&
                   (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
            }
        }

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            await window.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    window.PopupControl.ShowTextIndeterminate("Reading Profile");
                    window.ProfileLocations = await ExcelReader.ReadPropertyLocationsAsync<PropertyLocation>(Properties.Settings.Default.ProfileFileName);
                    window.PopupControl.Hide();

                    window.MapView.ZoomToExtent(window.ProfileLocations.Bounds());

                    window.PopupControl.ShowTextIndeterminate("Reading tally");
                    window.TallyLocations = await ExcelReader.ReadPropertyLocationsAsync<PropertyLocation>(Properties.Settings.Default.TallyFileName);
                    window.PopupControl.Hide();
                }));

            await window.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    window.Graphs.Add(await BnGraph.ReadAsync<BnVertexViewModel>(@"D:\Data\SCC\NNpHSCC\NNpHSCC 2013 05 21.net"));
                    window.Graphs.Add(await BnGraph.ReadAsync<BnVertexViewModel>(@"D:\Data\SCC\NNpHSCC\NNpH_failure.net"));
                }));

            KocDataReader kocDataReader = new KocDataReader();
            window.VertexValuesByYear = kocDataReader.ReadVertexValuesForAllYears();
            window.SelectedVertexValues = window.VertexValuesByYear.First().Value;
            kocDataReader.ReadVertexInputsForAllYears(window.InputManager);

            window.Model.Graphs = window.Graphs;
            window.Model.StartYear = window.StartYear;
            window.Model.EndYear = window.EndYear;

            window.SensorListener.NewEvidenceAvailable += SensorListener_NewEvidenceAvailable;
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