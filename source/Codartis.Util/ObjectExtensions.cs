using System;
using System.Linq;
using JetBrains.Annotations;

namespace Codartis.Util
{
    public static class ObjectExtensions
    {
        public static bool In([NotNull] this object o, [NotNull] params object[] others)
        {
            return others.Any(o.Equals);
        }

        [NotNull]
        public static T Ensure<T>([CanBeNull] this object o)
        {
            if (o is T t)
                return t;

            throw new ArgumentException($"{typeof(T).Name} was expected but received {o?.GetType().Name ?? "null"}");
        }
    }
}