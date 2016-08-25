using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// Abstract base view model for a bubble selector.
    /// </summary>
    public abstract class BubbleListBoxViewModel : ViewModelBase
    {
        private bool _isVisible;
        private List<object> _items;
        private object _selectedItem;

        public DelegateCommand<object> ItemSelectedCommand { get; protected set; }

        protected void Show(IEnumerable<object> items)
        {
            IsVisible = true;
            Items = items.OrderBy(i => i.ToString()).ToList();
            SelectedItem = null;
        }

        public virtual void Hide()
        {
            IsVisible = false;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public List<object> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// A typed wrapper around BubbleSelectorViewModel.
    /// </summary>
    public class BubbleListBoxViewModel<TItem> : BubbleListBoxViewModel
        where TItem : class
    {
        public event Action<TItem> ItemSelected;

        public BubbleListBoxViewModel()
        {
            ItemSelectedCommand = new DelegateCommand<object>(i => ItemSelected?.Invoke((TItem)i));
            ItemSelected += OnItemSelected;
        }

        public virtual void Show(IEnumerable<TItem> items) => base.Show(items);

        protected virtual void OnItemSelected(TItem item)
        { }
    }
}
