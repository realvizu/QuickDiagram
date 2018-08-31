using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// These are the application services that the application commands can use.
    /// </summary>
    internal interface IAppServices
    {
        IRoslynModelService RoslynModelService { get; }
        IRoslynDiagramService RoslynDiagramService { get; }
        IApplicationUiService ApplicationUiService { get; }
    }
}
