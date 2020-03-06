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
        [NotNull] private readonly IModelRelationshipFeatureProvider _modelRelationshipFeatureProvider;

        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        public Rect2D Rect { get; }
        public bool IsEmpty { get; }

        /// <remarks>
        /// We need a separate graph for each group of relationships that are transitive.
        /// </remarks>
        [NotNull]
        private readonly IDictionary<string, IDiagramGraph> _diagramGraphsByRelationshipTransitivityGroup;

        public ImmutableDiagram(
            [NotNull] IModel model,
            [NotNull] IImmutableDictionary<ModelNodeId, IDiagramNode> nodes,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> connectors,
            [NotNull] IModelRelationshipFeatureProvider modelRelationshipFeatureProvider)
        {
            Model = model;
            _nodes = nodes;
            _connectors = connectors;
            _modelRelationshipFeatureProvider = modelRelationshipFeatureProvider;

            Nodes = nodes.Values.ToImmutableHashSet();
            Connectors = connectors.Values.ToImmutableHashSet();
            Rect = CalculateRect();
            IsEmpty = !_nodes.Any() && !_connectors.Any();
            _diagramGraphsByRelationshipTransitivityGroup =
                CreateDiagramGraphsByRelationshipTransitivityGroup(modelRelationshipFeatureProvider, Nodes, Connectors);
        }

        [NotNull]
        private static IDictionary<string, IDiagramGraph> CreateDiagramGraphsByRelationshipTransitivityGroup(
            [NotNull] IModelRelationshipFeatureProvider modelRelationshipFeatureProvider,
            [NotNull] [ItemNotNull] IImmutableSet<IDiagramNode> nodes,
            [NotNull] [ItemNotNull] IImmutableSet<IDiagramConnector> connectors)
        {
            return connectors.GroupBy(i => modelRelationshipFeatureProvider.GetTransitivityPartitionKey(i.ModelRelationship.Stereotype))
                .ToDictionary(i => i.Key, i => DiagramGraph.Create(nodes, i.ToImmutableHashSet()));
        }

        public bool NodeExists(ModelNodeId modelNodeId) => Nodes.Any(i => i.Id == modelNodeId);
        public bool ConnectorExists(ModelRelationshipId modelRelationshipId) => Connectors.Any(i => i.Id == modelRelationshipId);

        public bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId, ModelRelationshipStereotype stereotype)
        {
            return NodeExists(sourceModelNodeId) && NodeExists(targetModelNodeId) && GetGraph(stereotype).PathExists(sourceModelNodeId, targetModelNodeId);
        }

        public bool PathExists(Maybe<ModelNodeId> maybeSourceModelNodeId, Maybe<ModelNodeId> maybeTargetModelNodeId, ModelRelationshipStereotype stereotype)
        {
            return maybeSourceModelNodeId.Match(
                sourceNodeId => maybeTargetModelNodeId.Match(
                    targetNodeId => PathExists(sourceNodeId, targetNodeId, stereotype),
                    () => false),
                () => false);
        }

        public bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId, ModelRelationshipStereotype stereotype)
        {
            return GetGraph(stereotype).IsEdgeRedundant(modelRelationshipId);
        }

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

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            return modelNodeIds
                .Select(i => TryGetNode(i).Match(j => j.AbsoluteRect, () => Rect2D.Undefined))
                .Union();
        }

        private Rect2D CalculateRect() => Nodes.Select(i => i.AbsoluteRect).Concat(Connectors.Select(i => i.AbsoluteRect)).Union();

        [NotNull]
        private IDiagramGraph GetGraph(ModelRelationshipStereotype stereotype)
        {
            var transitivityPartitionKey = _modelRelationshipFeatureProvider.GetTransitivityPartitionKey(stereotype);
            _diagramGraphsByRelationshipTransitivityGroup.TryGetValue(transitivityPartitionKey, out var graph);
            return graph ?? DiagramGraph.Empty();
        }

        [NotNull]
        public static IDiagram Create(
            [NotNull] IModel model,
            [NotNull] IModelRelationshipFeatureProvider modelRelationshipFeatureProvider)
        {
            return new ImmutableDiagram(
                model,
                ImmutableDictionary<ModelNodeId, IDiagramNode>.Empty,
                ImmutableDictionary<ModelRelationshipId, IDiagramConnector>.Empty,
                modelRelationshipFeatureProvider);
        }
    }
}