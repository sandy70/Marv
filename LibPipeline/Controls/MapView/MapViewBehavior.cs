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
    class MapViewBehavior : Behavior<MapView>
    {
        private int discreteZoomLevel = 100;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.ViewportChanged += AssociatedObject_ViewportChanged;
        }

        private void AssociatedObject_ViewportChanged(object sender, EventArgs e)
        {
            Console.WriteLine(this.AssociatedObject.Extent);

            int zl = (int)Math.Floor(this.AssociatedObject.ZoomLevel);

            if (zl != this.discreteZoomLevel)
            {
                this.discreteZoomLevel = zl;

                this.AssociatedObject.RaiseEvent(new ValueEventArgs<int>
                {
                    RoutedEvent = MapView.ZoomLevelChangedEvent,
                    Value = this.discreteZoomLevel
                });
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var mapControl = this.AssociatedObject;

            if (mapControl.StartExtent != null)
            {
                mapControl.Extent = mapControl.StartExtent;
            }
        }
    }
}
