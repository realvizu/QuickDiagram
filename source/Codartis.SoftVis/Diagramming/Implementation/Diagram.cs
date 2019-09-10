using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        [NotNull] private readonly IImmutableDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _connectors;
        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        [NotNull] private readonly IDiagramGraph _allShapesGraph;

        public ILayoutGroup RootLayoutGroup { get; }
        [NotNull] private readonly IDictionary<ModelNodeId, ILayoutGroup> _nodeLayoutGroups;
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _crossLayoutGroupConnectors;
        public IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors { get; }

        private Diagram(
            [NotNull] IModel model,
            [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors)
        {
            Model = model;
            _nodes = nodes;
            _connectors = connectors;

            Nodes = nodes.Values.ToImmutableHashSet();
            Connectors = connectors.Values.ToImmutableHashSet();
            _allShapesGraph = DiagramGraph.Create(Nodes, Connectors);

            RootLayoutGroup = CreateLayoutGroup(Maybe<ModelNodeId>.Nothing);
            _nodeLayoutGroups = CreateLayoutGroupForAllNodes();
            _crossLayoutGroupConnectors = GetCrossLayoutGroupConnectors();
            CrossLayoutGroupConnectors = _crossLayoutGroupConnectors.Values.ToImmutableHashSet();
        }

        private ImmutableDictionary<ModelNodeId, ILayoutGroup> CreateLayoutGroupForAllNodes()
        {
            return Nodes
                .Select(i => (i.Id, CreateLayoutGroup(i.Id.ToMaybe())))
                .ToImmutableDictionary(i => i.Id, i => i.Item2);
        }

        private IImmutableDictionary<ModelRelationshipId, IDiagramConnector> GetCrossLayoutGroupConnectors()
        {
            return Connectors
                .Where(i => GetNode(i.Source).ParentNodeId != GetNode(i.Target).ParentNodeId)
                .ToImmutableDictionary(i => i.Id);
        }

        [NotNull]
        private ILayoutGroup CreateLayoutGroup(Maybe<ModelNodeId> containerNodeId)
        {
            var nodesInLayoutGroup = Nodes
                .Where(i => i.ParentNodeId.Equals(containerNodeId))
                .ToImmutableHashSet();

            var connectorsInLayoutGroup = Connectors
                .Where(i => GetNode(i.Source).ParentNodeId.Equals(containerNodeId) && GetNode(i.Target).ParentNodeId.Equals(containerNodeId))
                .ToImmutableHashSet();

            return LayoutGroup.Create(nodesInLayoutGroup, connectorsInLayoutGroup);
        }

        public Maybe<ILayoutGroup> GetLayoutGroupByNodeId(ModelNodeId modelNodeId)
        {
            return _nodeLayoutGroups[modelNodeId].ToMaybe(); //(i => i != LayoutGroup.Empty);
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

        public IDiagram WithModel(IModel newModel)
        {
            // TODO: remove all shapes whose model ID does not exist in the new model.
            return CreateInstance(newModel, _nodes, _connectors);
        }

        public IDiagram AddNode(IDiagramNode newNode)
        {
            return CreateInstance(Model, _nodes.Add(newNode.Id, newNode), _connectors);
        }

        public IDiagram UpdateNode(IDiagramNode updatedNode)
        {
            return CreateInstance(Model, _nodes.SetItem(updatedNode.Id, updatedNode), _connectors);
        }

        public IDiagram RemoveNode(ModelNodeId nodeId)
        {
            return CreateInstance(Model, _nodes.Remove(nodeId), _connectors);
        }

        public IDiagram AddConnector(IDiagramConnector newConnector)
        {
            return CreateInstance(Model, _nodes, _connectors.Add(newConnector.Id, newConnector));
        }

        public IDiagram UpdateConnector(IDiagramConnector updatedConnector)
        {
            return CreateInstance(Model, _nodes, _connectors.SetItem(updatedConnector.Id, updatedConnector));
        }

        public IDiagram RemoveConnector(ModelRelationshipId connectorId)
        {
            return CreateInstance(Model, _nodes, _connectors.Remove(connectorId));
        }

        public IDiagram Clear() => Create(Model);

        public bool IsCrossingLayoutGroups(ModelRelationshipId modelRelationshipId)
        {
            return _crossLayoutGroupConnectors.ContainsKey(modelRelationshipId);
        }

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            return modelNodeIds
                .Select(i => TryGetNode(i).Match(j => j.Rect, () => Rect2D.Undefined))
                .Union();
        }

        [NotNull]
        private static IDiagram CreateInstance(
            [NotNull] IModel model,
            [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors)
        {
            return new Diagram(model, nodes, connectors);
        }

        [NotNull]
        public static IDiagram Create([NotNull] IModel model)
        {
            return CreateInstance(
                model,
                ImmutableDictionary<ModelNodeId, IDiagramNode>.Empty,
                ImmutableDictionary<ModelRelationshipId, IDiagramConnector>.Empty);
        }
    }
}