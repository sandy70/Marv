﻿using Marv.Common;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace LibPipeline
{
    internal class MapViewBehavior : Behavior<MapView>
    {
        private int discreteZoomLevel = 100;
        private Location previousCenter = null;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.ViewportChanged += AssociatedObject_ViewportChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var mapControl = this.AssociatedObject;
            this.previousCenter = mapControl.Center;

            if (mapControl.StartExtent != null)
            {
                mapControl.Extent = mapControl.StartExtent;
            }
        }

        private void AssociatedObject_ViewportChanged(object sender, EventArgs e)
        {
            var mapView = this.AssociatedObject;

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

            var rect = new LocationRect
            {
                NorthWest = Utils.Mid(mapView.Center, mapView.Extent.NorthWest),
                SouthEast = Utils.Mid(mapView.Center, mapView.Extent.SouthEast)
            };

            if (this.previousCenter == null)
            {
                this.previousCenter = mapView.Center;
            }

            if (!this.previousCenter.IsWithin(rect))
            {
                this.previousCenter = mapView.Center;

                mapView.RaiseEvent(new ValueEventArgs<Location>
                {
                    RoutedEvent = MapView.ViewportMovedEvent,
                    Value = this.previousCenter
                });
            }
        }
    }
}