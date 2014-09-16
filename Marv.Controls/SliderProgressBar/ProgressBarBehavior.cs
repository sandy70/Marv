using System;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Marv;
using Telerik.Windows.Controls;

namespace Marv.Controls
{
    public class ProgressBarBehavior : Behavior<RadProgressBar>
    {
        private bool isDoubleClicked;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
            this.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            this.AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.isDoubleClicked = true;
            this.AssociatedObject.Value = 100;
            this.RaiseValueEntered();
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isDoubleClicked = false;
            this.SetValue(sender, e);
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !this.isDoubleClicked)
            {
                this.SetValue(sender, e);
                e.Handled = true;
            }
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.isDoubleClicked)
            {
                this.RaiseValueEntered();
            }
        }

        private void RaiseValueEntered()
        {
            var sliderProgressBar = this.AssociatedObject.FindParent<SliderProgressBar>();

            if (sliderProgressBar == null) return;

            sliderProgressBar.RaiseValueEntered();
        }

        private void SetValue(object sender, MouseEventArgs e)
        {
            var parent = this.AssociatedObject.FindParent<SliderProgressBar>();

            if (!parent.IsEditable) return;

            var progressBar = sender as RadProgressBar;
            this.AssociatedObject.Value = (e.GetPosition(progressBar).X - 1) / (progressBar.ActualWidth - 2) * 100;
        }
    }
}