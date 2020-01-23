using System;
using System.Threading.Tasks;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Operations to access the UI services of the host environment.
    /// </summary>
    public interface IHostUiServices
    {
        /// <summary>
        /// Shows the diagram-hosting tool window.
        /// </summary>
        Task ShowDiagramWindowAsync();

        /// <summary>
        /// Returns a new progress dialog.
        /// </summary>
        /// <remarks>
        /// Invoke it in a using block for automatic disposing.
        /// </remarks>
        Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0);

        /// <summary>
        /// Runs an async method in such a way that don't cause deadlock in the host.
        /// </summary>
        Task RunAsync(Func<Task> asyncMethod);

    }
}
