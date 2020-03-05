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
        private static readonly DiagramNodeMember[] ParentNodeChildrenAreaSizeAffectingMembers =
        {
            DiagramNodeMember.Size,
            DiagramNodeMember.ChildrenAreaSize,
            DiagramNodeMember.RelativePosition,
            DiagramNodeMember.ParentNode
        };

        private static readonly DiagramNodeMember[] AbsolutePositionAffectingMembers =
        {
            DiagramNodeMember.RelativePosition,
        };

        private static readonly DiagramNodeMember[] ChildrenAbsolutePositionAffectingMembers =
        {
            DiagramNodeMember.AbsolutePosition,
            DiagramNodeMember.ChildrenAreaTopLeft,
        };

        [NotNull] private readonly IDiagram _initialDiagram;
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
            _initialDiagram = diagram;
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
            var newDiagram = new ImmutableDiagram(
                _model,
                _nodes.ToImmutableDictionary(),
                _connectors.ToImmutableDictionary());

            return new DiagramEvent(_initialDiagram, newDiagram, _shapeEvents);
        }

        public void AddNode(ModelNodeId nodeId)
        {
            var maybeModelNode = _model.TryGetNode(nodeId);
            if (!maybeModelNode.HasValue)
                throw new Exception($"Node {nodeId} not found in model.");

            AddNodeCore(maybeModelNode.Value);

            var parentNodeId = GetParentNodeId(nodeId);
            if (parentNodeId != null && DiagramNodeExists(parentNodeId.Value))
                UpdateParent(nodeId, parentNodeId);

            var childNodeIds = GetChildDiagramNodeIds(nodeId);
            foreach (var childNodeId in childNodeIds)
                UpdateParent(childNodeId, nodeId);
        }

        public void UpdateParent(ModelNodeId nodeId, ModelNodeId? parentNodeId)
        {
            UpdateNodeCore(
                nodeId,
                i => i.WithParentNode(parentNodeId, CalculateHierarchyLevel(parentNodeId)),
                DiagramNodeMember.ParentNode);
        }

        private int CalculateHierarchyLevel(ModelNodeId? parentNodeId)
        {
            return parentNodeId.HasValue && DiagramNodeExists(parentNodeId.Value)
                ? GetDiagramNode(parentNodeId.Value).HierarchyLevel + 1
                : 0;
        }

        public void UpdateSize(ModelNodeId nodeId, Size2D newSize)
        {
            UpdateNodeCore(
                nodeId,
                i => i.WithSize(newSize),
                DiagramNodeMember.Size);
        }

        public void UpdateNodeRelativeTopLeft(ModelNodeId nodeId, Point2D newRelativeTopLeft)
        {
            UpdateNodeCore(
                nodeId,
                i => i.WithRelativeTopLeft(newRelativeTopLeft),
                DiagramNodeMember.RelativePosition);
        }

        public void UpdateChildrenAreaTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            UpdateNodeCore(
                nodeId,
                i => i.WithChildrenAreaTopLeft(newTopLeft),
                DiagramNodeMember.ChildrenAreaTopLeft);
        }

        private void AddNodeCore([NotNull] IModelNode modelNode)
        {
            if (DiagramNodeExists(modelNode.Id))
                return;

            var newNode = CreateNode(modelNode);
            _nodes.Add(newNode.Id, newNode);

            _shapeEvents.Add(new DiagramNodeAddedEvent(newNode));
        }

        private void UpdateNodeCore(
            ModelNodeId nodeId,
            [NotNull] Func<IDiagramNode, IDiagramNode> nodeMutatorFunc,
            DiagramNodeMember updatedMember)
        {
            if (!DiagramNodeExists(nodeId))
                return;

            var oldNode = GetDiagramNode(nodeId);
            var newNode = nodeMutatorFunc(oldNode);

            _nodes[newNode.Id] = newNode;

            _shapeEvents.Add(new DiagramNodeChangedEvent(oldNode, newNode, updatedMember));

            if (updatedMember.In(ParentNodeChildrenAreaSizeAffectingMembers))
                UpdateParentNodeChildrenAreaSize(newNode);

            if (updatedMember.In(AbsolutePositionAffectingMembers))
                UpdateAbsolutePosition(newNode);

            if (updatedMember.In(ChildrenAbsolutePositionAffectingMembers))
                UpdateChildrenAbsolutePosition(newNode);
        }

        private void UpdateAbsolutePosition([NotNull] IDiagramNode updatedNode)
        {
            var newAbsoluteTopLeft = CalculateAbsolutePosition(updatedNode.Id);

            UpdateNodeCore(
                updatedNode.Id,
                i => i.WithAbsoluteTopLeft(newAbsoluteTopLeft),
                DiagramNodeMember.AbsolutePosition);
        }

        private void UpdateChildrenAbsolutePosition([NotNull] IDiagramNode updatedNode)
        {
            foreach (var childNode in GetChildNodes(updatedNode.Id).ToList())
                UpdateAbsolutePosition(childNode);
        }

        private void UpdateParentNodeChildrenAreaSize([NotNull] IDiagramNode updatedNode)
        {
            if (!updatedNode.HasParent)
                return;

            var parentNode = GetDiagramNode(updatedNode.ParentNodeId.Value);

            var newChildrenAreaSize = CalculateChildrenAreaSize(parentNode.Id);

            if (newChildrenAreaSize.IsUndefined && parentNode.ChildrenAreaSize.IsUndefined ||
                newChildrenAreaSize == parentNode.ChildrenAreaSize)
                return;

            UpdateNodeCore(
                parentNode.Id,
                i => i.WithChildrenAreaSize(newChildrenAreaSize),
                DiagramNodeMember.ChildrenAreaSize);
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            if (!DiagramNodeExists(nodeId))
                return;

            var oldNode = GetDiagramNode(nodeId);

            RemoveAllConnectorsOfNode(oldNode);
            RemoveChildNodes(oldNode);
            _nodes.Remove(nodeId);

            _shapeEvents.Add(new DiagramNodeRemovedEvent(oldNode));

            UpdateParentNodeChildrenAreaSize(oldNode);
        }

        private void RemoveChildNodes([NotNull] IDiagramNode node)
        {
            foreach (var childNode in GetChildNodes(node.Id).ToList())
                RemoveNode(childNode.Id);
        }

        private void RemoveAllConnectorsOfNode([NotNull] IDiagramNode node)
        {
            foreach (var connector in GetConnectorsByNode(node.Id).ToList())
                RemoveConnector(connector.Id);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId nodeId)
        {
            return _connectors.Values
                .Where(i => i.Source == nodeId || i.Target == nodeId);
        }

        public void AddConnector(ModelRelationshipId relationshipId)
        {
            if (DiagramConnectorExists(relationshipId))
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
            if (!DiagramConnectorExists(relationshipId))
                return;

            var oldConnector = GetDiagramConnector(relationshipId);
            var newConnector = oldConnector.WithRoute(newRoute);

            _connectors[relationshipId] = newConnector;

            _shapeEvents.Add(new DiagramConnectorRouteChangedEvent(oldConnector, newConnector));
        }

        public void RemoveConnector(ModelRelationshipId relationshipId)
        {
            if (!DiagramConnectorExists(relationshipId))
                return;

            var oldConnector = GetDiagramConnector(relationshipId);

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

        public void ApplyLayout(LayoutInfo layoutInfo)
        {
            foreach (var vertexRecKeyValue in layoutInfo.VertexRects)
            {
                var newRelativeTopLeft = vertexRecKeyValue.Value.TopLeft;
                var modelNodeId = vertexRecKeyValue.Key;

                UpdateNodeRelativeTopLeft(modelNodeId, newRelativeTopLeft);
            }

            foreach (var edgeRouteKeyValue in layoutInfo.EdgeRoutes)
                UpdateConnectorRoute(edgeRouteKeyValue.Key, edgeRouteKeyValue.Value);
        }

        private Point2D CalculateAbsolutePosition(ModelNodeId nodeId)
        {
            if (!DiagramNodeExists(nodeId))
                throw new Exception($"Diagram node: {nodeId} not found.");

            var diagramNode = GetDiagramNode(nodeId);

            return diagramNode.ParentNodeId.Match(
                some =>
                {
                    var parentNode = GetDiagramNode(some);
                    return parentNode.AbsoluteTopLeft + parentNode.ChildrenAreaTopLeft + ChildrenAreaPaddingVector + diagramNode.RelativeTopLeft;
                },
                () => diagramNode.RelativeTopLeft);
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
            var childrenAreaSize = GetChildNodes(parentNodeId)
                .Select(i => i.RelativeRect).Where(i => i.IsDefined()).Union()
                .WithMargin(_childrenAreaPadding).Size;

            return childrenAreaSize.IsDefined ? childrenAreaSize : Size2D.Zero;
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<IDiagramNode> GetChildNodes(ModelNodeId diagramNodeId)
        {
            return _nodes.Values.Where(i => i.ParentNodeId.ToNullable() == diagramNodeId);
        }

        private ModelNodeId? GetParentNodeId(ModelNodeId modelNodeId)
        {
            var containerNodes = _model
                .GetRelatedNodes(modelNodeId, CommonDirectedModelRelationshipTypes.Container, recursive: false)
                .ToList();

            if (!containerNodes.Any())
                return null;

            if (containerNodes.Count > 1)
                throw new Exception($"{modelNodeId} has more than 1 containers.");

            return containerNodes.First().Id;
        }

        [NotNull]
        private List<ModelNodeId> GetChildDiagramNodeIds(ModelNodeId modelNodeId)
        {
            return _model
                .GetRelatedNodes(modelNodeId, CommonDirectedModelRelationshipTypes.Contained, recursive: false)
                .Select(i => i.Id)
                .Where(DiagramNodeExists)
                .ToList();
        }

        private bool DiagramNodeExists(ModelNodeId modelNodeId) => _nodes.ContainsKey(modelNodeId);

        [NotNull]
        private IDiagramNode GetDiagramNode(ModelNodeId modelNodeId) => _nodes[modelNodeId];

        private bool DiagramConnectorExists(ModelRelationshipId modelRelationshipId) => _connectors.ContainsKey(modelRelationshipId);

        [NotNull]
        private IDiagramConnector GetDiagramConnector(ModelRelationshipId modelRelationshipId) => _connectors[modelRelationshipId];
    }
}