using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
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
        /// Returns the service of the host environment that can be used to access menu operations.
        /// </summary>
        /// <returns>The service to access menu operations in the host environment.</returns>
        OleMenuCommandService GetMenuCommandService();

        /// <summary>
        /// Returns the service that provides info about the active text views.
        /// </summary>
        Task<IVsTextManager> GetTextManagerServiceAsync();

        /// <summary>
        /// Returns the service that converts old VS editor representation to new.
        /// </summary>
        Task<IVsEditorAdaptersFactoryService> GetEditorAdaptersFactoryServiceAsync();

        ///// <summary>
        ///// Returns the service that can be used to access documents that are open in the host environment.
        ///// </summary>
        ///// <returns>The service to access operations related to active documents.</returns>
        //IVsRunningDocumentTable GetRunningDocumentTableService();

        /// <summary>
        /// Returns the Roslyn workspace that can be used to access the current solution's compilation.
        /// </summary>
        Task<VisualStudioWorkspace> GetVisualStudioWorkspaceAsync();

        TWindow CreateToolWindow<TWindow>(int instanceId = 0) where TWindow : ToolWindowPane;
    }
}
