using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int count = 1)
        {
            var enumerable = collection as IList<T> ?? collection.ToList();
            return enumerable.Skip(enumerable.Count - count).Take(count);
        }

        public static IEnumerable<T> TakeButLast<T>(this IEnumerable<T> collection, int count = 1)
        {
            var enumerable = collection as IList<T> ?? collection.ToList();
            return enumerable.Take(enumerable.Count - count);
        }
    }
}
