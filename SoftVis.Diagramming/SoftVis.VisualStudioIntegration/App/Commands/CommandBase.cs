using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for all commands. Commands are the application logic of the package.
    /// </summary>
    internal abstract class CommandBase
    {
        /// <summary>
        /// The command can consume package services via this interface.
        /// </summary>
        protected IAppServices AppServices { get; }

        protected IHostUiServices HostUiServices => AppServices.GetHostUiServices();
        protected IHostWorkspaceServices HostWorkspaceServices => AppServices.GetHostWorkspaceServices();
        protected IModelServices ModelServices => AppServices.GetModelServices();
        protected IDiagramServices DiagramServices => AppServices.GetDiagramServices();
        protected IUiServices UiServices => AppServices.GetUiServices();

        protected CommandBase(IAppServices appServices)
        {
            AppServices = appServices;
        }
    }
}