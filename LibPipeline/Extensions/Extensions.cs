using LibBn;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LibPipeline
{
    public static partial class Extensions
    {
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

        public static BnGraph GetGraph(this IEnumerable<BnGraph> graphs, string name)
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

        public static IEnumerable<IPoint> Reduce(this IEnumerable<IPoint> points, double tolerance = 10)
        {
            var nPoints = points.Count();

            // If points are null or too few then return
            if (points == null || nPoints < 3)
            {
                return points;
            }

            var first = points.First();
            var last = points.Last();

            var maxDistance = double.MinValue;
            var maxDistancePoint = first;

            foreach (var point in points)
            {
                var distance = Utils.Distance(first, last, point);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistancePoint = point;
                }
            }

            if (maxDistance > tolerance)
            {
                return points.TakeUntil(x => x == maxDistancePoint)
                             .Reduce(tolerance)
                             .Concat(points.SkipWhile(x => x != maxDistancePoint)
                                           .Reduce(tolerance)
                                           .Skip(1));

            }
            else
            {
                first.Value = Math.Max(points.Take(nPoints / 2).Max(point => point.Value), first.Value);
                last.Value = Math.Max(points.Skip(nPoints / 2).Max(point => point.Value), last.Value);

                return points.Take(1).Concat(last);
            }
        }

        public static LibPipeline.Location ToLibPipelineLocation(this MapControl.Location location)
        {
            return new LibPipeline.Location { Latitude = location.Latitude, Longitude = location.Longitude };
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