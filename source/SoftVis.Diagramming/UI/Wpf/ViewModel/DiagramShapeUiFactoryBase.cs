using System;
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

        public abstract IDiagramNodeUi CreateDiagramNodeUi(IDiagramService diagramService, IDiagramNode diagramNode);

        public virtual IDiagramConnectorUi CreateDiagramConnectorUi(IDiagramService diagramService, IDiagramConnector diagramConnector)
        {
            if (!DiagramShapeUiRepository.TryGetDiagramNodeUi(diagramConnector.Source, out var sourceNode))
                throw new InvalidOperationException($"ViewModel not found for node {diagramConnector.Source}");

            if (!DiagramShapeUiRepository.TryGetDiagramNodeUi(diagramConnector.Target, out var targetNode))
                throw new InvalidOperationException($"ViewModel not found for node {diagramConnector.Target}");

            return new DiagramConnectorViewModel(ModelService, diagramService, diagramConnector, 
                (DiagramNodeViewModelBase)sourceNode, (DiagramNodeViewModelBase)targetNode);
        }
    }
}
