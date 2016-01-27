using System;
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
        public bool IsFocused { get; private set; }

        public DelegateCommand FocusCommand { get; set; }
        public DelegateCommand UnfocusCommand { get; set; }

        public event Action<FocusableViewModelBase> GotFocus;
        public event Action<FocusableViewModelBase> LostFocus;

        protected FocusableViewModelBase()
        {
            FocusCommand = new DelegateCommand(Focus);
            UnfocusCommand = new DelegateCommand(Unfocus);
        }

        public void Focus()
        {
            if (!IsFocused)
            {
                IsFocused = true;
                GotFocus?.Invoke(this);
            }
        }

        public void Unfocus()
        {
            if (IsFocused)
            {
                IsFocused = false;
                LostFocus?.Invoke(this);
            }
        }
    }
}