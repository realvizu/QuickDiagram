using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Events;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Listens to model events and corrects the model accordingly.
    /// </summary>
    public class ModelTrackingDiagramPlugin : ConnectorManipulatorDiagramPluginBase
    {
        public ModelTrackingDiagramPlugin(IDiagramShapeFactory diagramShapeFactory)
            : base(diagramShapeFactory)
        {
        }

        public override void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            base.Initialize(modelService, diagramService);

            ModelService.ModelChanged += OnModelChanged;
        }

        public override void Dispose()
        {
            ModelService.ModelChanged -= OnModelChanged;
        }

        private void OnModelChanged(ModelEventBase modelEvent)
        {
            var diagram = DiagramService.Diagram;

            switch (modelEvent)
            {
                case ModelNodeUpdatedEvent modelNodeUpdatedEvent:
                    var newModelNode = modelNodeUpdatedEvent.NewNode;
                    if (diagram.TryGetNode(newModelNode.Id, out var diagramNodeToUpdate))
                        DiagramService.UpdateDiagramNodeModelNode(diagramNodeToUpdate, newModelNode);
                    break;

                case ModelNodeRemovedEvent modelNodeRemovedEvent:
                    var removedModelNode = modelNodeRemovedEvent.RemovedNode;
                    if (diagram.TryGetNode(removedModelNode.Id, out var diagramNodeToRemove))
                        DiagramService.RemoveNode(diagramNodeToRemove.Id);
                    break;

                case ModelRelationshipAddedEvent modelRelationshipAddedEvent:
                    var addedRelationship = modelRelationshipAddedEvent.AddedRelationship;
                    ShowModelRelationshipIfBothEndsAreVisible(addedRelationship, diagram);
                    break;

                case ModelRelationshipRemovedEvent modelRelationshipRemovedEvent:
                    var modelRelationship = modelRelationshipRemovedEvent.RemovedRelationship;
                    if (diagram.TryGetConnector(modelRelationship.Id, out var diagramConnector))
                        DiagramService.RemoveConnector(diagramConnector.Id);
                    break;

                case ModelClearedEvent _:
                    DiagramService.ClearDiagram();
                    break;
            }
        }
    }
}
