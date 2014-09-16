using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Marv
{
    public static partial class Extensions
    {
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
        }

        public static string Dequote(this string str, char startChar, char endChar)
        {
            var nChars = str.Count();
            var startIndex = 0;
            var length = nChars;

            if (nChars > 0)
            {
                if (str[0] == startChar)
                {
                    startIndex = 1;
                    length -= 1;
                }

                if (str[nChars - 1] == endChar)
                {
                    length -= 1;
                }
            }

            return str.Substring(startIndex, length);
        }

        public static string Dequote(this string str, char padChar)
        {
            return str.Dequote(padChar, padChar);
        }

        public static string Dequote(this string str)
        {
            return str.Dequote('"');
        }

        public static string Enquote(this string str, char startChar, char endChar)
        {
            return startChar + str + endChar;
        }

        public static string Enquote(this string str, char padChar)
        {
            return str.Enquote(padChar, padChar);
        }

        public static string Enquote(this string str)
        {
            return str.Enquote('"');
        }

        public static double Entropy(this double[] array)
        {
            return array.Where(value => value > 0).Sum(value => value * Math.Log(value)) / Math.Log(array.Length);
        }

        public static IEnumerable<T> FindChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

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
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            //use recursion to proceed with next level
            return FindParent<T>(parentObject);
        }

        public static Point GetOffset(this Rect viewport, Rect bounds, double pad = 0)
        {
            var point = new Point();

            var left = bounds.Left - viewport.Left;
            var right = bounds.Right - viewport.Right;
            var top = bounds.Top - viewport.Top;
            var bottom = bounds.Bottom - viewport.Bottom;

            if (left > 0 && right > 0) point.X = right + pad;
            else if (left < 0 && right < 0) point.X = left - pad;

            if (top < 0 && bottom < 0) point.Y = top - pad;
            else if (top > 0 && bottom > 0) point.Y = bottom + pad;

            return point;
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
            if (child == null) return null;

            // handle content elements separately
            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            // also try searching for parent in framework elements (such as DockPanel, etc)
            var frameworkElement = child as FrameworkElement;

            if (frameworkElement != null)
            {
                var parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            // if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        public static Color NextColor(this Random random)
        {
            return Color.FromScRgb(1.0f, (float) random.NextDouble(), (float) random.NextDouble(), (float) random.NextDouble());
        }

        public static Dictionary<string, double> Normalized(this Dictionary<string, double> evidence)
        {
            if (evidence == null) return null;

            var normalized = new Dictionary<string, double>();
            var sum = evidence.Sum(kvp => kvp.Value);

            foreach (var key in evidence.Keys)
            {
                if (sum == 0)
                {
                    normalized[key] = 0;
                }
                else
                {
                    normalized[key] = evidence[key] / sum;
                }
            }

            return normalized;
        }

        public static IEnumerable<double> Normalized(this IEnumerable<double> values)
        {
            var valueList = values as IList<double> ?? values.ToList();
            var sum = valueList.Sum();

            for (var i = 0; i < valueList.Count; i++)
            {
                valueList[i] /= sum;
            }

            return valueList;
        }

        public static List<string> ParseBlocks(this string str)
        {
            var startChar = '{';
            var endChar = '}';
            var count = 0;
            var readStr = "";
            var blocks = new List<string>();
            var isReading = false;

            foreach (var c in str.Trim())
            {
                if (c == startChar)
                {
                    count++;
                    isReading = true;
                }

                if (c == endChar)
                {
                    count--;
                }

                if (isReading)
                {
                    readStr += c;
                }

                if (count == 0 && isReading)
                {
                    blocks.Add(readStr);
                    readStr = "";
                    isReading = false;
                }
            }

            return blocks;
        }

        public static T ParseJson<T>(this string _string)
        {
            return JsonConvert.DeserializeObject<T>(_string);
        }

        public static KeyValuePair<string, string> ParseKeyValue(this string str)
        {
            var key = "";
            var readString = "";

            foreach (var c in str.Trim())
            {
                if (c == ',')
                {
                    key = readString;
                    readString = "";
                }
                else
                {
                    readString += c;
                }
            }

            return new KeyValuePair<string, string>(key, readString);
        }

        public static void Push<T>(this Collection<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Insert(0, item);
            }
        }

        public static void Push(this ObservableCollection<INotification> notifications, INotification notification)
        {
            if (!notifications.Contains(notification) && (!notification.IsMuteable || !notification.IsMuted))
            {
                notifications.Insert(0, notification);
            }
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

                if (distance < maxDistance) continue;

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

        public static string String<T>(this IEnumerable<T> items, string format = "{0}")
        {
            return items.Select(item => System.String.Format(format, item)).String();
        }

        public static string String(this IEnumerable<string> strings)
        {
            var str = "";

            var stringList = strings as IList<string> ?? strings.ToList();

            if (stringList.Count == 0)
            {
                return str;
            }

            if (stringList.Count == 1)
            {
                str += stringList.First();
                return str;
            }

            str = stringList.AllButLast().Aggregate(str, (current, s) => current + s + ",");

            str += stringList.Last();

            return str;
        }

        public static string String<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary, string format = "{0}")
        {
            return dictionary.Select(kvp => System.String.Format(format, kvp.Value)).String();
        }

        public static string String(this Dictionary<string, Point> positionsByGroup)
        {
            var str = "";

            if (positionsByGroup.Count == 0)
            {
                return str;
            }
            if (positionsByGroup.Count == 1)
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

        public static TValue[] ToArray<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary.Values.ToArray();
        }

        public static string ToJson(this object _object)
        {
            return JsonConvert.SerializeObject(_object);
        }

        public static IEnumerable<string> Trimmed(this IEnumerable<string> untrimmed)
        {
            return untrimmed.Select(x => x.Trim());
        }

        public static void WriteBson(this object _object, string fileName, Formatting formatting = Formatting.Indented)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = formatting,
                TypeNameHandling = TypeNameHandling.Auto
            };

            using (var jsonTextWriter = new BsonWriter(File.Open(fileName, FileMode.Create)))
            {
                serializer.Serialize(jsonTextWriter, _object);
            }
        }

        public static void WriteJson(this object _object, string fileName, Formatting formatting = Formatting.Indented)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = formatting,
                TypeNameHandling = TypeNameHandling.Auto
            };

            using (var streamWriter = new StreamWriter(fileName))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonTextWriter, _object);
                }
            }
        }
    }
}

namespace System.Windows
{
    public delegate void RoutedEventHandler<in TArgs>(object sender, TArgs e) where TArgs : RoutedEventArgs;
}