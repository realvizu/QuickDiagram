using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Provides lookup operations for diagram shape view models.
    /// </summary>
    public interface IDiagramShapeUiRepository
    {
        bool TryGetDiagramNodeViewModel(IDiagramNode diagramNode, out DiagramNodeViewModelBase viewModel);
        bool TryGetDiagramConnectorViewModel(IDiagramConnector diagramConnector, out DiagramConnectorViewModel viewModel);
    }
}
