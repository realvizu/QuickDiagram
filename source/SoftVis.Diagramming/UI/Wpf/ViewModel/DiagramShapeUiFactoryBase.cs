using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for factories that create view models from diagram shapes.
    /// </summary>
    public abstract class DiagramShapeUiFactoryBase : IDiagramShapeUiFactory
    {
        protected IReadOnlyModelStore ModelStore { get; private set; }
        protected IDiagramShapeUiRepository DiagramShapeUiRepository { get; private set; }

        public void Initialize(IReadOnlyModelStore modelStore, IDiagramShapeUiRepository diagramShapeUiRepository)
        {
            ModelStore = modelStore;
            DiagramShapeUiRepository = diagramShapeUiRepository;
        }

        public abstract DiagramNodeViewModelBase CreateDiagramNodeViewModel(IReadOnlyDiagramStore diagramStore, IDiagramNode diagramNode);

        public virtual DiagramConnectorViewModel CreateDiagramConnectorViewModel(IReadOnlyDiagramStore diagramStore, IDiagramConnector diagramConnector)
        {
            var sourceNode = DiagramShapeUiRepository.GetDiagramNodeViewModel(diagramConnector.Source);
            var targetNode = DiagramShapeUiRepository.GetDiagramNodeViewModel(diagramConnector.Target);
            return new DiagramConnectorViewModel(ModelStore, diagramStore, diagramConnector, sourceNode, targetNode);

        }
    }
}
