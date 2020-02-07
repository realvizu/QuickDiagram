using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A mutable diagram that collects change events.
    /// NOT thread-safe.
    /// </summary>
    public sealed class DiagramMutator : IDiagramMutator
    {
        [NotNull] private IModel _model;
        [NotNull] private readonly IConnectorTypeResolver _connectorTypeResolver;
        private readonly double _childrenAreaPadding;

        [NotNull] private readonly IDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IDictionary<ModelRelationshipId, IDiagramConnector> _connectors;
        [NotNull] private readonly List<DiagramShapeEventBase> _shapeEvents;

        public DiagramMutator(
            [NotNull] IDiagram diagram,
            [NotNull] IConnectorTypeResolver connectorTypeResolver,
            double childrenAreaPadding)
        {
            _model = diagram.Model;
            _connectorTypeResolver = connectorTypeResolver;
            _childrenAreaPadding = childrenAreaPadding;
            _nodes = diagram.Nodes.ToDictionary(i => i.Id);
            _connectors = diagram.Connectors.ToDictionary(i => i.Id);
            _shapeEvents = new List<DiagramShapeEventBase>();
        }

        private Point2D ChildrenAreaPaddingVector => new Point2D(_childrenAreaPadding, _childrenAreaPadding);

        public DiagramEvent GetDiagramEvent()
        {
            var diagram = new ImmutableDiagram(
                _model,
                _nodes.ToImmutableDictionary(),
                _connectors.ToImmutableDictionary());

            return new DiagramEvent(diagram, _shapeEvents);
        }

        public void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            if (_nodes.ContainsKey(nodeId))
                return;

            var maybeModelNode = _model.TryGetNode(nodeId);
            if (!maybeModelNode.HasValue)
                throw new Exception($"Node {nodeId} not found in model.");

            var newNode = CreateNode(maybeModelNode.Value).WithParentNodeId(parentNodeId);

            _nodes.Add(newNode.Id, newNode);

            _shapeEvents.Add(new DiagramNodeAddedEvent(newNode));
        }

        public void UpdateNodeHeaderSize(ModelNodeId nodeId, Size2D newSize)
        {
            UpdateNodeCore(
                nodeId,
                i => i.WithHeaderSize(newSize),
                DiagramNodeMember.HeaderSize);
        }

        public void UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter)
        {
            UpdateNodeCore(
                nodeId,
                i => i.WithCenter(newCenter),
                DiagramNodeMember.Position);
        }

        public void UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            UpdateNodeCore(
                nodeId,
                i => i.WithTopLeft(newTopLeft),
                DiagramNodeMember.Position);
        }

        private void UpdateNodeCore(
            ModelNodeId nodeId,
            [NotNull] Func<IDiagramNode, IDiagramNode> nodeMutatorFunc,
            DiagramNodeMember updatedMember)
        {
            if (!_nodes.ContainsKey(nodeId))
                return;

            var oldNode = _nodes[nodeId];
            var newNode = nodeMutatorFunc(oldNode);

            _nodes[newNode.Id] = newNode;

            _shapeEvents.Add(new DiagramNodeChangedEvent(oldNode, newNode, updatedMember));

            UpdateParentNodesRecursive(newNode);
        }

        private void UpdateParentNodesRecursive([NotNull] IDiagramNode updatedNode)
        {
            if (!updatedNode.HasParent)
                return;

            var parentNode = _nodes[updatedNode.ParentNodeId.Value];

            var newChildrenAreaSize = CalculateChildrenAreaSize(parentNode.Id);
            if (newChildrenAreaSize.IsUndefined && parentNode.ChildrenAreaSize.IsUndefined ||
                newChildrenAreaSize == parentNode.ChildrenAreaSize)
                return;

            var updatedParentNode = parentNode.WithChildrenAreaSize(newChildrenAreaSize);

            UpdateNodeCore(
                parentNode.Id,
                _ => updatedParentNode,
                DiagramNodeMember.ChildrenAreaSize);

            UpdateParentNodesRecursive(updatedParentNode);
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            if (!_nodes.ContainsKey(nodeId))
                return;

            var oldNode = _nodes[nodeId];

            RemoveAllConnectorsOfNode(oldNode);
            RemoveChildNodes(oldNode);
            _nodes.Remove(nodeId);

            _shapeEvents.Add(new DiagramNodeRemovedEvent(oldNode));
        }

        private void RemoveChildNodes([NotNull] IDiagramNode node)
        {
            foreach (var childNode in GetChildNodes(node.Id))
                RemoveNode(childNode.Id);
        }

        private void RemoveAllConnectorsOfNode([NotNull] IDiagramNode node)
        {
            foreach (var connector in GetConnectorsByNode(node.Id))
                RemoveConnector(connector.Id);
        }

        [NotNull]
        [ItemNotNull]
        private List<IDiagramConnector> GetConnectorsByNode(ModelNodeId nodeId)
        {
            return _connectors.Values
                .Where(i => i.Source == nodeId || i.Target == nodeId)
                .ToList();
        }

        public void AddConnector(ModelRelationshipId relationshipId)
        {
            if (_connectors.ContainsKey(relationshipId))
                return;

            var maybeRelationship = _model.TryGetRelationship(relationshipId);
            if (!maybeRelationship.HasValue)
                throw new InvalidOperationException($"Relationship {relationshipId} does not exist.");

            var newConnector = CreateConnector(maybeRelationship.Value);
            _connectors.Add(relationshipId, newConnector);

            _shapeEvents.Add(new DiagramConnectorAddedEvent(newConnector));
        }

        public void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute)
        {
            if (!_connectors.ContainsKey(relationshipId))
                return;

            var oldConnector = _connectors[relationshipId];
            var newConnector = oldConnector.WithRoute(newRoute);

            _connectors[relationshipId] = newConnector;

            _shapeEvents.Add(new DiagramConnectorRouteChangedEvent(oldConnector, newConnector));
        }

        public void RemoveConnector(ModelRelationshipId relationshipId)
        {
            if (!_connectors.ContainsKey(relationshipId))
                return;

            var oldConnector = _connectors[relationshipId];

            _connectors.Remove(relationshipId);

            _shapeEvents.Add(new DiagramConnectorRemovedEvent(oldConnector));
        }

        /// <remarks>
        /// When the model (backing the diagram) is updated there can be an interim time
        /// when there are diagram shapes that does not exist in the model any more.
        /// This is fine because those model events will eventually arrive that remove such shapes.
        /// </remarks>
        public void UpdateModel(IModel newModel)
        {
            _model = newModel;
        }

        public void UpdateModelNode(IModelNode updatedModelNode)
        {
            UpdateNodeCore(
                updatedModelNode.Id,
                i => i.WithModelNode(updatedModelNode),
                DiagramNodeMember.ModelNode);
        }

        public void ApplyLayout(GroupLayoutInfo diagramLayout)
        {
            throw new NotImplementedException();
        }

        public void ApplyLayout(LayoutInfo layoutInfo)
        {
            foreach (var vertexRecKeyValue in layoutInfo.VertexRects)
                UpdateNodeTopLeft(vertexRecKeyValue.Key, CalculateAbsolutePosition(vertexRecKeyValue.Key, vertexRecKeyValue.Value));

            foreach (var edgeRouteKeyValue in layoutInfo.EdgeRoutes)
                UpdateConnectorRoute(edgeRouteKeyValue.Key, edgeRouteKeyValue.Value);
        }

        private Point2D CalculateAbsolutePosition(ModelNodeId nodeId, Rect2D rect)
        {
            if (!_nodes.ContainsKey(nodeId))
                throw new Exception($"Diagram node: {nodeId} not found.");

            return _nodes[nodeId].ParentNodeId.Match(
                some => _nodes[some].TopLeft + ChildrenAreaPaddingVector + rect.TopLeft,
                () => rect.TopLeft);
        }

        public void Clear()
        {
            foreach (var connector in _connectors.Values.ToList())
                RemoveConnector(connector.Id);

            foreach (var node in _nodes.Values.ToList())
                RemoveNode(node.Id);
        }

        [NotNull]
        private static IDiagramNode CreateNode([NotNull] IModelNode modelNode)
        {
            return new DiagramNode(modelNode);
        }

        [NotNull]
        private IDiagramConnector CreateConnector([NotNull] IModelRelationship relationship)
        {
            var connectorType = _connectorTypeResolver.GetConnectorType(relationship.Stereotype);
            return new DiagramConnector(relationship, connectorType);
        }

        private Size2D CalculateChildrenAreaSize(ModelNodeId parentNodeId)
        {
            return GetChildNodes(parentNodeId)
                .Select(i => i.Rect).Where(i => i.IsDefined()).Union()
                .WithMargin(_childrenAreaPadding).Size;
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<IDiagramNode> GetChildNodes(ModelNodeId diagramNodeId)
        {
            return _nodes.Values.Where(i => i.ParentNodeId.ToNullable() == diagramNodeId);
        }
    }
}