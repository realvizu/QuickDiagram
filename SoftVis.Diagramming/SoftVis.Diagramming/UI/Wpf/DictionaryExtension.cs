using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Makes it possible to define and populate a dictionary instace in XAML.
    /// </summary>
    public class DictionaryExtension : MarkupExtension, IDictionary
    {
        public Type KeyType { get; set; }
        public Type ValueType { get; set; }

        private IDictionary _dictionary;
        private IDictionary Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    if (KeyType == null)
                        throw new ArgumentNullException(nameof(KeyType));
                    if (ValueType == null)
                        throw new ArgumentNullException(nameof(ValueType));

                    var type = typeof(Dictionary<,>);
                    var dictType = type.MakeGenericType(KeyType, ValueType);
                    _dictionary = (IDictionary)Activator.CreateInstance(dictType);
                }
                return _dictionary;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => Dictionary;

        public void Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!KeyType.IsInstanceOfType(key))
                key = TypeDescriptor.GetConverter(KeyType).ConvertFrom(key);

            if (key == null)
                throw new ArgumentException("Key was converted to null.", nameof(key));

            Dictionary.Add(key, value);
        }

        public object this[object key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public int Count => _dictionary.Count;
        public ICollection Keys => _dictionary.Keys;
        public ICollection Values => _dictionary.Values;
        public object SyncRoot => _dictionary.SyncRoot;
        public bool IsReadOnly => _dictionary.IsReadOnly;
        public bool IsFixedSize => _dictionary.IsFixedSize;
        public bool IsSynchronized => _dictionary.IsSynchronized;
        public bool Contains(object key) => _dictionary.Contains(key);

        public IDictionaryEnumerator GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear() => _dictionary.Clear();
        public void Remove(object key) => _dictionary.Remove(key);
        public void CopyTo(Array array, int index) => _dictionary.Values.CopyTo(array, index);
    }
}
