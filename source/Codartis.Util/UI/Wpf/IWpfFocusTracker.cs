using Codartis.Util.UI.Wpf.Commands;

namespace Codartis.Util.UI.Wpf
{
    /// <summary>
    /// WPF-specific focus tracker where methods can be invoked via DelegateCommands too.
    /// </summary>
    /// <typeparam name="TUiElement">The type of the UI element that receives the focus.</typeparam>
    public interface IWpfFocusTracker<TUiElement> : IFocusTracker<TUiElement>
        where TUiElement : class 
    {
        /// <summary>
        /// A delegate the invokes the Focus method.
        /// </summary>
        DelegateCommand<TUiElement> FocusCommand { get; }

        /// <summary>
        /// A delegate the invokes the Unfocus method.
        /// </summary>
        DelegateCommand<TUiElement> UnfocusCommand { get; }

        /// <summary>
        /// A delegate the invokes the UnfocusAll method.
        /// </summary>
        DelegateCommand UnfocusAllCommand { get; }
    }
}