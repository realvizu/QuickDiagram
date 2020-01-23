using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Operations to access the services of the host package and host environment.
    /// </summary>
    public interface IPackageServices
    {
        /// <summary>
        /// Return the host environment service.
        /// </summary>
        /// <returns>The host environment service.</returns>
        DTE2 GetHostEnvironmentService();

        /// <summary>
        /// Returns the service that provides info about the active text views.
        /// </summary>
        Task<IVsTextManager> GetTextManagerServiceAsync();

        /// <summary>
        /// Returns the service that converts old VS editor representation to new.
        /// </summary>
        Task<IVsEditorAdaptersFactoryService> GetEditorAdaptersFactoryServiceAsync();

        /// <summary>
        /// Returns the Roslyn workspace that can be used to access the current solution's compilation.
        /// </summary>
        Task<VisualStudioWorkspace> GetVisualStudioWorkspaceAsync();

        /// <summary>
        /// Returns the Diagram tool window. Also creates and initializes it if necessary.
        /// </summary>
        /// <returns></returns>
        Task<DiagramHostToolWindow> GetToolWindowAsync();
    }
}
