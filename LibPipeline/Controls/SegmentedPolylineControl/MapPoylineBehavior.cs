using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace LibPipeline
{
    class MapPoylineBehavior : Behavior<MapPolyline>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
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
    }
}
