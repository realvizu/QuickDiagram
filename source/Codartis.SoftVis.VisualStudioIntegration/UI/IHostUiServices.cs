using System.Threading.Tasks;
using System.Windows;

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
        /// Returns the main window of the host process.
        /// </summary>
        /// <returns>The main window of the host process.</returns>
        Task<Window> GetMainWindowAsync();
    }
}
