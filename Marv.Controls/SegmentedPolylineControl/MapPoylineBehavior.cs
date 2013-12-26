using MapControl;
using Marv.Common;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    internal class MapPoylineBehavior : Behavior<MapPolyline>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            this.AssociatedObject.TouchDown += AssociatedObject_TouchDown;
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