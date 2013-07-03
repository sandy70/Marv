﻿using MapControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Linq;

namespace LibPipeline
{
    public class MapView : Map
    {
        public static readonly DependencyProperty StartExtentProperty =
        DependencyProperty.Register("StartExtent", typeof(LocationRect), typeof(MapView), new PropertyMetadata(null));

        public static readonly RoutedEvent ZoomLevelChangedEvent =
        EventManager.RegisterRoutedEvent("ZoomLevelChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<int>>), typeof(MapView));

        public MapView()
            : base()
        {
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Add(new MapViewBehavior());
        }

        public event RoutedEventHandler<ValueEventArgs<int>> ZoomLevelChanged
        {
            add { AddHandler(ZoomLevelChangedEvent, value); }
            remove { RemoveHandler(ZoomLevelChangedEvent, value); }
        }

        public LocationRect StartExtent
        {
            get { return (LocationRect)GetValue(StartExtentProperty); }
            set { SetValue(StartExtentProperty, value); }
        }

        public List<Point> ILocationsToViewportPoints(IEnumerable<Location> locations)
        {
            var points = new List<Point>();

            foreach (var location in locations)
            {
                points.Add(this.LocationToViewportPoint(location.AsMapControlLocation()));
            }

            return points;
        }

        public List<Location> ViewportPointsToILocations(IEnumerable<Point> points)
        {
            var locations = new List<Location>();

            foreach (var point in points)
            {
                locations.Add(this.ViewportPointToLocation(point).AsLibPipelineLocation());
            }

            return locations;
        }

        /// <summary>
        /// Zoom to most appropriate level to encompass the given rectangle
        /// </summary>
        /// <param name="north"></param>
        /// <param name="east"></param>
        /// <param name="south"></param>
        /// <param name="west"></param>
        public void ZoomToExtent(double north, double east, double south, double west)
        {
            double zoom = GetBoundsZoomLevel(north, east, south, west);
            double cx = west + (east - west) / 2;
            double cy = south + (north - south) / 2;

            zoom = Math.Floor(zoom) - 1;

            TargetCenter = new MapControl.Location(cy, cx);
            TargetZoomLevel = zoom;
        }

        /// <summary>
        /// Zoom to most appropriate level to encompass the given rectangle
        /// </summary>
        /// <param name="bottomLeft"></param>
        /// <param name="topRight"></param>
        public void ZoomToExtent(Location bottomLeft, Location topRight)
        {
            ZoomToExtent(topRight.Latitude, topRight.Longitude, topRight.Latitude, bottomLeft.Longitude);
        }

        public void ZoomToExtent(LocationRect locationRect)
        {
            this.ZoomToExtent(south: locationRect.South,
                west: locationRect.West,
                north: locationRect.North,
                east: locationRect.East);
        }

        /// <summary>
        /// calculates a suitable zoom level given a boundary
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private double GetBoundsZoomLevel(double north, double east, double south, double west)
        {
            var GLOBE_HEIGHT = 256; // Height of a map that displays the entire world when zoomed all the way out
            var GLOBE_WIDTH = 256; // Width of a map that displays the entire world when zoomed all the way out

            var latAngle = north - south;
            if (latAngle < 0)
            {
                latAngle += 360;
            }

            var lngAngle = east - west;

            var latZoomLevel = Math.Floor(Math.Log(RenderSize.Height * 360 / latAngle / GLOBE_HEIGHT) / Math.Log(2));
            var lngZoomLevel = Math.Floor(Math.Log(RenderSize.Width * 360 / lngAngle / GLOBE_WIDTH) / Math.Log(2)); //0.6931471805599453

            return (latZoomLevel < lngZoomLevel) ? latZoomLevel : lngZoomLevel;
        }
    }
}