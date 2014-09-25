using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv
{
    public static partial class Extensions
    {
        public static void AddUnique<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        public static void AddUnique<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.AddUnique(item);
            }
        }

        public static IEnumerable<T> AllButLast<T>(this IEnumerable<T> items)
        {
            var it = items.GetEnumerator();
            var hasRemainingItems = false;
            var isFirst = true;
            var item = default(T);

            do
            {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems)
                {
                    if (!isFirst)
                    {
                        yield return item;
                    }
                    item = it.Current;
                    isFirst = false;
                }
            }
            while (hasRemainingItems);
        }

        public static IEnumerable<T> AllButLastN<T>(this IEnumerable<T> items, int n)
        {
            var it = items.GetEnumerator();
            var hasRemainingItems = false;
            var cache = new Queue<T>(n + 1);

            do
            {
                if (hasRemainingItems = it.MoveNext())
                {
                    cache.Enqueue(it.Current);
                    if (cache.Count > n)
                    {
                        yield return cache.Dequeue();
                    }
                }
            }
            while (hasRemainingItems);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T item)
        {
            return items.Except(item.Yield());
        }

        public static T FirstOrNew<T>(this ICollection<T> items, Func<T, bool> predicate)
        {
            var firstOrDefault = items.FirstOrDefault();

            if (firstOrDefault == null)
            {
                firstOrDefault = Utils.Create<T>();
                items.Add(firstOrDefault);
            }

            return firstOrDefault;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in items)
            {
                action(item, i++);
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    return retVal;
                }
                retVal++;
            }
            return -1;
        }

        public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            var maxIndex = -1;
            var maxValue = default(T); // Immediately overwritten anyway

            var index = 0;
            foreach (var value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

        public static void Remove<T>(this ICollection<T> items, Func<T, bool> predicate)
        {
            var itemsToRemove = items.Where(predicate).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                items.Remove(itemToRemove);
            }
        }

        public static void Remove<T>(this IEnumerable<ICollection<T>> collections, Func<T, bool> predicate)
        {
            foreach (var collection in collections)
            {
                collection.Remove(predicate);
            }
        }

        public static bool Replace<T>(this IList<T> items, T oldItem, T newItem)
        {
            var oldItemIndex = items.IndexOf(oldItem);
            var result = items.Remove(oldItem);

            if (result)
            {
                items.Insert(oldItemIndex, newItem);
            }

            return result;
        }

        public static Dict<TKey, TElement> ToDict<TSource, TKey, TElement>(this IEnumerable<TSource> items, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var dict = new Dict<TKey, TElement>();

            items.ForEach((item, i) => dict.Add(keySelector(item), elementSelector(item)));

            return dict;
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}