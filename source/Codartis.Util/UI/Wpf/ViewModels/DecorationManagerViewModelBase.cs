using System.Collections.Generic;
using Codartis.Util.UI.Wpf.Commands;

namespace Codartis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// Abstract base class for tracking focus changes and assigning a collection of decorators to a host view model.
    /// The decoration can be pinned meaning that it won't follow the focus until unpinned.
    /// </summary>
    /// <typeparam name="TDecoratorViewModel">The tpe of the decorator view model.</typeparam>
    /// <typeparam name="THostViewModel">The type of the view model that hosts the decorators.</typeparam>
    public abstract class DecorationManagerViewModelBase<TDecoratorViewModel, THostViewModel> : ViewModelBase,
        IDecorationManager<TDecoratorViewModel, THostViewModel>,
        IWpfFocusTracker<THostViewModel>
        where TDecoratorViewModel : IUiDecorator<THostViewModel>
        where THostViewModel : class
    {
        /// <summary>The focused host is the one that the user points to.</summary>
        private THostViewModel _focusedHost;

        /// <summary>The current decorators. Null if there's no focused host. </summary>
        public IEnumerable<TDecoratorViewModel> Decorators { get; private set; }

        /// <summary>The decoration is pinned if it stays visible even when focus is lost from the host.</summary>
        private bool _isDecorationPinned;

        /// <summary>The decorated host is the one that has the decorators attached.</summary>
        private THostViewModel _decoratedHost;

        public DelegateCommand<THostViewModel> FocusCommand { get; }
        public DelegateCommand<THostViewModel> UnfocusCommand { get; }
        public DelegateCommand UnfocusAllCommand { get; }

        protected DecorationManagerViewModelBase()
        {
            _focusedHost = null;
            _isDecorationPinned = false;

            FocusCommand = new DelegateCommand<THostViewModel>(Focus);
            UnfocusCommand = new DelegateCommand<THostViewModel>(Unfocus);
            UnfocusAllCommand = new DelegateCommand(UnfocusAll);
        }

        public THostViewModel DecoratedHost
        {
            get { return _decoratedHost; }
            set
            {
                _decoratedHost = value;
                OnPropertyChanged();
            }
        }

        public void Focus(THostViewModel uiElement)
        {
            if (uiElement == _focusedHost)
                return;

            Unfocus(_focusedHost);
            SetFocus(uiElement);
        }

        public void Unfocus(THostViewModel uiElement)
        {
            if (_focusedHost == uiElement)
                SetFocus(null);
        }

        public void UnfocusAll()
        {
            if (_focusedHost != null)
                Unfocus(_focusedHost);
        }

        public void PinDecoration()
        {
            _isDecorationPinned = true;
        }

        public void UnpinDecoration()
        {
            _isDecorationPinned = false;
            ChangeDecorationTo(_focusedHost);
        }

        protected abstract IEnumerable<TDecoratorViewModel> GetDecoratorsFor(THostViewModel hostViewModel);

        private void SetFocus(THostViewModel hostViewModel)
        {
            _focusedHost = hostViewModel;

            if (!_isDecorationPinned)
                ChangeDecorationTo(_focusedHost);
        }

        private void ChangeDecorationTo(THostViewModel hostViewModel)
        {
            if (Decorators != null)
            {
                foreach (var decorator in Decorators)
                    decorator.Hide();
            }

            DecoratedHost = hostViewModel;

            if (hostViewModel == null)
            {
                Decorators = null;
            }
            else
            {
                Decorators = GetDecoratorsFor(hostViewModel);
                foreach (var decorator in Decorators)
                    decorator.AssociateWith(hostViewModel);
            }
        }
    }
}