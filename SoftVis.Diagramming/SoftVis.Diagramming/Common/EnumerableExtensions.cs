using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Common
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the last n items of a collection.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="sourceCollection">The source collection.</param>
        /// <param name="count">The number of items to return. Optional, the default is 1.</param>
        /// <returns>The last n items of the source collection.</returns>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> sourceCollection, int count = 1)
        {
            var sourceList = sourceCollection as IList<T> ?? sourceCollection.ToList();
            return sourceList.Skip(sourceList.Count - count).Take(count);
        }

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

        /// <summary>
        /// Compares two sequences for equality (item-be-item).
        /// They are equal if both are null or both has te same length and the corresponding items are equal 
        /// using their default equality comparer.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="collection1">A collection</param>
        /// <param name="collection2">A collection.</param>
        /// <returns>True if both collections are null or they have the same length 
        /// and the corresponding items are equal using their default equality comparer.</returns>
        public static bool NullableSequenceEqual<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            if (collection1 == null && collection2 == null)
                return true;

            if (collection1 != null
                && collection2 != null
                && collection1.SequenceEqual(collection2))
                return true;

            return false;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            if (item != null)
                yield return item;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, T item)
        {
            return collection.Concat(item.ToEnumerable());
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }
    }
}
