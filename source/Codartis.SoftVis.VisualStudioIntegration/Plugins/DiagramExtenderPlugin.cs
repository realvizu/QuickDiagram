using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.SoftVis.Services.Plugins;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Extends the diagram with parent/child containment relationships when the model or the diagram changes.
    /// </summary>
    public sealed class DiagramExtenderPlugin : DiagramPluginBase
    {
        [NotNull]
        private static readonly ModelNodeStereotype[] TypeMemberModelNodeStereotypes =
        {
            ModelNodeStereotypes.Event,
            ModelNodeStereotypes.Field,
            ModelNodeStereotypes.Method,
            ModelNodeStereotypes.Property
        };

        [NotNull] private readonly IModelService _modelService;

        public DiagramExtenderPlugin(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService)
            : base(diagramService)
        {
            _modelService = modelService;
            _modelService.ModelChanged += OnModelChanged;
            diagramService.AfterDiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            _modelService.ModelChanged -= OnModelChanged;
            DiagramService.AfterDiagramChanged -= OnDiagramChanged;
        }

        private void OnModelChanged(ModelEvent modelEvent)
        {
            foreach (var itemEvent in modelEvent.ItemEvents)
                ProcessModelItemEvent(itemEvent, modelEvent.NewModel);
        }

        private void ProcessModelItemEvent([NotNull] ModelItemEventBase itemEvent, [NotNull] IModel model)
        {
            switch (itemEvent)
            {
                case ModelNodeAddedEvent modelNodeAddedEvent:
                    ShowAsParentNode(modelNodeAddedEvent.AddedNode, model);
                    break;
                case ModelRelationshipAddedEvent modelRelationshipAddedEvent
                    when modelRelationshipAddedEvent.AddedRelationship.Stereotype == ModelRelationshipStereotype.Containment:
                    ShowContainment(modelRelationshipAddedEvent.AddedRelationship);
                    break;
            }
        }

        private void ShowAsParentNode([NotNull] IModelNode addedModelNode, [NotNull] IModel model)
        {
            var diagram = DiagramService.LatestDiagram;
            var childNodes = model.GetRelatedNodes(addedModelNode.Id, CommonDirectedModelRelationshipTypes.Contained);

            if (childNodes.Any(i => diagram.NodeExists(i.Id)))
                DiagramService.AddNode(addedModelNode.Id);
        }

        private void ShowContainment([NotNull] IModelRelationship addedRelationship)
        {
            var diagram = DiagramService.LatestDiagram;

            if (diagram.NodeExists(addedRelationship.Source) && diagram.NodeExists(addedRelationship.Target))
                DiagramService.UpdateParent(addedRelationship.Target, addedRelationship.Source);
        }

        private void OnDiagramChanged(DiagramEvent diagramEvent)
        {
            foreach (var shapeEvent in diagramEvent.ShapeEvents)
                ProcessShapeEvent(shapeEvent);
        }

        private void ProcessShapeEvent([NotNull] DiagramShapeEventBase shapeEvent)
        {
            switch (shapeEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    ShowParentNode(diagramNodeAddedEvent.NewNode);
                    break;
            }
        }

        private void ShowParentNode([NotNull] IDiagramNode diagramNode)
        {
            var modelNode = diagramNode.ModelNode;

            if (!modelNode.Stereotype.In(TypeMemberModelNodeStereotypes))
                return;

            var parentModelNode = _modelService.LatestModel.GetRelatedNodes(modelNode.Id, CommonDirectedModelRelationshipTypes.Container).SingleOrDefault();
            if (parentModelNode != null)
                DiagramService.AddNode(parentModelNode.Id);
        }
    }
}