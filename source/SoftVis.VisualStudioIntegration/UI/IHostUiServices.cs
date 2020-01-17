using System;
using System.Threading.Tasks;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Operations to access the UI services of the host environment.
    /// </summary>
    public interface IHostUiServices
    {
        /// <summary>
        /// Hosts a diagram control in a host-provided tool window.
        /// </summary>
        /// <param name="diagramControl">The diagram control.</param>
        void HostDiagram(DiagramControl diagramControl);

        /// <summary>
        /// Shows the diagram-hosting tool window.
        /// </summary>
        Task ShowDiagramWindowAsync();

        void ShowMessageBox(string message);

        /// <summary>
        /// Shows a SaveFileDialog and returns the selected filename.
        /// </summary>
        string SelectSaveFilename(string title, string filter);

        /// <summary>
        /// Returns a new progress dialog.
        /// </summary>
        /// <remarks>
        /// Invoke it in a using block for automatic disposing.
        /// </remarks>
        Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0);

        /// <summary>
        /// Runs an async method from a sync method.
        /// </summary>
        void Run(Func<Task> asyncMethod);

    }
}
