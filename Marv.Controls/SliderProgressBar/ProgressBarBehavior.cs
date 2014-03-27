using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Controls
{
    public class ProgressBarBehavior : Behavior<RadProgressBar>
    {
        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.SetValue(sender, e);
            }
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetValue(sender, e);
                e.Handled = true;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDown += this.AssociatedObject_MouseDown;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }

        private void SetValue(object sender, MouseEventArgs e)
        {
            var parent = this.AssociatedObject.FindParent<SliderProgressBar>();

            if (!parent.IsEditable) return;

            var progressBar = sender as RadProgressBar;
            this.AssociatedObject.Value = (e.GetPosition(progressBar).X - 1)/(progressBar.ActualWidth - 2)*100;

            parent.RaiseEvent(new ValueEventArgs<double>
            {
                RoutedEvent = SliderProgressBar.ValueEnteredEvent,
                Value = this.AssociatedObject.Value
            });
        }
    }
}