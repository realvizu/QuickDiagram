using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace Codartis.Util
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns a new dictionary where selected values are transformed.
        /// Those values get transformed that has a param value corresponding to its key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <typeparam name="TParam">The type of the parameter used by the transform func.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="transformFunc">The transform func that takes a value from the dictionary and a param value.</param>
        /// <param name="paramValues">A dictionary that contains param values for those keys whose value must be transformed.</param>
        /// <returns>The transformed key-value pair collection.</returns>
        [NotNull]
        public static IEnumerable<KeyValuePair<TKey, TValue>> Transform<TKey, TValue, TParam>(
            [NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> source,
            [NotNull] Func<TValue, TParam, TValue> transformFunc,
            [NotNull] IDictionary<TKey, TParam> paramValues)
        {
            return source.Select(i => GetTransformedValueWithKey(i, transformFunc, paramValues));
        }

        private static KeyValuePair<TKey, TValue> GetTransformedValueWithKey<TKey, TValue, TParam>(
            KeyValuePair<TKey, TValue> sourceValueWithKey,
            [NotNull] Func<TValue, TParam, TValue> transformFunc,
            [NotNull] IDictionary<TKey, TParam> paramValues)
        {
            return paramValues.TryGetValue(sourceValueWithKey.Key, out var param)
                ? new KeyValuePair<TKey, TValue>(sourceValueWithKey.Key, transformFunc(sourceValueWithKey.Value, param))
                : sourceValueWithKey;
        }

        [NotNull]
        public static IImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>([NotNull] this (TKey, TValue)[] items)
        {
            var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
            foreach (var (key, value) in items)
                builder.Add(key, value);
            return builder.ToImmutable();
        }
    }
}