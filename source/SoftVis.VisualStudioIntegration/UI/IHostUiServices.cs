using System.Windows;
using System.Windows.Controls;

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
        void HostDiagram(ContentControl diagramControl);

        /// <summary>
        /// Shows the diagram-hosting tool window.
        /// </summary>
        void ShowDiagramWindow();

        /// <summary>
        /// Returns the main window of the host process.
        /// </summary>
        /// <returns>The main window of the host process.</returns>
        Window GetMainWindow();
    }
}
