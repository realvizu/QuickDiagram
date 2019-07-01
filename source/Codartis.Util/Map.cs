using System.Collections;
using System.Collections.Generic;

namespace Codartis.Util
{
    /// <summary>
    /// Stores key-value pairs.
    /// Set operation performs add or update.
    /// Thread-safe.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class Map<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;
        private readonly object _lockObject = new object();

        public Map(IEqualityComparer<TKey> idComparer = null)
            : this(new Dictionary<TKey, TValue>(idComparer))
        {
        }

        public Map(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public void Set(TKey key, TValue value)
        {
            lock (_dictionary)
            {
                if (_dictionary.ContainsKey(key))
                    _dictionary[key] = value;
                else
                    _dictionary.Add(key, value);
            }
        }

        public TValue Get(TKey key)
        {
            lock (_lockObject)
            {
                if (Contains(key))
                    return _dictionary[key];

                throw new KeyNotFoundException($"Key {key} not found in the map.");
            }
        }

        public bool TryGet(TKey key, out TValue value, TValue valueForMissingKey = default)
        {
            lock (_lockObject)
            {
                if (Contains(key))
                {
                    value = _dictionary[key];
                    return true;
                }

                value = valueForMissingKey;
                return false;
            }
        }

        public bool Contains(TKey key)
        {
            lock (_lockObject)
                return _dictionary.ContainsKey(key);
        }

        public void Remove(TKey key)
        {
            lock (_lockObject)
                _dictionary.Remove(key);
        }

        public void Clear()
        {
            lock (_lockObject)
                _dictionary.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_lockObject)
                return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}