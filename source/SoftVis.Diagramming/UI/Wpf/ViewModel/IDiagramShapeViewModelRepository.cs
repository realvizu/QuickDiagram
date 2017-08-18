using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Provides lookup operations for diagram shape view models.
    /// </summary>
    public interface IDiagramShapeViewModelRepository
    {
        DiagramNodeViewModelBase GetDiagramNodeViewModel(IDiagramNode diagramNode);
        DiagramConnectorViewModel GetDiagramConnectorViewModel(IDiagramConnector diagramConnector);
    }
}
