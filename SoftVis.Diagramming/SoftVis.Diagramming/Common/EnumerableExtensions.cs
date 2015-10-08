using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Common
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

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            if (item != null)
                yield return item;
        }
    }
}
