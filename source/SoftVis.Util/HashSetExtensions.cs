using System.Collections.Generic;

namespace Codartis.SoftVis.Util
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
