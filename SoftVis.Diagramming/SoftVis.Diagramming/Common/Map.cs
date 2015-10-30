using System.Collections.Generic;

namespace Codartis.SoftVis.Common
{
    /// <summary>
    /// Stores key-value pairs.
    /// Set operation performs add or update.
    /// Get operation returns default(TValue) if the key is not found.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class Map<TKey, TValue>
    {
        private readonly Dictionary<TKey,TValue> _map = new Dictionary<TKey, TValue>();

        public void Set(TKey key, TValue value)
        {
            if (_map.ContainsKey(key))
                _map[key] = value;
            else
                _map.Add(key,value);
        }

        public TValue Get(TKey key)
        {
            return _map.ContainsKey(key)
                ? _map[key]
                : default(TValue);
        }

        public void Remove(TKey key)
        {
            _map.Remove(key);
        }

        public void Clear()
        {
            _map.Clear();
        }
    }
}
