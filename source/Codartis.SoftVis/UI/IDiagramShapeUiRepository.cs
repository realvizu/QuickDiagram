using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Provides lookup operations for diagram shape view models.
    /// </summary>
    public interface IDiagramShapeUiRepository
    {
        bool TryGetDiagramNodeUi(ModelNodeId modelNodeId, out IDiagramNodeUi viewModel);
    }
}
