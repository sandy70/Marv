using System.Windows.Input;
using System.Windows.Interactivity;
using MapControl;
using Marv;

namespace Marv.Controls.Map
{
    internal class MapPoylineBehavior : Behavior<MapPolyline>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDown += this.AssociatedObject_MouseDown;
            this.AssociatedObject.TouchDown += this.AssociatedObject_TouchDown;
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // We need this to raise the event on the templated parent.
            // It should be raised automatically but isn't. This is probably a bug.

            var mapItemsControl = this.AssociatedObject.FindParent<MapItemsControl>();

            if (mapItemsControl != null)
            {
                mapItemsControl.RaiseEvent(e);
            }
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            // We need this to raise the event on the templated parent.
            // It should be raised automatically but isn't. This is probably a bug.

            var mapItemsControl = this.AssociatedObject.FindParent<MapItemsControl>();

            if (mapItemsControl != null)
            {
                mapItemsControl.RaiseEvent(e);
            }
        }
    }
}