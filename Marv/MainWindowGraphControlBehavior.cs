using LibBn;
using LibPipeline;
using System;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Marv
{
    internal class MainWindowGraphControlBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.GraphControl.NewEvidenceAvailable += GraphControl_NewEvidenceAvailable;
            window.GraphControl.RetractButtonClicked += GraphControl_RetractButtonClicked;
            window.GraphControl.SensorButtonChecked += GraphControl_SensorButtonChecked;
            window.GraphControl.SensorButtonUnchecked += GraphControl_SensorButtonUnchecked;
        }

        private void GraphControl_NewEvidenceAvailable(object sender, ValueEventArgs<BnVertexViewModel> e)
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

        private void GraphControl_RetractButtonClicked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            window.RemoveInput(e.Value);

            var success = window.TryUpdateNetwork();

            if (!success)
            {
                window.PopupControl.ShowText("Inconsistent evidence entered.");
            }
        }

        private void GraphControl_SensorButtonChecked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            try
            {
                this.AssociatedObject.SensorListener.Start(e.Value);
            }
            catch (IOException exp)
            {
                this.AssociatedObject.SensorListener.Stop();
                this.AssociatedObject.PopupControl.ShowText("Unable to open serial port. Connect receiver.");
            }
        }

        private void GraphControl_SensorButtonUnchecked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            this.AssociatedObject.SensorListener.Stop();
        }
    }
}