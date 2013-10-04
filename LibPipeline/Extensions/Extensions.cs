using LibNetwork;
using MoreLinq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return FindParent<T>(parentObject);
            }
        }

        public static Graph GetGraph(this IEnumerable<Graph> graphs, string name)
        {
            return graphs.SingleOrDefault(x => x.Name.Equals(name));
        }

        public static Point GetOffset(this Rect viewport, Rect bounds, double pad = 0)
        {
            var point = new Point();

            double left = bounds.Left - viewport.Left;
            double right = bounds.Right - viewport.Right;
            double top = bounds.Top - viewport.Top;
            double bottom = bounds.Bottom - viewport.Bottom;

            if (left > 0 && right > 0) point.X = right + pad;
            else if (left < 0 && right < 0) point.X = left - pad;

            if (top < 0 && bottom < 0) point.Y = top - pad;
            else if (top > 0 && bottom > 0) point.Y = bottom + pad;

            return point;
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null) return null;

            // handle content elements separately
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            // also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            // if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
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
                    viewportPointCache = new Dictionary<Location,Point>();

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

        public static IEnumerable<Location> ToLocations(this IEnumerable<IPoint> points, MapView mapView)
        {
            return points.Select(point =>
                             {
                                 Location location = mapView.ViewportPointToLocation(new Point { X = point.X, Y = point.Y });
                                 location.Value = point.Value;
 
                                 return location;
                             })
                         .ToList();
        }

        public static MapControl.Location ToMapControlLocation(this Location location)
        {
            return new MapControl.Location { Latitude = location.Latitude, Longitude = location.Longitude };
        }
    }
}

namespace System.Windows
{
    public delegate void RoutedEventHandler<TArgs>(object sender, TArgs e) where TArgs : RoutedEventArgs;
}