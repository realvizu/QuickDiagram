using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    public sealed class LayoutGroup : ILayoutGroup
    {
        [NotNull] private readonly IDictionary<ModelNodeId, IDiagramNode> _nodes;
        [NotNull] private readonly IDictionary<ModelRelationshipId, IDiagramConnector> _connectors;

        public ISet<IDiagramNode> Nodes { get; }
        public ISet<IDiagramConnector> Connectors { get; }

        public LayoutGroup()
            : this(
                new HashSet<IDiagramNode>(),
                new HashSet<IDiagramConnector>())
        {
        }

        public LayoutGroup(
            [NotNull] [ItemNotNull] ISet<IDiagramNode> nodes,
            [NotNull] [ItemNotNull] ISet<IDiagramConnector> connectors)
        {
            Nodes = nodes;
            Connectors = connectors;

            _nodes = Nodes.ToDictionary(i => i.Id);
            _connectors = Connectors.ToDictionary(i => i.Id);
        }

        public bool IsEmpty => AreEmpty(Nodes, Connectors);

        public IDiagramNode GetNode(ModelNodeId nodeId) => _nodes[nodeId];

        public IDiagramConnector GetConnector(ModelRelationshipId relationshipId) => _connectors[relationshipId];

        private static bool AreEmpty([NotNull] IEnumerable<IDiagramNode> nodes, [NotNull] IEnumerable<IDiagramConnector> connectors)
        {
            return !nodes.Any() && !connectors.Any();
        }

        [NotNull]
        public static ILayoutGroup Create(
            [NotNull] [ItemNotNull] IEnumerable<IDiagramNode> nodes,
            [NotNull] [ItemNotNull] IEnumerable<IDiagramConnector> connectors)
        {
            var nodeSet = nodes.ToHashSet();
            var connectorSet = connectors.ToHashSet();

            return new LayoutGroup(nodeSet, connectorSet);
        }
    }
}