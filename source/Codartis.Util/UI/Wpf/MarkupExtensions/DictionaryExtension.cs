using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;

namespace Codartis.Util.UI.Wpf.MarkupExtensions
{
    /// <summary>
    /// Makes it possible to define and populate a dictionary instance in XAML.
    /// </summary>
    [MarkupExtensionReturnType(typeof(IDictionary))]
    public class DictionaryExtension : MarkupExtension, IDictionary
    {
        public Type KeyType { get; set; }
        public Type ValueType { get; set; }
        public IDictionary Init { get; set; }

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

                    if (Init != null)
                        foreach (var key in Init.Keys)
                            Add(key, Init[key]);
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

            if (Dictionary.Contains(key))
                Dictionary[key] = value;
            else
                Dictionary.Add(key, value);
        }

        public object this[object key]
        {
            get { return Dictionary[key]; }
            set { Dictionary[key] = value; }
        }

        public int Count => Dictionary.Count;
        public ICollection Keys => Dictionary.Keys;
        public ICollection Values => Dictionary.Values;
        public object SyncRoot => Dictionary.SyncRoot;
        public bool IsReadOnly => Dictionary.IsReadOnly;
        public bool IsFixedSize => Dictionary.IsFixedSize;
        public bool IsSynchronized => Dictionary.IsSynchronized;
        public bool Contains(object key) => Dictionary.Contains(key);

        public IDictionaryEnumerator GetEnumerator() => Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear() => Dictionary.Clear();
        public void Remove(object key) => Dictionary.Remove(key);
        public void CopyTo(Array array, int index) => Dictionary.Values.CopyTo(array, index);
    }
}