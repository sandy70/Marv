﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Marv.Common;

namespace Marv.Controls
{
    public static class Extensions
    {
        public static IEnumerable<T> GetChildren<T>(this DependencyObject depObj) where T : class
        {
            if (depObj != null)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (child is T)
                    {
                        yield return child as T;
                    }

                    foreach (var childOfChild in GetChildren<T>(child as DependencyObject))
                    {
                        yield return childOfChild;
                    }
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
        public static T GetParent<T>(this DependencyObject child) where T : DependencyObject
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
            return GetParent<T>(parentObject);
        }

        public static IEnumerable<Location> ToLocations(this IEnumerable<Point> points, MapView mapView)
        {
            return points.Select<Point, Location>(point => mapView.ViewportPointToLocation(point));
        }

        public static IEnumerable<Point> ToPoints(this IEnumerable<Location> locations, MapView mapView)
        {
            return locations.Select(location => mapView.LocationToViewportPoint(location));
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
        private static DependencyObject GetParentObject(this DependencyObject child)
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
    }
}