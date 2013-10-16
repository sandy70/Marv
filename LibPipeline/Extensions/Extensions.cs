using Marv.Common;
using MoreLinq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LibPipeline
{
    public static partial class Extensions
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static IEnumerable<T> FindChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static Graph GetGraph(this IEnumerable<Graph> graphs, string name)
        {
            return graphs.SingleOrDefault(x => x.Name.Equals(name));
        }

        public static IEnumerable<Location> Reduce(this IEnumerable<Location> locations, MapView mapView, double tolerance = 10, Dictionary<Location, Point> viewportPointCache = null, int level = 0)
        {
            if (locations.Count() <= 2)
            {
                return locations;
            }
            else
            {
                var nLocations = locations.Count();

                if (viewportPointCache == null)
                {
                    viewportPointCache = new Dictionary<Location, Point>();

                    // we need to build the viewportPointCache
                    foreach (var location in locations)
                    {
                        viewportPointCache[location] = mapView.LocationToViewportPoint(location.ToMapControlLocation());
                    }
                }

                var first = locations.First();
                var last = locations.Last();

                var maxDistance = double.MinValue;
                var maxDistanceLocation = first;

                foreach (var location in locations)
                {
                    var distance = Utils.Distance(viewportPointCache[first],
                                                  viewportPointCache[last],
                                                  viewportPointCache[location]);

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxDistanceLocation = location;
                    }
                }

                logger.Debug("level: {0}, nLocations: {1}, maxDistance {2}", level, nLocations, maxDistance);

                if (maxDistance > tolerance)
                {
                    return locations.TakeUntil(x => x == maxDistanceLocation)
                                    .Reduce(mapView, tolerance, viewportPointCache, level + 1)
                                    .Concat(locations.SkipWhile(x => x != maxDistanceLocation)
                                                     .Reduce(mapView, tolerance, viewportPointCache, level + 1)
                                                     .Skip(1));
                }
                else
                {
                    return locations.Take(1)
                                    .Concat(last);
                }
            }
        }

        public static MapControl.Location ToMapControlLocation(this Location location)
        {
            return new MapControl.Location { Latitude = location.Latitude, Longitude = location.Longitude };
        }
    }
}