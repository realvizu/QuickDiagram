using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.Util
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the items of the source collection, except the last n items.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="sourceCollection">The source collection.</param>
        /// <param name="count">The number of items to return. Optional, the default is 1.</param>
        /// <returns>The items of the source collection except the last n items.</returns>
        public static IEnumerable<T> TakeButLast<T>(this IEnumerable<T> sourceCollection, int count = 1)
        {
            var sourceList = sourceCollection as IList<T> ?? sourceCollection.ToList();
            return sourceList.Take(sourceList.Count - count);
        }

        [NotNull]
        public static IEnumerable<T> EmptyIfNull<T>([CanBeNull] this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            if (item != null)
                yield return item;
        }

        public static IEnumerable<T> Concat<T>(this T o, IEnumerable<T> others)
        {
            return o.ToEnumerable().Concat(others);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            if (source is IList<T> sourceAsList)
                return sourceAsList.IndexOf(value);

            var index = 0;
            var comparer = EqualityComparer<T>.Default;
            foreach (var item in source)
            {
                if (comparer.Equals(item, value))
                    return index;

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Returns a value indicating whether two sequences are equal compared item-by-item.
        /// If any of the collections is null than it's treated as empty.
        /// </summary>
        /// <typeparam name="T">The type of the collections' items.</typeparam>
        /// <param name="collection">A collection.</param>
        /// <param name="otherCollection">A collection.</param>
        /// <returns></returns>
        public static bool EmptyIfNullSequenceEqual<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            return collection.EmptyIfNull().SequenceEqual(otherCollection.EmptyIfNull());
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T item) => collection.Except(item.ToEnumerable());

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> enumeration)
        {
            return enumeration.Where(i => i != null);
        }

        [NotNull]
        public static IEnumerable<TAggregate> RollingAggregate<TItem, TAggregate>(
            [NotNull] this IEnumerable<TItem> items,
            [NotNull] Func<TItem, TAggregate, TAggregate> aggregator,
            TAggregate seed = default)
        {
            var result = seed;
            foreach (var item in items)
            {
                result = aggregator(item, result);
                yield return result;
            }
        }

        [NotNull]
        [ItemNotNull]
        public static async Task<IEnumerable<TResult>> SelectAsync<T, TResult>(this IEnumerable<T> enumeration, Func<T, Task<TResult>> func)
        {
            return await Task.WhenAll(enumeration.Select(func));
        }

        public static async Task<IEnumerable<TResult>> SelectManyAsync<T, TResult>(this IEnumerable<T> enumeration, Func<T, Task<IEnumerable<TResult>>> func)
        {
            return (await Task.WhenAll(enumeration.Select(func))).SelectMany(s => s);
        }

        [NotNull]
        public static IEnumerable<TResult> SelectPairs<T, TResult>([NotNull] this IEnumerable<T> enumeration, [NotNull] Func<T, T, TResult> func)
        {
            var array = enumeration.ToArray();

            if (array.Length % 2 != 0)
                throw new ArgumentException($"Collection must have even number of items but has {array.Length}");

            for (int i = 0; i < array.Length; i += 2)
                yield return func(array[i], array[i + 1]);
        }
    }
}