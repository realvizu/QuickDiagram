using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.Util;

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
            var diagram = DiagramService.LatestDiagram;

            switch (modelEvent)
            {
                case ModelNodeUpdatedEvent modelNodeUpdatedEvent:
                    var newModelNode = modelNodeUpdatedEvent.NewNode;
                    diagram
                        .TryGetNode(newModelNode.Id)
                        .Match(node => DiagramService.UpdateDiagramNodeModelNode(node, newModelNode));
                    break;

                case ModelNodeRemovedEvent modelNodeRemovedEvent:
                    var removedModelNode = modelNodeRemovedEvent.RemovedNode;
                    diagram
                        .TryGetNode(removedModelNode.Id)
                        .Match(node => DiagramService.RemoveNode(node.Id));
                    break;

                case ModelRelationshipAddedEvent modelRelationshipAddedEvent:
                    var addedRelationship = modelRelationshipAddedEvent.AddedRelationship;
                    ShowModelRelationshipIfBothEndsAreVisible(addedRelationship, diagram);
                    break;

                case ModelRelationshipRemovedEvent modelRelationshipRemovedEvent:
                    var modelRelationship = modelRelationshipRemovedEvent.RemovedRelationship;
                    diagram
                        .TryGetConnector(modelRelationship.Id)
                        .Match(connector => DiagramService.RemoveConnector(connector.Id));
                    break;

                case ModelClearedEvent _:
                    DiagramService.ClearDiagram();
                    break;
            }
        }
    }
}