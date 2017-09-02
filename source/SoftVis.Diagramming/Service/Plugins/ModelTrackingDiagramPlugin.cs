using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Events;

namespace Codartis.SoftVis.Service.Plugins
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
                    if (diagram.TryGetNodeById(newModelNode.Id, out var diagramNodeToUpdate))
                        DiagramService.UpdateDiagramNodeModelNode(diagramNodeToUpdate, newModelNode);
                    break;

                case ModelNodeRemovedEvent modelNodeRemovedEvent:
                    var removedModelNode = modelNodeRemovedEvent.RemovedNode;
                    if (diagram.TryGetNodeById(removedModelNode.Id, out var diagramNodeToRemove))
                        DiagramService.RemoveNode(diagramNodeToRemove);
                    break;

                case ModelRelationshipAddedEvent modelRelationshipAddedEvent:
                    var addedRelationship = modelRelationshipAddedEvent.AddedRelationship;
                    ShowModelRelationshipIfBothEndsAreVisible(addedRelationship, diagram);
                    break;

                case ModelRelationshipRemovedEvent modelRelationshipRemovedEvent:
                    var modelRelationship = modelRelationshipRemovedEvent.RemovedRelationship;
                    if (diagram.TryGetConnectorById(modelRelationship.Id, out var diagramConnector))
                        DiagramService.RemoveConnector(diagramConnector);
                    break;

                case ModelClearedEvent _:
                    DiagramService.ClearDiagram();
                    break;
            }
        }
    }
}
