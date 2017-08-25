using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates diagram UI instances.
    /// </summary>
    public interface IDiagramUiFactory
    {
        IDiagramUi Create(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
            IDiagramShapeUiFactory diagramShapeUiFactory, double minZoom, double maxZoom, double initialZoom);
    }
}
