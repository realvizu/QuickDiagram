using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// WPF-specific focus tracker where methods can be invoked via DelegateCommands too.
    /// </summary>
    /// <typeparam name="THost">The type of the UI element that hosts the decorators.</typeparam>
    public interface IWpfFocusTracker<THost> : IFocusTracker<THost>
        where THost : class 
    {
        /// <summary>
        /// A delegate the invokes the Focus method.
        /// </summary>
        DelegateCommand<THost> FocusCommand { get; }

        /// <summary>
        /// A delegate the invokes the Unfocus method.
        /// </summary>
        DelegateCommand<THost> UnfocusCommand { get; }

        /// <summary>
        /// A delegate the invokes the UnfocusAll method.
        /// </summary>
        DelegateCommand UnfocusAllCommand { get; }
    }
}