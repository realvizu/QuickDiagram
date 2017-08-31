using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Service;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Extends the visualization service with roslyn-specific operations.
    /// </summary>
    internal interface IRoslynVisualizationService : IVisualizationService
    {
        IRoslynModelService GetRoslynModelService();
        IRoslynDiagramService GetRoslynDiagramService(DiagramId diagramId);
        IRoslynUiService GetRoslynUiService(DiagramId diagramId);
    }
}
