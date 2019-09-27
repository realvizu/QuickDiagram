using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    public sealed class LayoutGroup : ILayoutGroup
    {
        [NotNull] private readonly IDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IDictionary<ModelRelationshipId, IDiagramConnector> _connectors;

        public Maybe<ModelNodeId> ContainerNodeId { get; }
        public ISet<IDiagramNode> Nodes { get; }
        public ISet<IDiagramConnector> Connectors { get; }
        public Rect2D Rect { get; }

        public LayoutGroup()
            : this(
                Maybe<ModelNodeId>.Nothing,
                new HashSet<IDiagramNode>(),
                new HashSet<IDiagramConnector>())
        {
        }

        public LayoutGroup(
            Maybe<ModelNodeId> containerNodeId,
            [NotNull] [ItemNotNull] ISet<IDiagramNode> nodes,
            [NotNull] [ItemNotNull] ISet<IDiagramConnector> connectors)
        {
            ContainerNodeId = containerNodeId;
            Nodes = nodes;
            Connectors = connectors;

            _nodes = Nodes.ToDictionary(i => i.Id);
            _connectors = Connectors.ToDictionary(i => i.Id);

            Rect = CalculateRect(Nodes, Connectors);
        }

        public bool IsEmpty => AreEmpty(Nodes, Connectors);

        public IDiagramNode GetNode(ModelNodeId nodeId) => _nodes[nodeId];
        public IDiagramConnector GetConnector(ModelRelationshipId relationshipId) => _connectors[relationshipId];

        public void SetChildrenAreaSize(ModelNodeId nodeId, Size2D childrenAreaSize)
        {
            _nodes[nodeId] = _nodes[nodeId].WithChildrenAreaSize(childrenAreaSize);
        }

        private static Rect2D CalculateRect(
            [NotNull] [ItemNotNull] IEnumerable<IDiagramNode> nodes,
            [NotNull] [ItemNotNull] IEnumerable<IDiagramConnector> connectors)
        {
            return nodes.OfType<IDiagramShape>().Union(connectors).Where(i => i.Rect.IsDefined()).Select(i => i.Rect).Union();
        }

        private static bool AreEmpty([NotNull] IEnumerable<IDiagramNode> nodes, [NotNull] IEnumerable<IDiagramConnector> connectors)
        {
            return !nodes.Any() && !connectors.Any();
        }

        [NotNull]
        public static ILayoutGroup CreateForNode(
            Maybe<ModelNodeId> containerNodeId,
            [NotNull] [ItemNotNull] IEnumerable<IDiagramNode> nodes,
            [NotNull] [ItemNotNull] IEnumerable<IDiagramConnector> connectors)
        {
            var nodeSet = nodes.ToHashSet();
            var connectorSet = connectors.ToHashSet();

            return new LayoutGroup(containerNodeId, nodeSet, connectorSet);
        }

        [NotNull]
        public static ILayoutGroup CreateForRoot(
            [NotNull] [ItemNotNull] IEnumerable<IDiagramNode> nodes,
            [NotNull] [ItemNotNull] IEnumerable<IDiagramConnector> connectors)
        {
            return CreateForNode(Maybe<ModelNodeId>.Nothing, nodes, connectors);
        }
    }
}