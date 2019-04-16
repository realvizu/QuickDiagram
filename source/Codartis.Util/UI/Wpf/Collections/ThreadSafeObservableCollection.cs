using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Codartis.Util.UI.Wpf.Collections
{
    /// <summary>
    /// A thread-safe implementation of an observable collection.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        private readonly object _lockObject = new object();

        public ThreadSafeObservableCollection()
        {
            Init();
        }

        public ThreadSafeObservableCollection(List<T> list) : base(list)
        {
            Init();
        }

        public ThreadSafeObservableCollection(IEnumerable<T> collection) : base(collection)
        {
            Init();
        }

        private void Init()
        {
            BindingOperations.EnableCollectionSynchronization(this, _lockObject);
        }
    }
}