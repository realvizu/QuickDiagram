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

        public IEnumerable<IDiagramNode> GetChildNodes(ModelNodeId diagramNodeId) => Nodes.Where(i => i.ParentNodeId.ToNullable() == diagramNodeId);

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

        public DiagramEvent AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            var maybeDiagramNode = TryGetNode(nodeId);
            if (maybeDiagramNode.HasValue)
                return DiagramEvent.None(this);

            var maybeModelNode = Model.TryGetNode(nodeId);
            if (!maybeModelNode.HasValue)
                throw new Exception($"Node {nodeId} not found in model.");

            var newNode = CreateNode(maybeModelNode.Value).WithParentNodeId(parentNodeId);
            var newDiagram = CreateInstance(Model, _nodes.Add(newNode.Id, newNode), _connectors);
            return DiagramEvent.Create(newDiagram, new DiagramNodeAddedEvent(newNode));
        }

        public DiagramEvent UpdateNodePayloadAreaSize(ModelNodeId nodeId, Size2D newSize)
        {
            return UpdateNode(
                nodeId,
                i => i.WithPayloadAreaSize(newSize),
                (oldNode, newNode) => new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.PayloadAreaSize));
        }

        public DiagramEvent UpdateNodeChildrenAreaSize(ModelNodeId nodeId, Size2D newSize)
        {
            return UpdateNode(
                nodeId,
                i => i.WithChildrenAreaSize(newSize),
                (oldNode, newNode) => new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.ChildrenAreaSize));
        }

        public DiagramEvent UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter)
        {
            return UpdateNode(
                nodeId,
                i => i.WithCenter(newCenter),
                (oldNode, newNode) => new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.Position));
        }

        public DiagramEvent UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            return UpdateNode(
                nodeId,
                i => i.WithTopLeft(newTopLeft),
                (oldNode, newNode) => new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.Position));
        }

        public DiagramEvent RemoveNode(ModelNodeId nodeId)
        {
            return CreateDiagramEvent(RemoveNodeCore(nodeId, CreateDiagramOperationResult()));
        }

        public DiagramEvent AddConnector(ModelRelationshipId relationshipId)
        {
            var maybeConnector = TryGetConnector(relationshipId);
            if (maybeConnector.HasValue)
                return DiagramEvent.None(this);

            var maybeRelationship = Model.TryGetRelationship(relationshipId);
            if (!maybeRelationship.HasValue)
                throw new InvalidOperationException($"Relationship {relationshipId} does not exist.");

            var newConnector = CreateConnector(maybeRelationship.Value);
            var newDiagram = CreateInstance(Model, _nodes, _connectors.Add(newConnector.Id, newConnector));
            return DiagramEvent.Create(newDiagram, new DiagramConnectorAddedEvent(newConnector));
        }

        public DiagramEvent UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute)
        {
            var maybeDiagramConnector = TryGetConnector(relationshipId);
            if (!maybeDiagramConnector.HasValue)
                throw new InvalidOperationException($"Connector {relationshipId} does not exist.");

            var oldConnector = maybeDiagramConnector.Value;
            var newConnector = oldConnector.WithRoute(newRoute);
            var newDiagram = CreateInstance(Model, _nodes, _connectors.SetItem(relationshipId, newConnector));
            return DiagramEvent.Create(newDiagram, new DiagramConnectorRouteChangedEvent(oldConnector, newConnector));
        }

        public DiagramEvent RemoveConnector(ModelRelationshipId relationshipId)
        {
            return CreateDiagramEvent(RemoveConnectorCore(relationshipId, CreateDiagramOperationResult()));
        }

        public DiagramEvent UpdateModel(IModel newModel)
        {
            // TODO: remove all shapes whose model ID does not exist in the new model.
            var newDiagram = CreateInstance(newModel, _nodes, _connectors);

            return DiagramEvent.Create(newDiagram);
        }

        public DiagramEvent UpdateModelNode(IModelNode updatedModelNode)
        {
            return UpdateNode(
                updatedModelNode.Id,
                i => i.WithModelNode(updatedModelNode),
                (oldNode, newNode) => new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.ModelNode));
        }

        public DiagramEvent ApplyLayout(GroupLayoutInfo diagramLayout)
        {
            var events = new List<DiagramShapeEventBase>();
            var updatedNodes = new Dictionary<ModelNodeId, IDiagramNode>();
            var updatedConnectors = new Dictionary<ModelRelationshipId, IDiagramConnector>();

            ApplyLayoutRecursive(diagramLayout, events, updatedNodes, updatedConnectors);

            var newDiagram = CreateInstance(Model, _nodes.SetItems(updatedNodes), _connectors.SetItems(updatedConnectors));
            return DiagramEvent.Create(newDiagram, events);
        }

        private void ApplyLayoutRecursive(
            [NotNull] GroupLayoutInfo groupLayoutInfo,
            [NotNull] [ItemNotNull] ICollection<DiagramShapeEventBase> events,
            [NotNull] IDictionary<ModelNodeId, IDiagramNode> updatedNodes,
            [NotNull] IDictionary<ModelRelationshipId, IDiagramConnector> updatedConnectors)
        {
            ApplyNodeLayout(groupLayoutInfo.Boxes, events, updatedNodes, updatedConnectors);
            ApplyConnectorLayout(groupLayoutInfo.Lines, events, updatedConnectors);
        }

        private void ApplyNodeLayout(
            [NotNull] [ItemNotNull] IEnumerable<BoxLayoutInfo> boxLayoutList,
            [NotNull] [ItemNotNull] ICollection<DiagramShapeEventBase> events,
            [NotNull] IDictionary<ModelNodeId, IDiagramNode> updatedNodes,
            [NotNull] IDictionary<ModelRelationshipId, IDiagramConnector> updatedConnectors)
        {
            foreach (var boxLayoutInfo in boxLayoutList)
            {
                var modelNodeId = ModelNodeId.Parse(boxLayoutInfo.ShapeId);
                var maybeCurrentNode = TryGetNode(modelNodeId);
                if (!maybeCurrentNode.HasValue)
                    continue;

                var oldNode = maybeCurrentNode.Value;

                if (boxLayoutInfo.ChildGroup != null)
                    ApplyLayoutRecursive(boxLayoutInfo.ChildGroup, events, updatedNodes, updatedConnectors);

                var newNode = oldNode.WithTopLeft(boxLayoutInfo.TopLeft).WithChildrenAreaSize(boxLayoutInfo.ChildrenAreaSize);
                updatedNodes.Add(oldNode.Id, newNode);

                if (oldNode.TopLeft != newNode.TopLeft)
                    events.Add(new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.Position));

                if (oldNode.ChildrenAreaSize != newNode.ChildrenAreaSize)
                    events.Add(new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.ChildrenAreaSize));
            }
        }

        private void ApplyConnectorLayout(
            [NotNull] IEnumerable<LineLayoutInfo> lineLayoutList,
            [NotNull] [ItemNotNull] ICollection<DiagramShapeEventBase> events,
            [NotNull] IDictionary<ModelRelationshipId, IDiagramConnector> updatedConnectors)
        {
            foreach (var lineLayoutInfo in lineLayoutList)
            {
                var modelRelationshipIdNodeId = ModelRelationshipId.Parse(lineLayoutInfo.ShapeId);
                var maybeCurrentConnector = TryGetConnector(modelRelationshipIdNodeId);
                if (!maybeCurrentConnector.HasValue)
                    continue;

                var oldConnector = maybeCurrentConnector.Value;

                var newConnector = oldConnector.WithRoute(lineLayoutInfo.Route);
                updatedConnectors.Add(oldConnector.Id, newConnector);

                if (oldConnector.Route != newConnector.Route)
                    events.Add(new DiagramConnectorRouteChangedEvent(oldConnector, newConnector));
            }
        }

        public DiagramEvent Clear()
        {
            var newDiagram = Create(Model, _connectorTypeResolver);
            // Shall we raise node and connector removed events ?
            return DiagramEvent.Create(newDiagram);
        }

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            return modelNodeIds
                .Select(i => TryGetNode(i).Match(j => j.Rect, () => Rect2D.Undefined))
                .Union();
        }

        private DiagramEvent UpdateNode(
            ModelNodeId nodeId,
            [NotNull] Func<IDiagramNode, IDiagramNode> nodeMutatorFunc,
            [NotNull] Func<IDiagramNode, IDiagramNode, DiagramNodeChangedEvent> nodeChangedEventFunc)
        {
            var maybeDiagramNode = TryGetNode(nodeId);
            if (!maybeDiagramNode.HasValue)
                throw new InvalidOperationException($"Trying to update {nodeId} but it does not exist.");

            var oldNode = maybeDiagramNode.Value;
            var newNode = nodeMutatorFunc(oldNode);
            var newDiagram = CreateInstance(Model, _nodes.SetItem(newNode.Id, newNode), _connectors);
            var diagramNodeChangedEvent = nodeChangedEventFunc(oldNode, newNode);

            return DiagramEvent.Create(newDiagram, diagramNodeChangedEvent);
        }

        private DiagramOperationResult RemoveNodeCore(ModelNodeId nodeId, DiagramOperationResult diagramOperationResult)
        {
            return TryGetNode(nodeId)
                .Match(
                    some => RemoveChildNodes(some, RemoveAllConnectorsOfNode(some, diagramOperationResult))
                        .Remove(some)
                        .Add(new DiagramNodeRemovedEvent(some)),
                    () => diagramOperationResult
                );
        }

        private DiagramOperationResult RemoveChildNodes([NotNull] IDiagramNode node, DiagramOperationResult diagramOperationResult)
        {
            return GetChildNodes(node.Id)
                .Aggregate(diagramOperationResult, (current, childNode) => RemoveNodeCore(childNode.Id, current));
        }

        private DiagramOperationResult RemoveAllConnectorsOfNode([NotNull] IDiagramNode node, DiagramOperationResult diagramOperationResult)
        {
            return _allShapesGraph.GetAllEdges(node.Id)
                .Aggregate(diagramOperationResult, (current, connector) => RemoveConnectorCore(connector.Id, current));
        }

        private DiagramOperationResult RemoveConnectorCore(ModelRelationshipId relationshipId, DiagramOperationResult diagramOperationResult)
        {
            return TryGetConnector(relationshipId)
                .Match(
                    some => diagramOperationResult
                        .Remove(some)
                        .Add(new DiagramConnectorRemovedEvent(some)),
                    () => diagramOperationResult
                );
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

        private DiagramOperationResult CreateDiagramOperationResult() => new DiagramOperationResult(_nodes, _connectors);

        private DiagramEvent CreateDiagramEvent(DiagramOperationResult diagramOperationResult)
        {
            if (!diagramOperationResult.Events.Any())
                return DiagramEvent.None(this);

            var newDiagram = CreateInstance(Model, diagramOperationResult.Nodes, diagramOperationResult.Connectors);
            return new DiagramEvent(newDiagram, diagramOperationResult.Events);
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

        private struct DiagramOperationResult
        {
            [NotNull] public IImmutableDictionary<ModelNodeId, IDiagramNode> Nodes { get; }
            [NotNull] public IImmutableDictionary<ModelRelationshipId, IDiagramConnector> Connectors { get; }
            [NotNull] [ItemNotNull] public IImmutableList<DiagramShapeEventBase> Events { get; }

            public DiagramOperationResult(
                [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
                [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors)
                : this(nodes, connectors, ImmutableList.Create<DiagramShapeEventBase>())
            {
            }

            private DiagramOperationResult(
                [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
                [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors,
                [NotNull] [ItemNotNull] IImmutableList<DiagramShapeEventBase> events)
            {
                Nodes = nodes;
                Connectors = connectors;
                Events = events;
            }

            public DiagramOperationResult Remove([NotNull] IDiagramNode node)
            {
                return new DiagramOperationResult(Nodes.Remove(node.Id), Connectors, Events);
            }

            public DiagramOperationResult Remove([NotNull] IDiagramConnector connector)
            {
                return new DiagramOperationResult(Nodes, Connectors.Remove(connector.Id), Events);
            }

            public DiagramOperationResult Add(DiagramShapeEventBase @event)
            {
                return new DiagramOperationResult(Nodes, Connectors, Events.Add(@event));
            }
        }
    }
}