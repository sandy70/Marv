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
            bool hasRemainingItems;
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
            bool hasRemainingItems;
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

        /// <summary>
        ///     Returns a sequence consisting of the head element and the given tail elements.
        /// </summary>
        /// <typeparam name="T">Type of sequence</typeparam>
        /// <param name="head">Head element of the new sequence.</param>
        /// <param name="tail">All elements of the tail. Must not be null.</param>
        /// <returns>A sequence consisting of the head elements and the given tail elements.</returns>
        /// <remarks>This operator uses deferred execution and streams its results.</remarks>
        public static IEnumerable<T> Concat<T>(this T head, IEnumerable<T> tail)
        {
            if (tail == null)
            {
                throw new ArgumentNullException("tail");
            }
            return tail.Prepend(head);
        }

        /// <summary>
        ///     Returns a sequence consisting of the head elements and the given tail element.
        /// </summary>
        /// <typeparam name="T">Type of sequence</typeparam>
        /// <param name="head">All elements of the head. Must not be null.</param>
        /// <param name="tail">Tail element of the new sequence.</param>
        /// <returns>A sequence consisting of the head elements and the given tail element.</returns>
        /// <remarks>This operator uses deferred execution and streams its results.</remarks>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> head, T tail)
        {
            if (head == null)
            {
                throw new ArgumentNullException("head");
            }
            return head.Concat(Enumerable.Repeat(tail, 1));
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T item)
        {
            return items.Except(item.Yield());
        }

        public static T FirstOrNew<T>(this ICollection<T> items, Func<T, bool> predicate) where T : class
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

        /// <summary>
        ///     Returns the maximal element of the given sequence, based on
        ///     the given projection.
        /// </summary>
        /// <remarks>
        ///     If more than one element has the maximal projected value, the first
        ///     one encountered will be returned. This overload uses the default comparer
        ///     for the projected type. This operator uses immediate execution, but
        ///     only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the items sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="selector" /> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source" /> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        ///     Returns the maximal element of the given sequence, based on
        ///     the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        ///     If more than one element has the maximal projected value, the first
        ///     one encountered will be returned. This overload uses the default comparer
        ///     for the projected type. This operator uses immediate execution, but
        ///     only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the items sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="source" />, <paramref name="selector" />
        ///     or <paramref name="comparer" /> is null
        /// </exception>
        /// <exception cref="InvalidOperationException"><paramref name="source" /> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
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

        /// <summary>
        ///     Returns the minimal element of the given sequence, based on
        ///     the given projection.
        /// </summary>
        /// <remarks>
        ///     If more than one element has the minimal projected value, the first
        ///     one encountered will be returned. This overload uses the default comparer
        ///     for the projected type. This operator uses immediate execution, but
        ///     only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the items sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="selector" /> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source" /> is empty</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        ///     Returns the minimal element of the given sequence, based on
        ///     the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        ///     If more than one element has the minimal projected value, the first
        ///     one encountered will be returned. This overload uses the default comparer
        ///     for the projected type. This operator uses immediate execution, but
        ///     only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the items sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="source" />, <paramref name="selector" />
        ///     or <paramref name="comparer" /> is null
        /// </exception>
        /// <exception cref="InvalidOperationException"><paramref name="source" /> is empty</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        /// <summary>
        ///     Prepends a single value to a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The sequence to prepend to.</param>
        /// <param name="value">The value to prepend.</param>
        /// <returns>
        ///     Returns a sequence where a value is prepended to it.
        /// </returns>
        /// <remarks>
        ///     This operator uses deferred execution and streams its results.
        /// </remarks>
        /// <code>
        /// int[] numbers = { 1, 2, 3 };
        /// IEnumerable&lt;int&gt; result = numbers.Prepend(0);
        /// </code>
        /// The
        /// <c>result</c>
        /// variable, when iterated over, will yield
        /// 0, 1, 2 and 3, in turn.
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return Enumerable.Repeat(value, 1).Concat(source);
        }

        public static void Push<T>(this IList<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Insert(0, item);
            }
        }

        public static void PushUnique<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Push(item);
            }
        }

        public static void PushUnique<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.PushUnique(item);
            }
        }

        public static void Remove<T>(this ICollection<T> items, Func<T, bool> predicate)
        {
            if (items == null)
            {
                return;
            }

            var itemsToRemove = items.Where(predicate).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                items.Remove(itemToRemove);
            }
        }

        public static void Remove<T>(this IEnumerable<ICollection<T>> collections, Func<T, bool> predicate)
        {
            if (collections == null)
            {
                return;
            }

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

        /// <summary>
        ///     Returns items from the input sequence until the given predicate returns true
        ///     when applied to the current items item; that item will be the last returned.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         TakeUntil differs from Enumerable.TakeWhile in two respects. Firstly, the sense
        ///         of the predicate is reversed: it is expected that the predicate will return false
        ///         to start with, and then return true - for example, when trying to find a matching
        ///         item in a sequence.
        ///     </para>
        ///     <para>
        ///         Secondly, TakeUntil yields the element which causes the predicate to return true. For
        ///         example, in a sequence <code>{ 1, 2, 3, 4, 5 }</code> and with a predicate of
        ///         <code>x => x == 3</code>, the result would be <code>{ 1, 2, 3 }</code>.
        ///     </para>
        ///     <para>
        ///         TakeUntil is as lazy as possible: it will not iterate over the items sequence
        ///         until it has to, it won't iterate further than it has to, and it won't evaluate
        ///         the predicate until it has to. (This means that an item may be returned which would
        ///         actually cause the predicate to throw an exception if it were evaluated, so long as
        ///         no more items of data are requested.)
        ///     </para>
        /// </remarks>
        /// <typeparam name="TSource">Type of the items sequence</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="predicate">Predicate used to determine when to stop yielding results from the items.</param>
        /// <returns>Items from the items sequence, until the predicate returns true when applied to the item.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null</exception>
        public static IEnumerable<TSource> TakeUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return TakeUntilImpl(source, predicate);
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

        private static IEnumerable<TSource> TakeUntilImpl<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (var item in source)
            {
                yield return item;
                if (predicate(item))
                {
                    yield break;
                }
            }
        }
    }
}