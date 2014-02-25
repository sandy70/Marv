using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Map;

namespace Marv.Controls.Map
{
    public class MapView : MapControl.Map
    {
        public static readonly DependencyProperty ExtentProperty =
            DependencyProperty.Register("Extent", typeof (LocationRect), typeof (MapView), new PropertyMetadata(null));

        public static readonly DependencyProperty StartExtentProperty =
            DependencyProperty.Register("StartExtent", typeof (LocationRect), typeof (MapView), new PropertyMetadata(null));

        public static readonly RoutedEvent ViewportMovedEvent =
            EventManager.RegisterRoutedEvent("ViewportMoved", RoutingStrategy.Bubble, typeof (RoutedEventHandler<ValueEventArgs<Location>>), typeof (MapView));

        public static readonly RoutedEvent ZoomLevelChangedEvent =
            EventManager.RegisterRoutedEvent("ZoomLevelChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler<ValueEventArgs<int>>), typeof (MapView));

        public MapView()
        {
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Add(new MapViewBehavior());
        }

        public LocationRect Extent
        {
            get
            {
                var rect = new LocationRect
                {
                    NorthWest = this.ViewportPointToLocation(new Point
                    {
                        X = 0,
                        Y = 0
                    }),
                    SouthEast = this.ViewportPointToLocation(new Point
                    {
                        X = this.RenderSize.Width,
                        Y = this.RenderSize.Height
                    })
                };

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
            get
            {
                return (LocationRect) this.GetValue(StartExtentProperty);
            }
            set
            {
                this.SetValue(StartExtentProperty, value);
            }
        }

        public event RoutedEventHandler<ValueEventArgs<Location>> ViewportMoved
        {
            add
            {
                this.AddHandler(ViewportMovedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ViewportMovedEvent, value);
            }
        }

        public event RoutedEventHandler<ValueEventArgs<int>> ZoomLevelChanged
        {
            add
            {
                this.AddHandler(ZoomLevelChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ZoomLevelChangedEvent, value);
            }
        }

        /// <summary>
        ///     Zoom to most appropriate level to encompass the given rectangle
        /// </summary>
        /// <param name="north"></param>
        /// <param name="east"></param>
        /// <param name="south"></param>
        /// <param name="west"></param>
        public void ZoomToExtent(double north, double east, double south, double west)
        {
            var zoom = this.GetBoundsZoomLevel(north, east, south, west);
            var cx = west + (east - west)/2;
            var cy = south + (north - south)/2;

            zoom = Math.Floor(zoom) - 1;

            this.TargetCenter = new MapControl.Location(cy, cx);
            this.TargetZoomLevel = zoom;
        }

        /// <summary>
        ///     Zoom to most appropriate level to encompass the given rectangle
        /// </summary>
        /// <param name="bottomLeft"></param>
        /// <param name="topRight"></param>
        public void ZoomToExtent(Location bottomLeft, Location topRight)
        {
            this.ZoomToExtent(topRight.Latitude, topRight.Longitude, topRight.Latitude, bottomLeft.Longitude);
        }

        public void ZoomTo(LocationRect rect)
        {
            this.ZoomToExtent(rect.North, rect.East, rect.South, rect.West);
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        /// <summary>
        ///     Calculates a suitable zoom level given a boundary.
        /// </summary>
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

            var latZoomLevel = Math.Floor(Math.Log(this.RenderSize.Height*360/latAngle/GLOBE_HEIGHT)/Math.Log(2));
            var lngZoomLevel = Math.Floor(Math.Log(this.RenderSize.Width*360/lngAngle/GLOBE_WIDTH)/Math.Log(2)); //0.6931471805599453

            return (latZoomLevel < lngZoomLevel) ? latZoomLevel : lngZoomLevel;
        }
    }
}