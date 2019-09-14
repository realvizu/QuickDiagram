using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    public sealed class LayoutGroup : ILayoutGroup
    {
        public static readonly LayoutGroup Empty = new LayoutGroup(ImmutableHashSet<IDiagramNode>.Empty, ImmutableHashSet<IDiagramConnector>.Empty);

        [NotNull] private readonly IImmutableDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _connectors;

        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        public Rect2D Rect { get; }

        private LayoutGroup(
            [NotNull] IImmutableSet<IDiagramNode> nodes,
            [NotNull] IImmutableSet<IDiagramConnector> connectors)
        {
            _nodes = nodes.ToImmutableDictionary(i => i.Id);
            _connectors = connectors.ToImmutableDictionary(i => i.Id);

            Nodes = nodes;
            Connectors = connectors;
            Rect = CalculateRect(nodes, connectors);
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
        public static ILayoutGroup Create([NotNull] IImmutableSet<IDiagramNode> nodes, [NotNull] IImmutableSet<IDiagramConnector> connectors)
        {
            return AreEmpty(nodes, connectors)
                ? Empty
                : new LayoutGroup(nodes, connectors);
        }
    }
}