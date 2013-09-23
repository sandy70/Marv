using Smile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace LibNetwork
{
    public static class Extensions
    {
        public static IEnumerable<T> AllButLast<T>(this IEnumerable<T> source)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            bool isFirst = true;
            T item = default(T);

            do
            {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems)
                {
                    if (!isFirst) yield return item;
                    item = it.Current;
                    isFirst = false;
                }
            } while (hasRemainingItems);
        }

        public static IEnumerable<T> AllButLastN<T>(this IEnumerable<T> source, int n)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            var cache = new Queue<T>(n + 1);

            do
            {
                if (hasRemainingItems = it.MoveNext())
                {
                    cache.Enqueue(it.Current);
                    if (cache.Count > n)
                        yield return cache.Dequeue();
                }
            } while (hasRemainingItems);
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