using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    public sealed class LayoutGroup : ILayoutGroup
    {
        [NotNull] private readonly IImmutableDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _connectors;

        public Maybe<ModelNodeId> ContainerNodeId { get; }
        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        public Rect2D Rect { get; }

        public LayoutGroup()
            : this(
                Maybe<ModelNodeId>.Nothing,
                ImmutableHashSet<IDiagramNode>.Empty,
                ImmutableHashSet<IDiagramConnector>.Empty)
        {
        }

        public LayoutGroup(
            Maybe<ModelNodeId> containerNodeId,
            [NotNull] IImmutableSet<IDiagramNode> nodes,
            [NotNull] IImmutableSet<IDiagramConnector> connectors)
        {
            ContainerNodeId = containerNodeId;
            Nodes = nodes;
            Connectors = connectors;

            _nodes = Nodes.ToImmutableDictionary(i => i.Id);
            _connectors = Connectors.ToImmutableDictionary(i => i.Id);

            Rect = CalculateRect(Nodes, Connectors);
        }

        public bool IsEmpty => AreEmpty(Nodes, Connectors);

        public IDiagramNode GetNode(ModelNodeId nodeId) => _nodes[nodeId];
        public IDiagramConnector GetConnector(ModelRelationshipId relationshipId) => _connectors[relationshipId];

        private static Rect2D CalculateRect(
            [NotNull] IImmutableSet<IDiagramNode> nodes,
            [NotNull] IImmutableSet<IDiagramConnector> connectors)
        {
            return nodes.OfType<IDiagramShape>().Union(connectors).Where(i => i.Rect.IsDefined()).Select(i => i.Rect).Union();
        }

        private static bool AreEmpty([NotNull] IImmutableSet<IDiagramNode> nodes, [NotNull] IImmutableSet<IDiagramConnector> connectors)
        {
            return !nodes.Any() && !connectors.Any();
        }

        [NotNull]
        public static ILayoutGroup CreateForNode(
            Maybe<ModelNodeId> containerNodeId,
            [NotNull] IEnumerable<IDiagramNode> nodes,
            [NotNull] IEnumerable<IDiagramConnector> connectors)
        {
            var nodeSet = nodes.ToImmutableHashSet();
            var connectorSet = connectors.ToImmutableHashSet();

            return new LayoutGroup(containerNodeId, nodeSet, connectorSet);
        }

        [NotNull]
        public static ILayoutGroup CreateForRoot(
            [NotNull] IEnumerable<IDiagramNode> nodes,
            [NotNull] IEnumerable<IDiagramConnector> connectors)
        {
            return CreateForNode(Maybe<ModelNodeId>.Nothing, nodes, connectors);
        }
    }
}