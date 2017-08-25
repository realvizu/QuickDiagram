using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates diagram shape view model instances.
    /// </summary>
    public interface IDiagramShapeUiFactory
    {
        void Initialize(IDiagramShapeUiRepository diagramShapeUiRepository);

        DiagramNodeViewModelBase CreateDiagramNodeViewModel(IDiagramNode diagramNode);
        DiagramConnectorViewModel CreateDiagramConnectorViewModel(IDiagramConnector diagramConnector);
    }
}
