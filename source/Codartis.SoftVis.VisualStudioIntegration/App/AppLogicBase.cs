using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Base class for application logic classes.
    /// </summary>
    internal abstract class AppLogicBase
    {
        [NotNull] private readonly IAppServices _appServices;

        protected AppLogicBase([NotNull] IAppServices appServices)
        {
            _appServices = appServices;
        }

        [NotNull] protected IHostModelProvider HostModelProvider => _appServices.HostModelProvider;
        [NotNull] protected IRoslynModelService RoslynModelService => _appServices.RoslynModelService;
        [NotNull] protected IDiagramService DiagramService => _appServices.DiagramService;
        [NotNull] protected IApplicationUiService UiService => _appServices.ApplicationUiService;
    }
}