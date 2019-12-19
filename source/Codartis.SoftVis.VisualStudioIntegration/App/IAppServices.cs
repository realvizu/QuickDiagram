using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Provides the services that the application commands can use.
    /// </summary>
    internal interface IAppServices
    {
        [NotNull] IRoslynWorkspaceProvider RoslynWorkspaceProvider { get; }
        [NotNull] IRoslynModelService RoslynModelService { get; }
        [NotNull] IDiagramService DiagramService { get; }
        [NotNull] IDiagramWindowService DiagramWindowService { get; }
        [NotNull] IHostUiService HostUiService { get; }
    }
}