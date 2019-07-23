using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram-related operations.
    /// </summary>
    public abstract class DiagramService : IDiagramService
    {
        protected DiagramStore DiagramStore { get; }
        protected IModelService ModelService { get; }
        protected IDiagramShapeFactory DiagramShapeFactory { get; }

        protected DiagramService(IDiagram diagram, IModelService modelService, IDiagramShapeFactory diagramShapeFactory)
        {
            DiagramStore = new DiagramStore(diagram);
            ModelService = modelService;
            DiagramShapeFactory = diagramShapeFactory;
        }

        public IDiagram Diagram => DiagramStore.Diagram;

        public event Action<DiagramEventBase> DiagramChanged
        {
            add => DiagramStore.DiagramChanged += value;
            remove => DiagramStore.DiagramChanged -= value;
        }

        public void AddNode(IDiagramNode node) => DiagramStore.AddNode(node);
        public void RemoveNode(ModelNodeId nodeId) => DiagramStore.RemoveNode(nodeId);

        public void UpdateDiagramNodeModelNode(IDiagramNode diagramNode, IModelNode newModelNode)
            => DiagramStore.UpdateDiagramNodeModelNode(diagramNode, newModelNode);

        public void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize) => DiagramStore.UpdateDiagramNodeSize(diagramNode, newSize);
        public void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter) => DiagramStore.UpdateDiagramNodeCenter(diagramNode, newCenter);
        public void UpdateDiagramNodeTopLeft(IDiagramNode diagramNode, Point2D newTopLeft) => DiagramStore.UpdateDiagramNodeTopLeft(diagramNode, newTopLeft);
        public void AddConnector(DiagramConnectorSpecification connectorSpec) => DiagramStore.AddConnector(connectorSpec);
        public void RemoveConnector(ModelRelationshipId connectorId) => DiagramStore.RemoveConnector(connectorId);
        public void UpdateDiagramConnectorRoute(IDiagramConnector connector, Route newRoute) => DiagramStore.UpdateDiagramConnectorRoute(connector, newRoute);
        public void ClearDiagram() => DiagramStore.ClearDiagram();

        public abstract ConnectorType GetConnectorType(ModelRelationshipStereotype modelRelationshipStereotype);

        public IDiagramNode GetDiagramNode(ModelNodeId id) => Diagram.GetNode(id);

        public IDiagramNode ShowModelNode(IModelNode modelNode)
        {
            return DiagramStore.Diagram.TryGetNode(modelNode.Id).Match(
                node => node,
                () => AddNode(modelNode)
            );
        }

        private IDiagramNode AddNode(IModelNode modelNode)
        {
            ModelService.TryGetParentNode(modelNode.Id, out var parentModelNode);

            var diagramNode = DiagramShapeFactory.CreateDiagramNode(this, modelNode, parentModelNode);
            DiagramStore.AddNode(diagramNode);
            return diagramNode;
        }

        public IReadOnlyList<IDiagramNode> ShowModelNodes(
            IEnumerable<IModelNode> modelNodes,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            var diagramNodes = new List<IDiagramNode>();

            foreach (var modelNode in modelNodes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var diagramNode = ShowModelNode(modelNode);
                diagramNodes.Add(diagramNode);

                progress?.Report(1);
            }

            return diagramNodes;
        }

        public void HideModelNode(ModelNodeId modelNodeId)
        {
            var diagram = DiagramStore.Diagram;
            if (diagram.NodeExists(modelNodeId))
                RemoveNode(modelNodeId, diagram);
        }

        private void RemoveNode(ModelNodeId modelNodeId, IDiagram diagram)
        {
            var diagramConnectors = diagram.GetConnectorsByNode(modelNodeId);
            foreach (var diagramConnector in diagramConnectors)
                DiagramStore.RemoveConnector(diagramConnector.Id);

            DiagramStore.RemoveNode(modelNodeId);
        }

        public void ShowModelRelationship(IModelRelationship modelRelationship)
        {
            if (modelRelationship.Stereotype == ModelRelationshipStereotype.Containment)
                return;

            var diagram = DiagramStore.Diagram;
            if (diagram.ConnectorExists(modelRelationship.Id))
                return;

            var diagramConnectorSpec = DiagramShapeFactory.CreateDiagramConnectorSpec(this, modelRelationship);
            DiagramStore.AddConnector(diagramConnectorSpec);
        }

        public void HideModelRelationship(ModelRelationshipId modelRelationshipId)
        {
            var diagram = DiagramStore.Diagram;
            if (diagram.ConnectorExists(modelRelationshipId))
                DiagramStore.RemoveConnector(modelRelationshipId);
        }

        public Maybe<IContainerDiagramNode> TryGetContainerNode(IDiagramNode diagramNode)
        {
            return ModelService.TryGetParentNode(diagramNode.Id, out var parentModelNode)
                ? Diagram.TryGetNode(parentModelNode.Id).Cast<IDiagramNode, IContainerDiagramNode>()
                : Maybe<IContainerDiagramNode>.Nothing;
        }

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            var diagram = Diagram;
            return modelNodeIds
                .Select(i => diagram.TryGetNode(i).Match(j => j.Rect, () => Rect2D.Zero))
                .Union();
        }
    }
}