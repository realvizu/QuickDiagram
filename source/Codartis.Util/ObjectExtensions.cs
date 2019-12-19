using System;
using System.Linq;
using JetBrains.Annotations;

namespace Codartis.Util
{
    public static class ObjectExtensions
    {
        public static bool In<T>([NotNull] this T o, [NotNull] params T[] others) => others.Any(i => o.Equals(i));

        [NotNull]
        public static T Ensure<T>([CanBeNull] this object o)
        {
            if (o is T t)
                return t;

            throw new ArgumentException($"{typeof(T).Name} was expected but received {o?.GetType().Name ?? "null"}");
        }
    }
}