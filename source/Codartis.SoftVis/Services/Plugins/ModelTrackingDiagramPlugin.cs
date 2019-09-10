using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Listens to model events and corrects the diagram accordingly.
    /// </summary>
    public class ModelTrackingDiagramPlugin : ConnectorManipulatorDiagramPluginBase
    {
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
            DiagramService.UpdateModel(modelEvent.NewModel);

            switch (modelEvent)
            {
                case ModelNodeUpdatedEvent modelNodeUpdatedEvent:
                    DiagramService.UpdateModelNode(modelNodeUpdatedEvent.NewNode);
                    break;

                case ModelNodeRemovedEvent modelNodeRemovedEvent:
                    var removedNode = modelNodeRemovedEvent.RemovedNode;
                    DiagramService.RemoveNode(removedNode.Id);
                    break;

                case ModelRelationshipAddedEvent modelRelationshipAddedEvent:
                    var addedRelationship = modelRelationshipAddedEvent.AddedRelationship;
                    ShowModelRelationshipIfBothEndsAreVisible(addedRelationship, DiagramService.LatestDiagram);
                    break;

                case ModelRelationshipRemovedEvent modelRelationshipRemovedEvent:
                    var modelRelationship = modelRelationshipRemovedEvent.RemovedRelationship;
                    DiagramService.RemoveConnector(modelRelationship.Id);
                    break;

                case ModelClearedEvent _:
                    DiagramService.ClearDiagram();
                    break;
            }
        }
    }
}