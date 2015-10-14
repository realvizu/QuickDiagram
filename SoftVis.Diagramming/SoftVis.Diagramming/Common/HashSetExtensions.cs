using System.Collections.Generic;

namespace Codartis.SoftVis.Common
{
    public static class HashSetExtensions
    {
        public static void Add<T>(this HashSet<T> hashSet, IEnumerable<T> newItems)
        {
            foreach (var item in newItems)
                hashSet.Add(item);
        }
    }
}
