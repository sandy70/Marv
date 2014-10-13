﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Marv.Controls
{
    public static class Extensions
    {
        public static IEnumerable<T> FindChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                yield break;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T)
                {
                    yield return (T) child;
                }

                foreach (var childOfChild in FindChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        /// <summary>
        ///     Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">
        ///     A direct or indirect child of the
        ///     queried item.
        /// </param>
        /// <returns>
        ///     The first parent item that matches the submitted
        ///     type parameter. If not matching item can be found, a null
        ///     reference is being returned.
        /// </returns>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            //use recursion to proceed with next level
            return FindParent<T>(parentObject);
        }

        /// <summary>
        ///     This method is an alternative to WPF's
        ///     <see cref="VisualTreeHelper.GetParent" /> method, which also
        ///     supports content elements. Keep in mind that for content element,
        ///     this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>
        ///     The submitted item's parent, if available. Otherwise
        ///     null.
        /// </returns>
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null)
            {
                return null;
            }

            // handle content elements separately
            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                {
                    return parent;
                }

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            // also try searching for parent in framework elements (such as DockPanel, etc)
            var frameworkElement = child as FrameworkElement;

            if (frameworkElement != null)
            {
                var parent = frameworkElement.Parent;
                if (parent != null)
                {
                    return parent;
                }
            }

            // if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        public static Point GetOffset(this Rect viewport, Rect bounds, double pad = 0)
        {
            var point = new Point();

            var left = bounds.Left - viewport.Left;
            var right = bounds.Right - viewport.Right;
            var top = bounds.Top - viewport.Top;
            var bottom = bounds.Bottom - viewport.Bottom;

            if (left > 0 && right > 0)
            {
                point.X = right + pad;
            }
            else if (left < 0 && right < 0)
            {
                point.X = left - pad;
            }

            if (top < 0 && bottom < 0)
            {
                point.Y = top - pad;
            }
            else if (top > 0 && bottom > 0)
            {
                point.Y = bottom + pad;
            }

            return point;
        }

        public static IEnumerable<Point> Reduce(this IEnumerable<Point> points, double tolerance = 10)
        {
            var pointList = points as IList<Point> ?? points.ToList();

            if (pointList.Count <= 2)
            {
                return pointList;
            }

            var first = pointList.First();
            var last = pointList.Last();

            var maxDistance = double.MinValue;
            var maxDistancePoint = first;

            foreach (var point in pointList)
            {
                var distance = Utils.Distance(first, last, point);

                if (distance < maxDistance)
                {
                    continue;
                }

                maxDistance = distance;
                maxDistancePoint = point;
            }

            if (maxDistance > tolerance)
            {
                return pointList.TakeUntil(x => x == maxDistancePoint)
                                .Reduce(tolerance)
                                .Concat(pointList.SkipWhile(x => x != maxDistancePoint)
                                                 .Reduce(tolerance)
                                                 .Skip(1));
            }
            return pointList.Take(1)
                            .Concat(last);
        }


    }
}

namespace System.Windows
{
    public delegate void RoutedEventHandler<in TArgs>(object sender, TArgs e) where TArgs : RoutedEventArgs;
}