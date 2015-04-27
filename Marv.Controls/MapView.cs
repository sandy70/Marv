using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MapControl;
using MapControl.Caching;
using Marv.Common;
using Location = Marv.Common.Location;

namespace Marv.Controls
{
    public class MapView : Map
    {
        public static readonly DependencyProperty StartBoundsProperty =
            DependencyProperty.Register("StartBounds", typeof (LocationRect), typeof (MapView), new PropertyMetadata(null));

        private int discreteZoomLevel = 100;
        private Location previousCenter;

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

            set { this.ZoomToBounds(value.SouthWest, value.NorthEast); }
        }

        public LocationRect StartBounds
        {
            get { return (LocationRect) this.GetValue(StartBoundsProperty); }
            set { this.SetValue(StartBoundsProperty, value); }
        }

        public MapView()
        {
            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            this.Loaded -= this.MapView_Loaded;
            this.Loaded += this.MapView_Loaded;

            this.ViewportChanged -= this.MapView_ViewportChanged;
            this.ViewportChanged += this.MapView_ViewportChanged;
        }

        public bool Contains(IEnumerable<Location> locations)
        {
            return locations.All(this.Bounds.Contains);
        }

        public bool Contains(Location location)
        {
            return this.Bounds.Contains(location);
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
                        P1 = this.LocationToViewportPoint(thisLocation),
                        P2 = this.LocationToViewportPoint(nextLocation)
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

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        private void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            this.previousCenter = this.Center;

            if (this.StartBounds != null)
            {
                this.Bounds = this.StartBounds;
            }
        }

        private void MapView_ViewportChanged(object sender, EventArgs e)
        {
            var zl = (int) Math.Floor(this.ZoomLevel);

            if (zl != this.discreteZoomLevel)
            {
                this.discreteZoomLevel = zl;
                this.RaiseZoomLevelChanged(this.discreteZoomLevel);
            }

            var rect = new LocationRect
            {
                NorthWest = Common.Utils.Mid(this.Center, this.Bounds.NorthWest),
                SouthEast = Common.Utils.Mid(this.Center, this.Bounds.SouthEast)
            };

            if (this.previousCenter == null)
            {
                this.previousCenter = this.Center;
            }

            if (!rect.Contains(this.previousCenter))
            {
                this.previousCenter = this.Center;
                this.RaiseViewportMoved(this.previousCenter);
            }
        }

        private void RaiseViewportMoved(Location location)
        {
            if (this.ViewportMoved != null)
            {
                this.ViewportMoved(this, location);
            }
        }

        private void RaiseZoomLevelChanged(int zoom)
        {
            if (this.ZoomLevelChanged != null)
            {
                this.ZoomLevelChanged(this, zoom);
            }
        }

        public event EventHandler<Location> ViewportMoved;

        public event EventHandler<int> ZoomLevelChanged;
    }
}