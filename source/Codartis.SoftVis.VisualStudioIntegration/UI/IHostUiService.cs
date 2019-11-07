using System;
using System.Threading.Tasks;
using System.Windows;
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
        [NotNull]
        Task ShowDiagramWindowAsync();

        /// <summary>
        /// Returns the main window of the host process.
        /// </summary>
        /// <returns>The main window of the host process.</returns>
        [NotNull]
        [ItemNotNull]
        Task<Window> GetMainWindowAsync();

        /// <summary>
        /// Runs an async method from a sync method.
        /// </summary>
        void Run([NotNull] Func<Task> asyncMethod);
    }
}