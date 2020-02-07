using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
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
    /// An immutable diagram.
    /// </summary>
    public sealed class ImmutableDiagram : IDiagram
    {
        public IModel Model { get; }
        [NotNull] private readonly IImmutableDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _connectors;

        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        public Rect2D Rect { get; }
        public bool IsEmpty { get; }
        [NotNull] private readonly IDiagramGraph _allShapesGraph;

        public ImmutableDiagram(
            [NotNull] IModel model,
            [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors)
        {
            Model = model;
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
        {
            return _nodes.TryGetValue(modelNodeId, out var node) ? Maybe.Create(node) : Maybe<IDiagramNode>.Nothing;
        }

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

        //public DiagramEvent ApplyLayout(GroupLayoutInfo diagramLayout)
        //{
        //    var events = new List<DiagramShapeEventBase>();
        //    var updatedNodes = new Dictionary<ModelNodeId, IDiagramNode>();
        //    var updatedConnectors = new Dictionary<ModelRelationshipId, IDiagramConnector>();

        //    ApplyLayoutRecursive(diagramLayout, events, updatedNodes, updatedConnectors);

        //    var newDiagram = CreateInstance(Model, _nodes.SetItems(updatedNodes), _connectors.SetItems(updatedConnectors));
        //    return DiagramEvent.Create(newDiagram, events);
        //}

        //private void ApplyLayoutRecursive(
        //    [NotNull] GroupLayoutInfo groupLayoutInfo,
        //    [NotNull] [ItemNotNull] ICollection<DiagramShapeEventBase> events,
        //    [NotNull] IDictionary<ModelNodeId, IDiagramNode> updatedNodes,
        //    [NotNull] IDictionary<ModelRelationshipId, IDiagramConnector> updatedConnectors)
        //{
        //    ApplyNodeLayout(groupLayoutInfo.Boxes, events, updatedNodes, updatedConnectors);
        //    ApplyConnectorLayout(groupLayoutInfo.Lines, events, updatedConnectors);
        //}

        //private void ApplyNodeLayout(
        //    [NotNull] [ItemNotNull] IEnumerable<BoxLayoutInfo> boxLayoutList,
        //    [NotNull] [ItemNotNull] ICollection<DiagramShapeEventBase> events,
        //    [NotNull] IDictionary<ModelNodeId, IDiagramNode> updatedNodes,
        //    [NotNull] IDictionary<ModelRelationshipId, IDiagramConnector> updatedConnectors)
        //{
        //    foreach (var boxLayoutInfo in boxLayoutList)
        //    {
        //        var modelNodeId = ModelNodeId.Parse(boxLayoutInfo.ShapeId);
        //        var maybeCurrentNode = TryGetNode(modelNodeId);
        //        if (!maybeCurrentNode.HasValue)
        //            continue;

        //        var oldNode = maybeCurrentNode.Value;

        //        if (boxLayoutInfo.ChildGroup != null)
        //            ApplyLayoutRecursive(boxLayoutInfo.ChildGroup, events, updatedNodes, updatedConnectors);

        //        var newNode = oldNode.WithTopLeft(boxLayoutInfo.TopLeft).WithChildrenAreaSize(boxLayoutInfo.ChildrenAreaSize);
        //        updatedNodes.Add(oldNode.Id, newNode);

        //        if (oldNode.TopLeft != newNode.TopLeft)
        //            events.Add(new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.Position));

        //        if (oldNode.ChildrenAreaSize != newNode.ChildrenAreaSize)
        //            events.Add(new DiagramNodeChangedEvent(oldNode, newNode, DiagramNodeMember.ChildrenAreaSize));
        //    }
        //}

        //private void ApplyConnectorLayout(
        //    [NotNull] IEnumerable<LineLayoutInfo> lineLayoutList,
        //    [NotNull] [ItemNotNull] ICollection<DiagramShapeEventBase> events,
        //    [NotNull] IDictionary<ModelRelationshipId, IDiagramConnector> updatedConnectors)
        //{
        //    foreach (var lineLayoutInfo in lineLayoutList)
        //    {
        //        var modelRelationshipIdNodeId = ModelRelationshipId.Parse(lineLayoutInfo.ShapeId);
        //        var maybeCurrentConnector = TryGetConnector(modelRelationshipIdNodeId);
        //        if (!maybeCurrentConnector.HasValue)
        //            continue;

        //        var oldConnector = maybeCurrentConnector.Value;

        //        var newConnector = oldConnector.WithRoute(lineLayoutInfo.Route);
        //        updatedConnectors.Add(oldConnector.Id, newConnector);

        //        if (oldConnector.Route != newConnector.Route)
        //            events.Add(new DiagramConnectorRouteChangedEvent(oldConnector, newConnector));
        //    }
        //}

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            return modelNodeIds
                .Select(i => TryGetNode(i).Match(j => j.Rect, () => Rect2D.Undefined))
                .Union();
        }

        private Rect2D CalculateRect() => Nodes.Select(i => i.Rect).Concat(Connectors.Select(i => i.Rect)).Union();

        [NotNull]
        public static IDiagram Create([NotNull] IModel model)
        {
            return new ImmutableDiagram(
                model,
                ImmutableDictionary<ModelNodeId, IDiagramNode>.Empty,
                ImmutableDictionary<ModelRelationshipId, IDiagramConnector>.Empty);
        }
    }
}