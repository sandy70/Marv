using MapControl;
using Marv.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;

namespace LibPipeline
{
    public class MapView : Map
    {
        public static readonly DependencyProperty ExtentProperty =
        DependencyProperty.Register("Extent", typeof(LocationRect), typeof(MapView), new PropertyMetadata(null));

        public static readonly DependencyProperty StartExtentProperty =
        DependencyProperty.Register("StartExtent", typeof(LocationRect), typeof(MapView), new PropertyMetadata(null));

        public static readonly RoutedEvent ViewportMovedEvent =
        EventManager.RegisterRoutedEvent("ViewportMoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<Location>>), typeof(MapView));

        public static readonly RoutedEvent ZoomLevelChangedEvent =
        EventManager.RegisterRoutedEvent("ZoomLevelChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<int>>), typeof(MapView));

        public MapView()
            : base()
        {
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Add(new MapViewBehavior());
        }

        public event RoutedEventHandler<ValueEventArgs<Location>> ViewportMoved
        {
            add { AddHandler(ViewportMovedEvent, value); }
            remove { RemoveHandler(ViewportMovedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<int>> ZoomLevelChanged
        {
            add { AddHandler(ZoomLevelChangedEvent, value); }
            remove { RemoveHandler(ZoomLevelChangedEvent, value); }
        }

        public LocationRect Extent
        {
            get
            {
                LocationRect rect = new LocationRect();

                rect.NorthWest = this.ViewportPointToLocation(new Point { X = 0, Y = 0 });
                rect.SouthEast = this.ViewportPointToLocation(new Point { X = this.RenderSize.Width, Y = this.RenderSize.Height });

                return rect;
            }

            set
            {
                this.ZoomToExtent(south: value.South,
                                  west: value.West,
                                  north: value.North,
                                  east: value.East);
            }
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
                points.Add(this.LocationToViewportPoint(location.ToMapControlLocation()));
            }

            return points;
        }

        public List<Location> ViewportPointsToILocations(IEnumerable<Point> points)
        {
            var locations = new List<Location>();

            foreach (var point in points)
            {
                locations.Add(this.ViewportPointToLocation(point));
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

        protected override void OnManipulationInertiaStarting(System.Windows.Input.ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior.DesiredDeceleration = 0.001;
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