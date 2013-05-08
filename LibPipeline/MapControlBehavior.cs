using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows;

namespace LibPipeline
{
    class MapControlBehavior : Behavior<MapControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var mapControl = this.AssociatedObject;

            if (mapControl.StartExtent != null)
            {
                mapControl.ZoomToExtent(north: mapControl.StartExtent.North,
                    west: mapControl.StartExtent.West,
                    south: mapControl.StartExtent.South,
                    east: mapControl.StartExtent.East);
            }
        }
    }
}
