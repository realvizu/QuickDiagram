using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace Codartis.SoftVis.Util.UI.Wpf.Collections
{
    /// <summary>
    /// A thread-safe implementation of an observable collection.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    public class ThreadSafeObservableList<T> : IList, IList<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IList<T> _collection;
        private readonly Dispatcher _dispatcher;
        private readonly ReaderWriterLock _sync = new ReaderWriterLock();

        public ThreadSafeObservableList(IEnumerable<T> initialItems = null)
        {
            _collection = initialItems == null ? new List<T>() : new List<T>(initialItems);
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Add(T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoAdd(item);
            else
                _dispatcher.BeginInvoke((Action)(() => DoAdd(item)));
        }

        public void Insert(int index, T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoInsert(index, item);
            else
                _dispatcher.BeginInvoke((Action)(() => DoInsert(index, item)));
        }

        public bool Remove(T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                return DoRemove(item);

            var dispatcherOperation = _dispatcher.BeginInvoke(new Func<T, bool>(DoRemove), item);
            if (dispatcherOperation.Result == null)
                return false;

            return (bool)dispatcherOperation.Result;
        }

        public void RemoveAt(int index)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoRemoveAt(index);
            else
                _dispatcher.BeginInvoke((Action)(() => DoRemoveAt(index)));
        }

        public void Clear()
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoClear();
            else
                _dispatcher.BeginInvoke((Action)DoClear);
        }

        public bool Contains(T item)
        {
            _sync.AcquireReaderLock(Timeout.Infinite);
            var result = _collection.Contains(item);
            _sync.ReleaseReaderLock();
            return result;
        }

        public int IndexOf(T item)
        {
            _sync.AcquireReaderLock(Timeout.Infinite);
            var result = _collection.IndexOf(item);
            _sync.ReleaseReaderLock();
            return result;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _sync.AcquireWriterLock(Timeout.Infinite);
            _collection.CopyTo(array, arrayIndex);
            _sync.ReleaseWriterLock();
        }

        public int Count
        {
            get
            {
                _sync.AcquireReaderLock(Timeout.Infinite);
                var result = _collection.Count;
                _sync.ReleaseReaderLock();
                return result;
            }
        }

        public T this[int index]
        {
            get
            {
                _sync.AcquireReaderLock(Timeout.Infinite);
                var result = _collection[index];
                _sync.ReleaseReaderLock();
                return result;
            }
            set
            {
                _sync.AcquireWriterLock(Timeout.Infinite);
                if (_collection.Count == 0 || _collection.Count <= index)
                {
                    _sync.ReleaseWriterLock();
                    return;
                }
                _collection[index] = value;
                _sync.ReleaseWriterLock();
            }
        }

        public bool IsReadOnly => _collection.IsReadOnly;
        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        public int Add(object value)
        {
            Add((T) value);
            return Count-1;
        }

        public void Insert(int index, object value) => Insert(index, (T)value);
        public void Remove(object value) => Remove((T)value);
        public bool Contains(object value) => Contains((T) value);
        public int IndexOf(object value) => IndexOf((T)value);
        public void CopyTo(Array array, int index) => CopyTo(array.OfType<T>().ToArray(), index);
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot { get; } = new object();

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T) value; }
        }

        private void DoAdd(T item)
        {
            _sync.AcquireWriterLock(Timeout.Infinite);
            _collection.Add(item);
            _sync.ReleaseWriterLock();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        private void DoInsert(int index, T item)
        {
            _sync.AcquireWriterLock(Timeout.Infinite);
            _collection.Insert(index, item);
            _sync.ReleaseWriterLock();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        private bool DoRemove(T item)
        {
            _sync.AcquireWriterLock(Timeout.Infinite);

            var index = _collection.IndexOf(item);
            if (index == -1)
            {
                _sync.ReleaseWriterLock();
                return false;
            }

            var result = _collection.Remove(item);
            _sync.ReleaseWriterLock();

            if (result)
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

            return result;
        }

        private void DoRemoveAt(int index)
        {
            _sync.AcquireWriterLock(Timeout.Infinite);
            if (_collection.Count == 0 || _collection.Count <= index)
            {
                _sync.ReleaseWriterLock();
                return;
            }
            var item = _collection[index];
            _collection.RemoveAt(index);
            _sync.ReleaseWriterLock();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        private void DoClear()
        {
            _sync.AcquireWriterLock(Timeout.Infinite);
            _collection.Clear();
            _sync.ReleaseWriterLock();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
