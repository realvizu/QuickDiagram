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

        public override void Initialize(IReadOnlyModelStore modelStore, IDiagramStore diagramStore)
        {
            base.Initialize(modelStore, diagramStore);

            ModelStore.ModelChanged += OnModelChanged;
        }

        public override void Dispose()
        {
            ModelStore.ModelChanged -= OnModelChanged;
        }

        private void OnModelChanged(ModelEventBase modelEvent)
        {
            var diagram = DiagramStore.CurrentDiagram;

            switch (modelEvent)
            {
                case ModelNodeUpdatedEvent modelNodeUpdatedEvent:
                    var newModelNode = modelNodeUpdatedEvent.NewNode;
                    var diagramNodeToUpdate = diagram.GetNodeById(newModelNode.Id);
                    DiagramStore.UpdateDiagramNodeModelNode(diagramNodeToUpdate, newModelNode);
                    break;

                case ModelNodeRemovedEvent modelNodeRemovedEvent:
                    var removedModelNode = modelNodeRemovedEvent.RemovedNode;
                    var oldDiagramNodeToRemove = diagram.GetNodeById(removedModelNode.Id);
                    DiagramStore.RemoveNode(oldDiagramNodeToRemove);
                    break;

                case ModelRelationshipAddedEvent modelRelationshipAddedEvent:
                    var addedRelationship = modelRelationshipAddedEvent.AddedRelationship;
                    ShowModelRelationshipIfBothEndsAreVisible(addedRelationship, diagram);
                    break;

                case ModelRelationshipRemovedEvent modelRelationshipRemovedEvent:
                    var modelRelationship = modelRelationshipRemovedEvent.RemovedRelationship;
                    var diagramConnector = diagram.GetConnectorById(modelRelationship.Id);
                    DiagramStore.RemoveConnector(diagramConnector);
                    break;

                case ModelClearedEvent _:
                    DiagramStore.ClearDiagram();
                    break;
            }
        }
    }
}
