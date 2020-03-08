using System;
using System.Linq;
using JetBrains.Annotations;

namespace Codartis.Util
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the given item is in the given collection.
        /// Uses Equals for comparison.
        /// </summary>
        public static bool In<T>([NotNull] this T o, [NotNull] params T[] others) => others.Any(i => o.Equals(i));

        /// <summary>
        /// Converts the given object to the given type.
        /// Throws <see cref="ArgumentException"/> if the object is not of the given type.
        /// </summary>
        [NotNull]
        public static T EnsureType<T>([CanBeNull] this object o)
        {
            if (o is T t)
                return t;

            throw new ArgumentException($"{typeof(T).Name} was expected but received {o?.GetType().Name ?? "null"}");
        }
    }
}