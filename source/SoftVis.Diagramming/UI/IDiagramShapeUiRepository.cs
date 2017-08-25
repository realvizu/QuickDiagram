using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Provides lookup operations for diagram shape view models.
    /// </summary>
    public interface IDiagramShapeUiRepository
    {
        DiagramNodeViewModelBase GetDiagramNodeViewModel(IDiagramNode diagramNode);
        DiagramConnectorViewModel GetDiagramConnectorViewModel(IDiagramConnector diagramConnector);
    }
}
