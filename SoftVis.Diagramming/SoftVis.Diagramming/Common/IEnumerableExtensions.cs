using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Common
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int count = 1)
        {
            return collection.Skip(collection.Count() - count).Take(count);
        }

        public static IEnumerable<T> TakeButLast<T>(this IEnumerable<T> collection, int count = 1)
        {
            return collection.Take(collection.Count() - count);
        }
    }
}
