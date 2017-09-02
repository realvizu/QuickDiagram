using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for factories that create view models from diagram shapes.
    /// </summary>
    public abstract class DiagramShapeUiFactoryBase : IDiagramShapeUiFactory
    {
        protected IModelService ModelService { get; private set; }
        protected IDiagramShapeUiRepository DiagramShapeUiRepository { get; private set; }

        public void Initialize(IModelService modelService, IDiagramShapeUiRepository diagramShapeUiRepository)
        {
            ModelService = modelService;
            DiagramShapeUiRepository = diagramShapeUiRepository;
        }

        public abstract DiagramNodeViewModelBase CreateDiagramNodeViewModel(IDiagramService diagramService, IDiagramNode diagramNode);

        public virtual DiagramConnectorViewModel CreateDiagramConnectorViewModel(IDiagramService diagramService, IDiagramConnector diagramConnector)
        {
            var sourceNode = DiagramShapeUiRepository.GetDiagramNodeViewModel(diagramConnector.Source);
            var targetNode = DiagramShapeUiRepository.GetDiagramNodeViewModel(diagramConnector.Target);
            return new DiagramConnectorViewModel(ModelService, diagramService, diagramConnector, sourceNode, targetNode);
        }
    }
}
