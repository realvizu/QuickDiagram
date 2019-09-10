using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    public sealed class LayoutGroup : ILayoutGroup
    {
        public static readonly LayoutGroup Empty = new LayoutGroup(ImmutableHashSet<IDiagramNode>.Empty, ImmutableHashSet<IDiagramConnector>.Empty);

        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        public Rect2D Rect { get; }

        private LayoutGroup(
            [NotNull] IImmutableSet<IDiagramNode> nodes,
            [NotNull] IImmutableSet<IDiagramConnector> connectors)
        {
            Nodes = nodes;
            Connectors = connectors;
            Rect = CalculateRect(nodes, connectors);
        }

        public bool IsEmpty => AreEmpty(Nodes, Connectors);

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