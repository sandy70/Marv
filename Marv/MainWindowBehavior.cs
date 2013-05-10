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
            this.AssociatedObject.Closed += AssociatedObject_Closed;
            this.AssociatedObject.Closing += AssociatedObject_Closing;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_Closed(object sender, EventArgs e)
        {
            BnGraphWriter.WritePositions(this.AssociatedObject.FileName, this.AssociatedObject.GraphControl.SourceGraph);
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.ProfileLocations = ExcelReader.ReadPropertyLocations<PropertyLocation>(Properties.Settings.Default.ProfileFileName);
            window.TallyLocations = ExcelReader.ReadPropertyLocations<PropertyLocation>(Properties.Settings.Default.TallyFileName);

            window.MapView.ZoomToExtent(window.ProfileLocations.Bounds());

            KocDataReader kocDataReader = new KocDataReader();
            window.VertexValuesByYear = kocDataReader.ReadVertexValuesForAllYears();
            window.SelectedVertexValues = window.VertexValuesByYear.First().Value;
            kocDataReader.ReadVertexInputsForAllYears(window.InputManager);

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