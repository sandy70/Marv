using Microsoft.Maps.MapControl.WPF;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace LibPipeline
{
    public class BingMapUpdateBehavior : Behavior<BingMap>
    {
        private bool IsMouseWheel = false;

        public void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.AssociatedObject.Update();
        }

        public void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.IsMouseWheel = true;
        }

        public void AssociatedObject_TouchUp(object sender, TouchEventArgs e)
        {
            this.AssociatedObject.Update();
        }

        public void AssociatedObject_ViewChangeEnd(object sender, MapEventArgs e)
        {
            if (this.IsMouseWheel)
            {
                this.AssociatedObject.Update();
                this.IsMouseWheel = false;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseUp += AssociatedObject_MouseUp;
            this.AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
            this.AssociatedObject.TouchUp += AssociatedObject_TouchUp;
            this.AssociatedObject.ViewChangeEnd += AssociatedObject_ViewChangeEnd;
        }
    }
}