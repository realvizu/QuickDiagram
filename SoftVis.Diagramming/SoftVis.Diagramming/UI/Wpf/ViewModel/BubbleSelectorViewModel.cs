using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a bubble selector.
    /// </summary>
    public class BubbleSelectorViewModel : ViewModelBase
    {
        private bool _isVisible;
        private double _width;
        private double _height;
        private double _top;
        private double _left;
        private HandleOrientation _handleOrientation;
        private List<object> _items;
        private object _selectedItem;

        public DelegateCommand<object> ItemSelectedCommand { get; protected set; }

        public BubbleSelectorViewModel(Size size)
        {
            _width = size.Width;
            _height = size.Height;
        }

        public void Show(Point attachPoint, HandleOrientation handleOrientation, IEnumerable<object> items)
        {
            IsVisible = true;
            HandleOrientation = handleOrientation;
            Top = CalculateTop(attachPoint, handleOrientation);
            Left = CalculateLeft(attachPoint, handleOrientation);
            Items = items.ToList();
            SelectedItem = null;
        }

        public void Hide()
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

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged();
            }
        }

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged();
            }
        }

        public HandleOrientation HandleOrientation
        {
            get { return _handleOrientation; }
            set
            {
                _handleOrientation = value;
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

        private double CalculateTop(Point attachPoint, HandleOrientation handleOrientation)
        {
            switch (handleOrientation)
            {
                case HandleOrientation.Top:
                    return attachPoint.Y;
                case HandleOrientation.Bottom:
                    return attachPoint.Y - Height;

                default: throw new NotImplementedException();
            }
        }

        private double CalculateLeft(Point attachPoint, HandleOrientation handleOrientation)
        {
            switch (handleOrientation)
            {
                case HandleOrientation.Top:
                case HandleOrientation.Bottom:
                    return attachPoint.X - Width / 2;

                default: throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// A typed wrapper around BubbleSelectorViewModel.
    /// </summary>
    public class BubbleSelectorViewModel<TItem> : BubbleSelectorViewModel
        where TItem : class
    {
        public event Action<TItem> ItemSelected;

        public BubbleSelectorViewModel(Size size)
            : base(size)
        {
            ItemSelectedCommand = new DelegateCommand<object>(i => ItemSelected?.Invoke((TItem)i));
        }

        public void Show(Point attachPoint, HandleOrientation handleOrientation, IEnumerable<TItem> items) 
            => base.Show(attachPoint, handleOrientation, items);
    }
}
