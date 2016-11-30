using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Codartis.SoftVis.Util.UI.Wpf.Collections
{
    /// <summary>
    /// A thread-safe observable collection implemented with immutable list.
    /// </summary>
    /// <typeparam name="T">The type of the list items.</typeparam>
    /// <remarks>
    /// Adapted from:
    /// https://www.codeproject.com/articles/64936/threadsafe-observableimmutable-collection
    /// </remarks>
    public class ObservableImmutableList<T> : ThreadSafeObservableCollectionBase, IList, ICollection, IEnumerable, IList<T>, IImmutableList<T>, ICollection<T>, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Private

        private readonly object _syncRoot;
        private ImmutableList<T> _items;

        #endregion Private

        #region Constructors

        public ObservableImmutableList() : this(new T[0], LockTypeEnum.SpinWait)
        {
        }

        public ObservableImmutableList(IEnumerable<T> items) : this(items, LockTypeEnum.SpinWait)
        {
        }

        public ObservableImmutableList(LockTypeEnum lockType) : this(new T[0], lockType)
        {
        }

        public ObservableImmutableList(IEnumerable<T> items, LockTypeEnum lockType) : base(lockType)
        {
            _syncRoot = new object();
            _items = ImmutableList<T>.Empty.AddRange(items);
        }

        #endregion Constructors

        #region Thread-Safe Methods

        #region General

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryOperation(Func<ImmutableList<T>, ImmutableList<T>> operation)
        {
            return TryOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DoOperation(Func<ImmutableList<T>, ImmutableList<T>> operation)
        {
            return DoOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #region Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableList<T>, ImmutableList<T>> operation, NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (TryLock())
                {
                    var oldList = _items;
                    var newItems = operation(oldList);

                    if (newItems == null)
                    {
                        // user returned null which means he cancelled operation
                        return false;
                    }

                    _items = newItems;

                    if (args != null)
                        RaiseNotifyCollectionChanged(args);
                    return true;
                }
            }
            finally
            {
                Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableList<T>, KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>> operation)
        {
            try
            {
                if (TryLock())
                {
                    var oldList = _items;
                    var kvp = operation(oldList);
                    var newItems = kvp.Key;
                    var args = kvp.Value;

                    if (newItems == null)
                    {
                        // user returned null which means he cancelled operation
                        return false;
                    }

                    _items = newItems;

                    if (args != null)
                        RaiseNotifyCollectionChanged(args);
                    return true;
                }
            }
            finally
            {
                Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableList<T>, ImmutableList<T>> operation, NotifyCollectionChangedEventArgs args)
        {
            bool result;

            try
            {
                Lock();
                var oldItems = _items;
                var newItems = operation(_items);

                if (newItems == null)
                {
                    // user returned null which means he cancelled operation
                    return false;
                }

                result = (_items = newItems) != oldItems;

                if (args != null)
                    RaiseNotifyCollectionChanged(args);
            }
            finally
            {
                Unlock();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableList<T>, KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>> operation)
        {
            bool result;

            try
            {
                Lock();
                var oldItems = _items;
                var kvp = operation(_items);
                var newItems = kvp.Key;
                var args = kvp.Value;

                if (newItems == null)
                {
                    // user returned null which means he cancelled operation
                    return false;
                }

                result = (_items = newItems) != oldItems;

                if (args != null)
                    RaiseNotifyCollectionChanged(args);
            }
            finally
            {
                Unlock();
            }

            return result;
        }

        #endregion Helpers

        #endregion General

        #region Specific

        public bool DoInsert(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                {
                    var kvp = valueProvider(currentItems);
                    var newItems = currentItems.Insert(kvp.Key, kvp.Value);
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp.Value, kvp.Key));
                }
                );
        }

        public bool DoAdd(Func<ImmutableList<T>, T> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                {
                    T value;
                    var newItems = _items.Add(value = valueProvider(currentItems));
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, currentItems.Count));
                }
                );
        }

        public bool DoAddRange(Func<ImmutableList<T>, IEnumerable<T>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                    currentItems.AddRange(valueProvider(currentItems))
                );
        }

        public bool DoRemove(Func<ImmutableList<T>, T> valueProvider)
        {
            return DoRemoveAt
                (
                currentItems =>
                    currentItems.IndexOf(valueProvider(currentItems))
                );
        }

        public bool DoRemoveAt(Func<ImmutableList<T>, int> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                {
                    var index = valueProvider(currentItems);
                    var value = currentItems[index];
                    var newItems = currentItems.RemoveAt(index);
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
                }
                );
        }

        public bool DoSetItem(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                {
                    var kvp = valueProvider(currentItems);
                    var newValue = kvp.Value;
                    var index = kvp.Key;
                    var oldValue = currentItems[index];
                    var newItems = currentItems.SetItem(kvp.Key, newValue);
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue, index));
                }
                );
        }

        public bool TryInsert(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                {
                    var kvp = valueProvider(currentItems);
                    var newItems = currentItems.Insert(kvp.Key, kvp.Value);
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp.Value, kvp.Key));
                }
                );
        }

        public bool TryAdd(Func<ImmutableList<T>, T> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                {
                    T value;
                    var newItems = _items.Add(value = valueProvider(currentItems));
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, currentItems.Count));
                }
                );
        }

        public bool TryAddRange(Func<ImmutableList<T>, IEnumerable<T>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                    currentItems.AddRange(valueProvider(currentItems))
                );
        }

        public bool TryRemove(Func<ImmutableList<T>, T> valueProvider)
        {
            return TryRemoveAt
                (
                currentItems =>
                    currentItems.IndexOf(valueProvider(currentItems))
                );
        }

        public bool TryRemoveAt(Func<ImmutableList<T>, int> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                {
                    var index = valueProvider(currentItems);
                    var value = currentItems[index];
                    var newItems = currentItems.RemoveAt(index);
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
                }
                );
        }

        public bool TrySetItem(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                {
                    var kvp = valueProvider(currentItems);
                    var newValue = kvp.Value;
                    var index = kvp.Key;
                    var oldValue = currentItems[index];
                    var newItems = currentItems.SetItem(kvp.Key, newValue);
                    return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue, index));
                }
                );
        }

        #endregion Specific

        public ImmutableList<T> ToImmutableList()
        {
            return _items;
        }

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<T>

        #endregion Thread-Safe Methods

        #region Non Thead-Safe Methods

        #region IList

        public int Add(object value)
        {
            var val = (T)value;
            Add(val);
            return IndexOf(val);
        }

        public bool Contains(object value)
        {
            return Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public void Remove(object value)
        {
            Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                SetItem(index, (T)value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            _items.ToArray().CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        #endregion IList

        #region IList<T>

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                SetItem(index, value);
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        void IList.Clear()
        {
            Clear();
        }

        void ICollection<T>.Clear()
        {
            Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            if (!_items.Contains(item))
                return false;

            _items = _items.Remove(item);
            RaiseNotifyCollectionChanged();
            return true;
        }

        #endregion IList<T>

        #region IImmutableList<T>

        public IImmutableList<T> Add(T value)
        {
            var index = _items.Count;
            _items = _items.Add(value);
            RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            return this;
        }

        public IImmutableList<T> AddRange(IEnumerable<T> items)
        {
            _items = _items.AddRange(items);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableList<T> Clear()
        {
            _items = _items.Clear();
            RaiseNotifyCollectionChanged();
            return this;
        }

        public int IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return _items.IndexOf(item, index, count, equalityComparer);
        }

        public IImmutableList<T> Insert(int index, T element)
        {
            _items = _items.Insert(index, element);
            RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, index));
            return this;
        }

        public IImmutableList<T> InsertRange(int index, IEnumerable<T> items)
        {
            _items = _items.InsertRange(index, items);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public int LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return _items.LastIndexOf(item, index, count, equalityComparer);
        }

        public IImmutableList<T> Remove(T value, IEqualityComparer<T> equalityComparer)
        {
            var index = _items.IndexOf(value, equalityComparer);
            RemoveAt(index);
            return this;
        }

        public IImmutableList<T> RemoveAll(Predicate<T> match)
        {
            _items = _items.RemoveAll(match);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableList<T> RemoveAt(int index)
        {
            var value = _items[index];
            _items = _items.RemoveAt(index);
            RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
            return this;
        }

        public IImmutableList<T> RemoveRange(int index, int count)
        {
            _items = _items.RemoveRange(index, count);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            _items = _items.RemoveRange(items, equalityComparer);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            var index = _items.IndexOf(oldValue, equalityComparer);
            SetItem(index, newValue);
            return this;
        }

        public IImmutableList<T> SetItem(int index, T value)
        {
            var oldItem = _items[index];
            _items = _items.SetItem(index, value);
            RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
            return this;
        }

        #endregion IImmutableList<T>

        #endregion Non Thead-Safe Methods
    }
}
