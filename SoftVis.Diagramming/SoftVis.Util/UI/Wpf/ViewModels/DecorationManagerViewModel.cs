using System.Collections.Generic;
using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// Tracks focus changes and assigns a collection of decorators to the host that has the focus.
    /// The decoration can be pinned meaning that it won't follow the focus until unpinned.
    /// </summary>
    /// <typeparam name="THostViewModel">The type of the view model that hosts the decorators.</typeparam>
    public class DecorationManagerViewModel<THostViewModel> : ViewModelBase
        where THostViewModel : ViewModelBase
    {
        /// <summary>The view models that are applied as decorators.</summary>
        private readonly IEnumerable<IDecoratorViewModel<THostViewModel>> _decorators;

        /// <summary>The focused host is the one that the user points to.</summary>
        private THostViewModel _focusedHost;

        /// <summary>The decoration is pinned if it stays visible even when focus is lost from the host.</summary>
        private bool _isDecorationPinned;

        /// <summary>The decorated host is the one that has the decorators attached.</summary>
        private THostViewModel _decoratedHost;

        public DelegateCommand<THostViewModel> FocusCommand { get; }
        public DelegateCommand<THostViewModel> UnfocusCommand { get; }
        public DelegateCommand UnfocusAllCommand { get; }

        public DecorationManagerViewModel(IEnumerable<IDecoratorViewModel<THostViewModel>> decorators)
        {
            _decorators = decorators;
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

        public void Focus(THostViewModel hostViewModel)
        {
            if (hostViewModel == _focusedHost)
                return;

            Unfocus(_focusedHost);
            SetFocus(hostViewModel);
        }

        public void Unfocus(THostViewModel hostViewModel)
        {
            if (_focusedHost == hostViewModel)
                SetFocus(null);
        }

        public void UnfocusAll()
        {
            if (_focusedHost != null)
                Unfocus(_focusedHost);
        }

        /// <summary>
        /// Keeps the decorations visible even when the host loses focus.
        /// </summary>
        public void PinDecoration()
        {
            _isDecorationPinned = true;
        }

        /// <summary>
        /// Exits the "pinned" mode, that is, lets the decorators disappear when the host loses focus.
        /// </summary>
        public void UnpinDecoration()
        {
            _isDecorationPinned = false;
            ChangeDecorationTo(_focusedHost);
        }

        private void SetFocus(THostViewModel hostViewModel)
        {
            _focusedHost = hostViewModel;

            if (!_isDecorationPinned)
                ChangeDecorationTo(_focusedHost);
        }

        private void ChangeDecorationTo(THostViewModel hostViewModel)
        {
            DecoratedHost = hostViewModel;

            if (hostViewModel == null)
            {
                foreach (var decoratorViewModel in _decorators)
                    decoratorViewModel.Hide();
            }
            else
            {
                foreach (var decoratorViewModel in _decorators)
                    decoratorViewModel.AssociateWith(hostViewModel);
            }
        }
    }
}