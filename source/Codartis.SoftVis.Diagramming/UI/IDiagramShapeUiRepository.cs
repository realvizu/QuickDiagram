using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Provides lookup operations for diagram shape view models.
    /// </summary>
    public interface IDiagramShapeUiRepository
    {
        bool TryGetDiagramNodeUi(IDiagramNode diagramNode, out IDiagramNodeUi viewModel);
    }
}
