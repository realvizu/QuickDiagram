using System.Threading.Tasks;
using EnvDTE80;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Operations to access the services of the host Visual Studio.
    /// </summary>
    public interface IVisualStudioServices
    {
        /// <summary>
        /// Return the host environment service.
        /// </summary>
        DTE2 GetHostEnvironmentService();

        /// <summary>
        /// Returns the service of the host environment that can be used to access menu operations.
        /// </summary>
        OleMenuCommandService GetMenuCommandService();

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
        [NotNull]
        [ItemNotNull]
        Task<VisualStudioWorkspace> GetVisualStudioWorkspaceAsync();

        /// <summary>
        /// Creates a tool window of the given type.
        /// </summary>
        /// <typeparam name="TWindow">The type that implements the tool window. Must be a subclass of ToolWindowPane.</typeparam>
        /// <param name="instanceId">The instance id, for multi-instance tool windows. Omit for single-instance tool windows.</param>
        /// <returns>The created tool window.</returns>
        Task<TWindow> CreateToolWindowAsync<TWindow>(int instanceId = 0) where TWindow : ToolWindowPane;

        Task ShowToolWindowAsync<TWindow>(int instanceId = 0) where TWindow : ToolWindowPane;
    }
}
