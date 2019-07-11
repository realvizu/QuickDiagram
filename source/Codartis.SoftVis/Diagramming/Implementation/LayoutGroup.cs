using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    public sealed class LayoutGroup : ILayoutGroup
    {
        [NotNull]
        public static ILayoutGroup Empty(ModelNodeId? parentNodeId = null)
        {
            return new LayoutGroup(parentNodeId, DiagramGraph.Empty);
        }

        private readonly ModelNodeId? _parentNodeId;
        [NotNull] private readonly DiagramGraph _graph;

        public LayoutGroup(ModelNodeId? parentNodeId, [NotNull] DiagramGraph graph)
        {
            _graph = graph;
            _parentNodeId = parentNodeId;
        }

        public IImmutableSet<IDiagramNode> Nodes => _graph.Vertices.ToImmutableHashSet();
        public IImmutableSet<IDiagramConnector> Connectors => _graph.Edges.ToImmutableHashSet();

        public Rect2D Rect => Nodes.OfType<IDiagramShape>().Union(Connectors).Where(i => i.IsRectDefined).Select(i => i.Rect).Union();

        public ILayoutGroup WithNode(IDiagramNode node, ModelNodeId? parentNodeId = null)
        {
            if (parentNodeId == _parentNodeId)
                return CreateInstance(_graph.AddVertex(node));

            var updatedNodes = Nodes.OfType<IContainerDiagramNode>().Select(i => i.WithLayoutGroup(i.LayoutGroup.WithNode(node, parentNodeId)));

            var updatedGraph = _graph;
            foreach (var updatedNode in updatedNodes)
                updatedGraph = updatedGraph.UpdateVertex(updatedNode);

            return CreateInstance(updatedGraph);
        }

        public ILayoutGroup WithoutNode(IDiagramNode node) => CreateInstance(_graph.RemoveVertex(node.Id));
        public ILayoutGroup WithConnector(IDiagramConnector connector) => CreateInstance(_graph.AddEdge(connector));
        public ILayoutGroup WithoutConnector(IDiagramConnector connector) => CreateInstance(_graph.RemoveEdge(connector.Id));
        public ILayoutGroup Clear() => CreateInstance(_graph.Clear());

        [NotNull]
        private ILayoutGroup CreateInstance([NotNull] DiagramGraph graph) => new LayoutGroup(_parentNodeId, graph);
    }
}