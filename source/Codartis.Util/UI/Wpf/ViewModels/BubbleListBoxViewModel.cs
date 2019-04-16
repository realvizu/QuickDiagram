using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.Util.UI.Wpf.Collections;
using Codartis.Util.UI.Wpf.Commands;

namespace Codartis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// View model for a bubble listbox control.
    /// </summary>
    public class BubbleListBoxViewModel<TItem> : ShowHideViewModelBase
        where TItem : class
    {
        private ThreadSafeObservableCollection<TItem> _items;
        private TItem _selectedItem;

        public DelegateCommand<object> ItemSelectedCommand { get; protected set; }
        public event Action<TItem> ItemSelected;

        protected BubbleListBoxViewModel()
        {
            Items = new ThreadSafeObservableCollection<TItem>();
            ItemSelected += OnItemSelected;
            ItemSelectedCommand = new DelegateCommand<object>(i => ItemSelected?.Invoke((TItem)i));
        }

        public ThreadSafeObservableCollection<TItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public TItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        protected void Show(IEnumerable<TItem> items)
        {
            SelectedItem = null;
            Items.Clear();

            foreach (var item in items.OrderBy(i => i.ToString()))
                Items.Add(item);

            base.Show();
        }

        protected virtual void OnItemSelected(TItem item)
        { }
    }
}
