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

        [NotNull] protected IRoslynWorkspaceProvider RoslynWorkspaceProvider => _appServices.RoslynWorkspaceProvider;
        [NotNull] protected IRoslynBasedModelService RoslynBasedModelService => _appServices.RoslynBasedModelService;
        [NotNull] protected IDiagramService DiagramService => _appServices.DiagramService;
        [NotNull] protected IDiagramWindowService DiagramWindowService => _appServices.DiagramWindowService;
        [NotNull] protected IHostUiService HostUiService => _appServices.HostUiService;
    }
}