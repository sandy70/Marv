using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Marv.Common
{
    public static partial class Extensions
    {
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

        public static void Push<T>(this ObservableCollection<T> collection, T item)
        {
            collection.Insert(0, item);
        }

        public static string String(this IEnumerable<string> strings)
        {
            var str = "";

            if (strings.Count() == 0)
            {
                return str;
            }
            else if (strings.Count() == 1)
            {
                str += strings.First();
                return str;
            }

            foreach (var s in strings.AllButLast())
            {
                str += s + ",";
            }

            str += strings.Last();

            return str;
        }

        public static string String(this Dictionary<string, Point> positionsByGroup)
        {
            var str = "";

            if (positionsByGroup.Count == 0)
            {
                return str;
            }
            else if (positionsByGroup.Count == 1)
            {
                var kvpFirst = positionsByGroup.First();
                str += kvpFirst.Key + "," + kvpFirst.Value.X + "," + kvpFirst.Value.Y;
                return str;
            }

            foreach (var kvp in positionsByGroup.AllButLast())
            {
                str += kvp.Key + "," + kvp.Value.X + "," + kvp.Value.Y + ",";
            }

            var kvpLast = positionsByGroup.Last();
            str += kvpLast.Key + "," + kvpLast.Value.X + "," + kvpLast.Value.Y;

            return str;
        }

        public static IEnumerable<string> Trimmed(this IEnumerable<string> untrimmed)
        {
            return untrimmed.Select(x => x.Trim());
        }
    }
}

namespace System.Windows
{
    public delegate void RoutedEventHandler<TArgs>(object sender, TArgs e) where TArgs : RoutedEventArgs;
}