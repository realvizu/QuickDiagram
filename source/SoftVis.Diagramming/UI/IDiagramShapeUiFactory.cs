using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates diagram shape view model instances.
    /// </summary>
    public interface IDiagramShapeUiFactory
    {
        void Initialize(IReadOnlyModelStore modelStore, IDiagramShapeUiRepository diagramShapeUiRepository);

        DiagramNodeViewModelBase CreateDiagramNodeViewModel(IReadOnlyDiagramStore diagramStore, IDiagramNode diagramNode);
        DiagramConnectorViewModel CreateDiagramConnectorViewModel(IReadOnlyDiagramStore diagramStore, IDiagramConnector diagramConnector);
    }
}
