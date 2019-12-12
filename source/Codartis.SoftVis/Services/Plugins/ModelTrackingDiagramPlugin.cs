using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Listens to model events and corrects the diagram accordingly.
    /// </summary>
    public sealed class ModelTrackingDiagramPlugin : ConnectorManipulatorDiagramPluginBase
    {
        [NotNull] private readonly IModelService _modelService;

        public ModelTrackingDiagramPlugin(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService)
            : base(diagramService)
        {
            _modelService = modelService;
            _modelService.ModelChanged += OnModelChanged;
        }

        public override void Dispose()
        {
            _modelService.ModelChanged -= OnModelChanged;
        }

        private void OnModelChanged(ModelEvent modelEvent)
        {
            DiagramService.UpdateModel(modelEvent.NewModel);

            foreach (var itemChange in modelEvent.ItemEvents)
                ProcessModelItemEvent(itemChange);
        }

        private void ProcessModelItemEvent(ModelItemEventBase modelItemEvent)
        {
            switch (modelItemEvent)
            {
                case ModelNodeUpdatedEvent modelNodeUpdated:
                    var updatedNode = modelNodeUpdated.NewNode;
                    DiagramService.UpdateModelNode(updatedNode);
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
            }
        }
    }
}