using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Util
{
    /// <summary>
    /// Stores key-value pairs.
    /// Set operation performs add or update.
    /// Get operation returns null if the key is not found and throws if TValue is not nullable.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class Map<TKey, TValue>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly bool IsValueNullable;

        protected readonly Dictionary<TKey, TValue> Dictionary;

        static Map()
        {
            var type = typeof(TValue);
            IsValueNullable = type.IsClass || IsNullableValueType(type);
        }

        public Map() 
            : this(new Dictionary<TKey, TValue>())
        {
        }

        public Map(Dictionary<TKey, TValue> dictionary)
        {
            Dictionary = dictionary;
        }

        public IEnumerable<TKey> Keys => Dictionary.Keys;

        public void Set(TKey key, TValue value)
        {
            if (Dictionary.ContainsKey(key))
                Dictionary[key] = value;
            else
                Dictionary.Add(key, value);
        }

        public TValue Get(TKey key)
        {
            if (Contains(key))
                return Dictionary[key];

            if (IsValueNullable)
                return default(TValue);

            throw new KeyNotFoundException($"Key {key} not found in the map.");
        }

        public bool Contains(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public void Remove(TKey key)
        {
            Dictionary.Remove(key);
        }

        public void Clear()
        {
            Dictionary.Clear();
        }

        private static bool IsNullableValueType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
