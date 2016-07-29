using EnvDTE80;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Operations to access the services of the host package and host environment.
    /// </summary>
    public interface IPackageServices
    {
        /// <summary>
        /// Creates a tool window of the given type.
        /// </summary>
        /// <typeparam name="TWindow">The type that implements the tool window. Must be a subclass of ToolWindowPane.</typeparam>
        /// <param name="instanceId">The instance id, for multi-instance tool windows. Omit for single-instance tool windows.</param>
        /// <returns>The created tool window.</returns>
        TWindow CreateToolWindow<TWindow>(int instanceId = 0) where TWindow : ToolWindowPane;

        /// <summary>
        /// Return the host service.
        /// </summary>
        /// <returns>The host service.</returns>
        DTE2 GetHostService();

        /// <summary>
        /// Returns the service of the host environment that can be used to access menu operations.
        /// </summary>
        /// <returns>The service to access menu operations in the host environment.</returns>
        OleMenuCommandService GetMenuCommandService();

        /// <summary>
        /// Returns the service that can be used to access documents that are open in the host environment.
        /// </summary>
        /// <returns>The service to access operations related to active documents.</returns>
        IVsRunningDocumentTable GetRunningDocumentTableService();

        /// <summary>
        /// Returns the Roslyn workspace that can be used to access the current solution's compilation.
        /// </summary>
        /// <returns>The Roslyn workspace of the current solution.</returns>
        VisualStudioWorkspace GetVisualStudioWorkspace();
    }
}
