using Marv.Common;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace Marv.Controls
{
    public class ProgressBarBehavior : Behavior<RadProgressBar>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.SetValue(sender, e);
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            this.SetValue(sender, e);
        }

        private void SetValue(object sender, MouseEventArgs e)
        {
            var parent = this.AssociatedObject.FindParent<SliderProgressBar>();

            if (parent.IsEditable)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    RadProgressBar progressBar = sender as RadProgressBar;
                    this.AssociatedObject.Value = (e.GetPosition(progressBar).X - 1) / (progressBar.ActualWidth - 2) * 100;
                    e.Handled = true;

                    parent.RaiseEvent(new ValueEventArgs<double>
                    {
                        RoutedEvent = SliderProgressBar.ValueEnteredEvent,
                        Value = this.AssociatedObject.Value
                    });
                }
            }
        }
    }
}