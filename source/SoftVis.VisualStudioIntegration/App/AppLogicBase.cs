using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
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
        protected IRoslynDiagramService DiagramServices => _appServices.RoslynDiagramService;
        protected IApplicationUiService UiService => _appServices.ApplicationUiService;

        protected AppLogicBase(IAppServices appServices)
        {
            _appServices = appServices;
        }
    }
}