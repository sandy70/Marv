using System;
using System.Windows;
using System.Windows.Interactivity;
using Marv.Map;

namespace Marv.Controls.Map
{
    internal class MapViewBehavior : Behavior<MapView>
    {
        private int discreteZoomLevel = 100;
        private Location previousCenter;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
            this.AssociatedObject.ViewportChanged += this.AssociatedObject_ViewportChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var mapControl = this.AssociatedObject;
            this.previousCenter = mapControl.Center;

            if (mapControl.StartBounds != null)
            {
                mapControl.Bounds = mapControl.StartBounds;
            }
        }

        private void AssociatedObject_ViewportChanged(object sender, EventArgs e)
        {
            var mapView = this.AssociatedObject;

            var zl = (int) Math.Floor(this.AssociatedObject.ZoomLevel);

            if (zl != this.discreteZoomLevel)
            {
                this.discreteZoomLevel = zl;
                this.AssociatedObject.RaiseZoomLevelChanged(this.discreteZoomLevel);
            }

            var rect = new LocationRect
            {
                NorthWest = Marv.Map.Utils.Mid(mapView.Center, mapView.Bounds.NorthWest),
                SouthEast = Marv.Map.Utils.Mid(mapView.Center, mapView.Bounds.SouthEast)
            };

            if (this.previousCenter == null)
            {
                this.previousCenter = mapView.Center;
            }

            if (!rect.Contains(this.previousCenter))
            {
                this.previousCenter = mapView.Center;
                mapView.RaiseViewportMoved(this.previousCenter);
            }
        }
    }
}