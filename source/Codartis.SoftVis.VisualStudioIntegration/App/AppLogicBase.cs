using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Base class for application logic classes.
    /// </summary>
    internal abstract class AppLogicBase
    {
        private readonly IAppServices _appServices;

        protected IRoslynModelService ModelService => _appServices.RoslynModelService;
        protected IDiagramService DiagramServices => _appServices.DiagramService;
        protected IApplicationUiService UiService => _appServices.ApplicationUiService;

        protected AppLogicBase(IAppServices appServices)
        {
            _appServices = appServices;
        }
    }
}