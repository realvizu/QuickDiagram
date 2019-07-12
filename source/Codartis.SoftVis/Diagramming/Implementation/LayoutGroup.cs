using System;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    using IDiagramGraph = IImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId>;
    using DiagramGraph = ImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId>;

    public sealed class LayoutGroup : ILayoutGroup
    {
        [NotNull]
        public static ILayoutGroup Empty(ModelNodeId? parentNodeId = null)
        {
            return new LayoutGroup(parentNodeId, DiagramGraph.Empty(allowParallelEdges: false));
        }

        private readonly ModelNodeId? _layoutGroupNodeId;
        [NotNull] private readonly IDiagramGraph _graph;

        public LayoutGroup(ModelNodeId? layoutGroupNodeId, [NotNull] IDiagramGraph graph)
        {
            _graph = graph;
            _layoutGroupNodeId = layoutGroupNodeId;
        }

        public IImmutableSet<IDiagramNode> Nodes => _graph.Vertices.ToImmutableHashSet();
        public IImmutableSet<IDiagramConnector> Connectors => _graph.Edges.ToImmutableHashSet();

        public IImmutableSet<IDiagramNode> NodesRecursive
        {
            get
            {
                return _graph.Vertices
                    .Union(Nodes.OfType<IContainerDiagramNode>().SelectMany(i => i.LayoutGroup.NodesRecursive))
                    .ToImmutableHashSet();
            }
        }

        public IImmutableSet<IDiagramConnector> ConnectorsRecursive
        {
            get
            {
                return _graph.Edges
                    .Union(Nodes.OfType<IContainerDiagramNode>().SelectMany(i => i.LayoutGroup.ConnectorsRecursive))
                    .ToImmutableHashSet();
            }
        }

        public Rect2D Rect => Nodes.OfType<IDiagramShape>().Union(Connectors).Where(i => i.IsRectDefined).Select(i => i.Rect).Union();

        public ILayoutGroup AddNode(IDiagramNode node, ModelNodeId? parentNodeId = null)
        {
            if (parentNodeId == _layoutGroupNodeId)
                return CreateInstance(_graph.AddVertex(node.WithParentNodeId(parentNodeId)));

            if (parentNodeId == null)
                throw new Exception($"ParentNodeId should not be null.");

            // TODO: make it DRY: _graph.UpdateVertices?
            var updatedNodes = Nodes.OfType<IContainerDiagramNode>().Select(i => i.AddNode(node, parentNodeId.Value));

            var updatedGraph = _graph;
            foreach (var updatedNode in updatedNodes)
                updatedGraph = updatedGraph.UpdateVertex(updatedNode);

            return CreateInstance(updatedGraph);
        }

        public ILayoutGroup RemoveNode(IDiagramNode node)
        {
            throw new NotImplementedException();

            //return CreateInstance(_graph.RemoveVertex(node.Id));
        }

        public ILayoutGroup AddConnector(IDiagramConnector connector)
        {
            if (connector.IsCrossingLayoutGroups)
                throw new InvalidOperationException($"Cannot add connector {connector} to layout group {_layoutGroupNodeId} because is crosses layout groups.");

            if (_graph.ContainsVertex(connector.Source))
                return CreateInstance(_graph.AddEdge(connector));

            // TODO: make it DRY: _graph.UpdateVertices?
            var updatedNodes = Nodes.OfType<IContainerDiagramNode>().Select(i => i.AddConnector(connector));

            var updatedGraph = _graph;
            foreach (var updatedNode in updatedNodes)
                updatedGraph = updatedGraph.UpdateVertex(updatedNode);

            return CreateInstance(updatedGraph);
        }

        public ILayoutGroup RemoveConnector(IDiagramConnector connector)
        {
            throw new NotImplementedException();

            //return CreateInstance(_graph.RemoveEdge(connector.Id));
        }

        public ILayoutGroup Clear() => CreateInstance(_graph.Clear());

        [NotNull]
        private ILayoutGroup CreateInstance([NotNull] IDiagramGraph graph) => new LayoutGroup(_layoutGroupNodeId, graph);
    }
}