﻿using Microsoft.Maps.MapControl.WPF;
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

        public static void Print(this Point point)
        {
            Console.WriteLine("x: " + point.X + ", y: " + point.Y);
        }

        public static ObservableCollection<Location> WithinViewport(this ObservableCollection<Location> segments, BingMap bingMap)
        {
            ObservableCollection<Location> insideLocations = new ObservableCollection<Location>();
            LocationRect boundingRect = bingMap.BoundingRectangle;

            // Calculate padding. Since distances are small, using lat, lon values works
            double pad = Math.Max(Math.Abs(boundingRect.East - boundingRect.West),
                                  Math.Abs(boundingRect.North - boundingRect.South));

            foreach (var segment in segments)
            {
                if (boundingRect.South - pad <= segment.Latitude && segment.Latitude <= boundingRect.North + pad &&
                   boundingRect.West - pad <= segment.Longitude && segment.Longitude <= boundingRect.East + pad)
                {
                    insideLocations.Add(segment);
                }
            }

            return insideLocations;
        }

        public static List<Location> Corners(this LocationRect locRect)
        {
            var corners = new List<Location>();

            corners.Add(locRect.Northwest);
            corners.Add(locRect.Northeast);
            corners.Add(locRect.Southeast);
            corners.Add(locRect.Southwest);

            return corners;
        }
    }
}

namespace System.Windows
{
    public delegate void RoutedEventHandler<TArgs>(object sender, TArgs e) where TArgs : RoutedEventArgs;
}