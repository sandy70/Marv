using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Caching;
using MapControl;
using Marv.Common;
using Marv.Map;
using Location = Marv.Common.Location;

namespace Marv.Controls.Map
{
    public class MapView : MapControl.Map
    {
        public static readonly DependencyProperty StartBoundsProperty =
            DependencyProperty.Register("StartBounds", typeof (LocationRect), typeof (MapView), new PropertyMetadata(null));

        public LocationRect Bounds
        {
            get
            {
                return new LocationRect
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
            }

            set { this.ZoomToBounds(value.SouthWest.ToMapControlLocation(), value.NorthEast.ToMapControlLocation()); }
        }

        public LocationRect StartBounds
        {
            get { return (LocationRect) this.GetValue(StartBoundsProperty); }
            set { this.SetValue(StartBoundsProperty, value); }
        }

        public MapView()
        {
            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, "./");
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Add(new MapViewBehavior());
        }

        public bool Contains(IEnumerable<Location> locations)
        {
            return locations.All(location => this.Bounds.Contains(location));
        }

        public IEnumerable<Location> Intersection(IList<Location> locations)
        {
            var cornerPoints = new List<Point>(this.Bounds.ToPoints(this));
            cornerPoints.Add(cornerPoints[0]); // close the polygon

            return locations
                .AllButLast()
                .Where((location, i) =>
                {
                    var thisLocation = location;
                    var nextLocation = locations[i + 1];

                    if (this.Bounds.Contains(thisLocation) || this.Bounds.Contains(nextLocation))
                    {
                        return true;
                    }

                    var isIntersection = false;

                    var line2 = new LineSegment
                    {
                        P1 = this.LocationToViewportPoint(thisLocation.ToMapControlLocation()),
                        P2 = this.LocationToViewportPoint(nextLocation.ToMapControlLocation())
                    };

                    cornerPoints.AllButLast().ForEach((cornerPoint, k) =>
                    {
                        var line1 = new LineSegment
                        {
                            P1 = cornerPoint,
                            P2 = cornerPoints[k + 1]
                        };

                        isIntersection = Common.Utils.Intersection(line1, line2) != null;
                    });

                    return isIntersection;
                });
        }

        public void RaiseViewportMoved(Location location)
        {
            if (this.ViewportMoved != null)
            {
                this.ViewportMoved(this, location);
            }
        }

        public void RaiseZoomLevelChanged(int zoom)
        {
            if (this.ZoomLevelChanged != null)
            {
                this.ZoomLevelChanged(this, zoom);
            }
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        public event EventHandler<Location> ViewportMoved;

        public event EventHandler<int> ZoomLevelChanged;
    }
}