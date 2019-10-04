using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    using IDiagramGraph = IImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId>;
    using DiagramGraph = ImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId>;

    /// <summary>
    /// An immutable implementation of a diagram.
    /// </summary>
    public sealed class Diagram : IDiagram
    {
        public IModel Model { get; }
        [NotNull] private readonly IConnectorTypeResolver _connectorTypeResolver;
        [NotNull] private readonly IImmutableDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _connectors;

        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        public Rect2D Rect { get; }
        public bool IsEmpty { get; }
        [NotNull] private readonly IDiagramGraph _allShapesGraph;

        private Diagram(
            [NotNull] IModel model,
            [NotNull] IConnectorTypeResolver connectorTypeResolver,
            [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors)
        {
            Model = model;
            _connectorTypeResolver = connectorTypeResolver;
            _nodes = nodes;
            _connectors = connectors;
            Nodes = nodes.Values.ToImmutableHashSet();
            Connectors = connectors.Values.ToImmutableHashSet();
            Rect = CalculateRect();
            IsEmpty = !_nodes.Any() && !_connectors.Any();
            _allShapesGraph = DiagramGraph.Create(Nodes, Connectors);
        }

        public bool NodeExists(ModelNodeId modelNodeId) => Nodes.Any(i => i.Id == modelNodeId);
        public bool ConnectorExists(ModelRelationshipId modelRelationshipId) => Connectors.Any(i => i.Id == modelRelationshipId);

        public bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId)
            => NodeExists(sourceModelNodeId) && NodeExists(targetModelNodeId) && _allShapesGraph.PathExists(sourceModelNodeId, targetModelNodeId);

        public bool PathExists(Maybe<ModelNodeId> maybeSourceModelNodeId, Maybe<ModelNodeId> maybeTargetModelNodeId)
        {
            return maybeSourceModelNodeId.Match(
                sourceNodeId => maybeTargetModelNodeId.Match(
                    targetNodeId => PathExists(sourceNodeId, targetNodeId),
                    () => false),
                () => false);
        }

        public bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId) => _allShapesGraph.IsEdgeRedundant(modelRelationshipId);

        public IDiagramNode GetNode(ModelNodeId modelNodeId) => _nodes[modelNodeId];

        public Maybe<IDiagramNode> TryGetNode(ModelNodeId modelNodeId)
            => _nodes.TryGetValue(modelNodeId, out var node) ? Maybe.Create(node) : Maybe<IDiagramNode>.Nothing;

        public IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId) => _connectors[modelRelationshipId];

        public Maybe<IDiagramConnector> TryGetConnector(ModelRelationshipId modelRelationshipId)
            => _connectors.TryGetValue(modelRelationshipId, out var connector) ? Maybe.Create(connector) : Maybe<IDiagramConnector>.Nothing;

        public IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id) => Connectors.Where(i => i.Source == id || i.Target == id);

        //public IEnumerable<IDiagramNode> GetAdjacentNodes(ModelNodeId id, DirectedModelRelationshipType? directedModelRelationshipType = null)
        //{
        //    IEnumerable<IDiagramNode> result;

        //    if (directedModelRelationshipType != null)
        //    {
        //        result = _allShapesGraph.GetAdjacentVertices(
        //            id,
        //            directedModelRelationshipType.Value.Direction,
        //            e => e.ModelRelationship.Stereotype == directedModelRelationshipType.Value.Stereotype);
        //    }
        //    else
        //    {
        //        result = _allShapesGraph.GetAdjacentVertices(id, EdgeDirection.In)
        //            .Union(_allShapesGraph.GetAdjacentVertices(id, EdgeDirection.Out));
        //    }

        //    return result;
        //}

        public DiagramChangedEvent AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            var maybeDiagramNode = TryGetNode(nodeId);
            if (maybeDiagramNode.HasValue)
                return DiagramChangedEvent.None(this);

            var maybeModelNode = Model.TryGetNode(nodeId);
            if (!maybeModelNode.HasValue)
                throw new Exception($"Node {nodeId} not found in model.");

            var newNode = CreateNode(maybeModelNode.Value).WithParentNodeId(parentNodeId.ToMaybe());
            var newDiagram = CreateInstance(Model, _nodes.Add(newNode.Id, newNode), _connectors);
            return DiagramChangedEvent.Create(newDiagram, new DiagramNodeAddedEvent(newNode));
        }

        public DiagramChangedEvent UpdateNodePayloadAreaSize(ModelNodeId nodeId, Size2D newSize)
        {
            return UpdateNode(
                nodeId,
                i => i.WithPayloadAreaSize(newSize),
                (oldNode, newNode) => new DiagramNodeRectChangedEvent(oldNode, newNode));
        }

        public DiagramChangedEvent UpdateNodeChildrenAreaSize(ModelNodeId nodeId, Size2D newSize)
        {
            return UpdateNode(
                nodeId,
                i => i.WithChildrenAreaSize(newSize),
                (oldNode, newNode) => new DiagramNodeRectChangedEvent(oldNode, newNode));
        }

        public DiagramChangedEvent UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter)
        {
            return UpdateNode(
                nodeId,
                i => i.WithCenter(newCenter),
                (oldNode, newNode) => new DiagramNodeRectChangedEvent(oldNode, newNode));
        }

        public DiagramChangedEvent UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            return UpdateNode(
                nodeId,
                i => i.WithTopLeft(newTopLeft),
                (oldNode, newNode) => new DiagramNodeRectChangedEvent(oldNode, newNode));
        }

        public DiagramChangedEvent RemoveNode(ModelNodeId nodeId)
        {
            if (!NodeExists(nodeId))
                return DiagramChangedEvent.None(this);

            var oldNode = GetNode(nodeId);
            var newDiagram = CreateInstance(Model, _nodes.Remove(nodeId), _connectors);
            return DiagramChangedEvent.Create(newDiagram, new DiagramNodeRemovedEvent(oldNode));
        }

        public DiagramChangedEvent AddConnector(ModelRelationshipId relationshipId)
        {
            var maybeConnector = TryGetConnector(relationshipId);
            if (maybeConnector.HasValue)
                return DiagramChangedEvent.None(this);

            var maybeRelationship = Model.TryGetRelationship(relationshipId);
            if (!maybeRelationship.HasValue)
                throw new InvalidOperationException($"Relationship {relationshipId} does not exist.");

            var newConnector = CreateConnector(maybeRelationship.Value);
            var newDiagram = CreateInstance(Model, _nodes, _connectors.Add(newConnector.Id, newConnector));
            return DiagramChangedEvent.Create(newDiagram, new DiagramConnectorAddedEvent(newConnector));
        }

        public DiagramChangedEvent UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute)
        {
            var maybeDiagramConnector = TryGetConnector(relationshipId);
            if (!maybeDiagramConnector.HasValue)
                throw new InvalidOperationException($"Connector {relationshipId} does not exist.");

            var oldConnector = maybeDiagramConnector.Value;
            var newConnector = oldConnector.WithRoute(newRoute);
            var newDiagram = CreateInstance(Model, _nodes, _connectors.SetItem(relationshipId, newConnector));
            return DiagramChangedEvent.Create(newDiagram, new DiagramConnectorRouteChangedEvent(oldConnector, newConnector));
        }

        public DiagramChangedEvent RemoveConnector(ModelRelationshipId relationshipId)
        {
            if (!ConnectorExists(relationshipId))
                return DiagramChangedEvent.None(this);

            var oldConnector = GetConnector(relationshipId);
            var newDiagram = CreateInstance(Model, _nodes, _connectors.Remove(relationshipId));
            return DiagramChangedEvent.Create(newDiagram, new DiagramConnectorRemovedEvent(oldConnector));
        }

        public DiagramChangedEvent UpdateModel(IModel newModel)
        {
            // TODO: remove all shapes whose model ID does not exist in the new model.
            var newDiagram = CreateInstance(newModel, _nodes, _connectors);

            return DiagramChangedEvent.Create(newDiagram, new DiagramModelUpdatedEvent());
        }

        public DiagramChangedEvent UpdateModelNode(IModelNode updatedModelNode)
        {
            return UpdateNode(
                updatedModelNode.Id,
                i => i.WithModelNode(updatedModelNode),
                (oldNode, newNode) => new DiagramNodeModelNodeChangedEvent(oldNode, newNode));
        }

        public DiagramChangedEvent ApplyLayout(DiagramLayoutInfo diagramLayout)
        {
            var events = new List<DiagramComponentChangedEventBase>();
            var updatedNodes = new Dictionary<ModelNodeId, IDiagramNode>();

            foreach (var nodeLayout in diagramLayout.RootNodes)
            {
                var maybeCurrentNode = TryGetNode(nodeLayout.Node.Id);
                if (maybeCurrentNode.HasValue)
                {
                    var oldNode = maybeCurrentNode.Value;
                    var newNode = oldNode.WithTopLeft(nodeLayout.Rect.TopLeft);
                    updatedNodes.Add(oldNode.Id, newNode);
                    events.Add(new DiagramNodeRectChangedEvent(oldNode, newNode));
                }

                // TODO: update connector routes too
            }

            var newDiagram = CreateInstance(Model, _nodes.SetItems(updatedNodes), _connectors);
            return DiagramChangedEvent.Create(newDiagram, events);
        }

        public DiagramChangedEvent Clear()
        {
            var newDiagram = Create(Model, _connectorTypeResolver);
            // Shall we raise node and connector removed events ?
            return DiagramChangedEvent.Create(newDiagram);
        }

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            return modelNodeIds
                .Select(i => TryGetNode(i).Match(j => j.Rect, () => Rect2D.Undefined))
                .Union();
        }

        private DiagramChangedEvent UpdateNode(
            ModelNodeId nodeId,
            [NotNull] Func<IDiagramNode, IDiagramNode> nodeMutatorFunc,
            [NotNull] Func<IDiagramNode, IDiagramNode, DiagramNodeChangedEventBase> nodeChangedEventFunc)
        {
            var maybeDiagramNode = TryGetNode(nodeId);
            if (!maybeDiagramNode.HasValue)
                throw new InvalidOperationException($"Trying to update {nodeId} but it does not exist.");

            var oldNode = maybeDiagramNode.Value;
            var newNode = nodeMutatorFunc(oldNode);
            var newDiagram = CreateInstance(Model, _nodes.SetItem(newNode.Id, newNode), _connectors);
            var diagramNodeChangedEvent = nodeChangedEventFunc(oldNode, newNode);

            return DiagramChangedEvent.Create(newDiagram, diagramNodeChangedEvent);
        }

        private Rect2D CalculateRect() => Nodes.Select(i => i.Rect).Concat(Connectors.Select(i => i.Rect)).Union();

        [NotNull]
        private static IDiagramNode CreateNode([NotNull] IModelNode modelNode) => new DiagramNode(modelNode);

        [NotNull]
        private IDiagramConnector CreateConnector([NotNull] IModelRelationship relationship)
        {
            var connectorType = _connectorTypeResolver.GetConnectorType(relationship.Stereotype);
            return new DiagramConnector(relationship, connectorType);
        }

        [NotNull]
        private IDiagram CreateInstance(
            [NotNull] IModel model,
            [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors)
        {
            return new Diagram(model, _connectorTypeResolver, nodes, connectors);
        }

        [NotNull]
        public static IDiagram Create([NotNull] IModel model, [NotNull] IConnectorTypeResolver connectorTypeResolver)
        {
            return new Diagram(
                model,
                connectorTypeResolver,
                ImmutableDictionary<ModelNodeId, IDiagramNode>.Empty,
                ImmutableDictionary<ModelRelationshipId, IDiagramConnector>.Empty);
        }
    }
}