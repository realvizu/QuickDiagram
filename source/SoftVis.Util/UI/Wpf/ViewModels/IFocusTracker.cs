using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// Tracks focus changes and assigns a collection of decorators to a host view model.
    /// The decoration can be pinned meaning that it won't follow the focus until unpinned.
    /// </summary>
    /// <typeparam name="THostViewModel">The type of the view model that hosts the decorators.</typeparam>
    public interface IFocusTracker<THostViewModel>
    {
        /// <summary>
        /// Assigns focus to a given view model.
        /// </summary>
        /// <param name="hostViewModel"></param>
        void Focus(THostViewModel hostViewModel);

        /// <summary>
        /// Removes focus from a given view model.
        /// </summary>
        /// <param name="hostViewModel"></param>
        void Unfocus(THostViewModel hostViewModel);

        /// <summary>
        /// Removes focus from all items.
        /// </summary>
        void UnfocusAll();

        /// <summary>
        /// Keeps the decorations visible even when the host loses focus.
        /// </summary>
        void PinDecoration();

        /// <summary>
        /// Exits the "pinned" mode, that is, lets the decorators disappear when the host loses focus.
        /// </summary>
        void UnpinDecoration();

        /// <summary>
        /// A delegate the invokes the Focus method.
        /// </summary>
        DelegateCommand<THostViewModel> FocusCommand { get; }

        /// <summary>
        /// A delegate the invokes the Unfocus method.
        /// </summary>
        DelegateCommand<THostViewModel> UnfocusCommand { get; }

        /// <summary>
        /// A delegate the invokes the UnfocusAll method.
        /// </summary>
        DelegateCommand UnfocusAllCommand { get; }
    }
}