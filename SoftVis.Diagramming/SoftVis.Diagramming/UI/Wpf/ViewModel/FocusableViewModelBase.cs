using System;
using System.Windows.Input;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A kind of view model that can receive and lose focus.
    /// Can be controled by commands or method calls. 
    /// Publishes events about focus state changes.
    /// </summary>
    public abstract class FocusableViewModelBase : ViewModelBase
    {
        private bool _isFocused;

        public ICommand FocusCommand { get; set; }
        public ICommand UnfocusCommand { get; set; }

        public event Action<FocusableViewModelBase> GotFocus;
        public event Action<FocusableViewModelBase> LostFocus;

        protected FocusableViewModelBase()
        {
            FocusCommand = new DelegateCommand(i => Focus());
            UnfocusCommand = new DelegateCommand(i => Unfocus());
        }

        public void Focus()
        {
            if (!_isFocused)
            {
                _isFocused = true;
                GotFocus?.Invoke(this);
            }
        }

        public void Unfocus()
        {
            if (_isFocused)
            {
                _isFocused = false;
                LostFocus?.Invoke(this);
            }
        }
    }
}