using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Base class for all commands. Commands are the application logic of the package.
    /// </summary>
    internal abstract class CommandBase
    {
        /// <summary>
        /// The command can consume package services via this interface.
        /// </summary>
        private readonly IAppServices _appServices;

        protected IRoslynModelService ModelService => _appServices.RoslynModelService;
        protected IRoslynDiagramService DiagramServices => _appServices.RoslynDiagramService;
        protected IRoslynUiService UiService => _appServices.RoslynUiService;

        protected CommandBase(IAppServices appServices)
        {
            _appServices = appServices;
        }

        public virtual bool IsEnabled() => true;
    }
}