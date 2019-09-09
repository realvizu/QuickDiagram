using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for factories that create view models from diagram shapes.
    /// </summary>
    public sealed class DiagramShapeUiFactory : IDiagramShapeUiFactory
    {
        private readonly IRelatedNodeTypeProvider _relatedNodeTypeProvider;

        public DiagramShapeUiFactory(IRelatedNodeTypeProvider relatedNodeTypeProvider)
        {
            _relatedNodeTypeProvider = relatedNodeTypeProvider;
        }

        private IModelService ModelService { get; set; }
        private IDiagramShapeUiRepository DiagramShapeUiRepository { get; set; }

        public void Initialize(IModelService modelService, IDiagramShapeUiRepository diagramShapeUiRepository)
        {
            ModelService = modelService;
            DiagramShapeUiRepository = diagramShapeUiRepository;
        }

        public IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramService diagramService,
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new DiagramNodeViewModel(ModelService, diagramService, _relatedNodeTypeProvider, focusTracker, diagramNode);
        }

        public IDiagramConnectorUi CreateDiagramConnectorUi(IDiagramService diagramService, IDiagramConnector diagramConnector)
        {
            if (!DiagramShapeUiRepository.TryGetDiagramNodeUi(diagramConnector.Source, out var sourceNode))
                throw new InvalidOperationException($"ViewModel not found for node {diagramConnector.Source}");

            if (!DiagramShapeUiRepository.TryGetDiagramNodeUi(diagramConnector.Target, out var targetNode))
                throw new InvalidOperationException($"ViewModel not found for node {diagramConnector.Target}");

            return new DiagramConnectorViewModel(
                ModelService,
                diagramService,
                diagramConnector,
                (DiagramNodeViewModel)sourceNode,
                (DiagramNodeViewModel)targetNode);
        }
    }
}