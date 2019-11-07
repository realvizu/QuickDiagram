using System;
using System.Threading.Tasks;
using Codartis.Util.UI.Wpf.Dialogs;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Operations to access the UI services of the host environment.
    /// </summary>
    public interface IHostUiService
    {
        /// <summary>
        /// Shows the diagram-hosting tool window.
        /// </summary>
        /// <remarks>
        /// Later it will have a DiagramId parameter to specify an instance.
        /// </remarks>
        [NotNull]
        Task ShowDiagramWindowAsync();

        void ShowMessageBox([NotNull] string message);

        /// <summary>
        /// Shows a SaveFileDialog and returns the selected filename.
        /// </summary>
        [NotNull]
        string SelectSaveFilename([NotNull] string title, [NotNull] string filter);

        /// <summary>
        /// Returns a new progress dialog.
        /// </summary>
        /// <remarks>
        /// Invoke it in a using block for automatic disposing.
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        Task<ProgressDialog> CreateProgressDialogAsync([NotNull] string text, int maxProgress = 0);

        /// <summary>
        /// Runs an async method from a sync method.
        /// </summary>
        void Run([NotNull] Func<Task> asyncMethod);
    }
}