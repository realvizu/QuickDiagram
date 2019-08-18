using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram-related operations.
    /// </summary>
    public class DiagramService : IDiagramService
    {
        protected DiagramStore DiagramStore { get; }

        public DiagramService(IDiagram diagram)
        {
            DiagramStore = new DiagramStore(diagram);
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
        public void AddConnector(IDiagramConnector connector) => DiagramStore.AddConnector(connector);
        public void RemoveConnector(ModelRelationshipId connectorId) => DiagramStore.RemoveConnector(connectorId);
        public void UpdateConnectorRoute(ModelRelationshipId connectorId, Route newRoute) => DiagramStore.UpdateConnectorRoute(connectorId, newRoute);
        public void ClearDiagram() => DiagramStore.ClearDiagram();

        //public abstract ConnectorType GetConnectorType(ModelRelationshipStereotype modelRelationshipStereotype);

        public IDiagramNode GetDiagramNode(ModelNodeId id) => Diagram.GetNode(id);

        //public IDiagramNode ShowModelNode(IModelNode modelNode)
        //{
        //    return DiagramStore.Diagram.TryGetNode(modelNode.Id).Match(
        //        node => node,
        //        () => AddNode(modelNode)
        //    );
        //}

        //private IDiagramNode AddNode(IModelNode modelNode)
        //{
        //   var maybeParentModelNode =  ModelService.Model.TryGetParentNode(modelNode.Id);

        //    var diagramNode = new DiagramNode(modelNode, maybeParentModelNode.FromMaybe());
        //    DiagramStore.AddNode(diagramNode);
        //    return diagramNode;
        //}

        //public IReadOnlyList<IDiagramNode> ShowModelNodes(
        //    IEnumerable<IModelNode> modelNodes,
        //    CancellationToken cancellationToken,
        //    IIncrementalProgress progress)
        //{
        //    var diagramNodes = new List<IDiagramNode>();

        //    foreach (var modelNode in modelNodes)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        var diagramNode = ShowModelNode(modelNode);
        //        diagramNodes.Add(diagramNode);

        //        progress?.Report(1);
        //    }

        //    return diagramNodes;
        //}

        //public void HideModelNode(ModelNodeId modelNodeId)
        //{
        //    var diagram = DiagramStore.Diagram;
        //    if (diagram.NodeExists(modelNodeId))
        //        RemoveNode(modelNodeId, diagram);
        //}

        private void RemoveNode(ModelNodeId modelNodeId, IDiagram diagram)
        {
            var diagramConnectors = diagram.GetConnectorsByNode(modelNodeId);
            foreach (var diagramConnector in diagramConnectors)
                DiagramStore.RemoveConnector(diagramConnector.Id);

            DiagramStore.RemoveNode(modelNodeId);
        }

        //public void ShowModelRelationship(IModelRelationship modelRelationship)
        //{
        //    if (modelRelationship.Stereotype == ModelRelationshipStereotype.Containment)
        //        return;

        //    var diagram = DiagramStore.Diagram;
        //    if (diagram.ConnectorExists(modelRelationship.Id))
        //        return;

        //    var diagramConnectorSpec = DiagramShapeFactory.CreateDiagramConnector(modelRelationship);
        //    DiagramStore.AddConnector(diagramConnectorSpec);
        //}

        //public void HideModelRelationship(ModelRelationshipId modelRelationshipId)
        //{
        //    var diagram = DiagramStore.Diagram;
        //    if (diagram.ConnectorExists(modelRelationshipId))
        //        DiagramStore.RemoveConnector(modelRelationshipId);
        //}

        public Maybe<IContainerDiagramNode> TryGetContainerNode(IDiagramNode diagramNode)
        {
            return diagramNode.ParentNodeId == null
                ? Maybe<IContainerDiagramNode>.Nothing
                : Maybe.Create((IContainerDiagramNode)GetDiagramNode(diagramNode.ParentNodeId.Value));
        }

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            var diagram = Diagram;
            return modelNodeIds
                .Select(i => diagram.TryGetNode(i).Match(j => j.Rect, () => Rect2D.Zero))
                .Union();
        }

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            throw new NotImplementedException();
        }
    }
}