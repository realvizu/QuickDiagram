using System;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling;
using Codartis.Util;
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

            return CreateInstance(_graph.UpdateVertices(i => i is IContainerDiagramNode, i => (i as IContainerDiagramNode).AddNode(node, parentNodeId.Value)));
        }

        public ILayoutGroup UpdateNode(IDiagramNode updatedNode)
        {
            return CreateInstance(
                _graph.ContainsVertex(updatedNode)
                    ? _graph.UpdateVertex(updatedNode)
                    : _graph.UpdateVertices(i => i is IContainerDiagramNode, i => (i as IContainerDiagramNode).UpdateNode(updatedNode)));
        }

        public ILayoutGroup RemoveNode(ModelNodeId nodeId)
        {
            return _graph.TryGetVertex(nodeId).Match(
                i => CreateInstance(_graph.RemoveVertex(nodeId)),
                () => CreateInstance(_graph.UpdateVertices(j => j is IContainerDiagramNode, j => (j as IContainerDiagramNode).RemoveNode(nodeId))));
        }

        public ILayoutGroup AddConnector(IDiagramConnector connector)
        {
            if (connector.IsCrossingLayoutGroups)
                throw new InvalidOperationException($"Cannot add connector {connector} to layout group {_layoutGroupNodeId} because is crosses layout groups.");

            return CreateInstance(
                _graph.ContainsVertex(connector.Source)
                    ? _graph.AddEdge(connector)
                    : _graph.UpdateVertices(i => i is IContainerDiagramNode, i => (i as IContainerDiagramNode).AddConnector(connector)));
        }

        public ILayoutGroup UpdateConnector(IDiagramConnector updatedConnector)
        {
            if (updatedConnector.IsCrossingLayoutGroups)
                throw new InvalidOperationException($"Cannot update connector {updatedConnector} in layout group {_layoutGroupNodeId} because is crosses layout groups.");

            return CreateInstance(
                _graph.ContainsVertex(updatedConnector.Source)
                    ? _graph.UpdateEdge(updatedConnector)
                    : _graph.UpdateVertices(i => i is IContainerDiagramNode, i => (i as IContainerDiagramNode).UpdateConnector(updatedConnector)));
        }

        public ILayoutGroup RemoveConnector(ModelRelationshipId connectorId)
        {
            return _graph.TryGetEdge(connectorId).Match(
                i => CreateInstance(_graph.RemoveEdge(connectorId)),
                () => CreateInstance(
                    _graph.UpdateVertices(j => j is IContainerDiagramNode, j => (j as IContainerDiagramNode).RemoveConnector(connectorId)))
            );
        }

        public ILayoutGroup Clear() => CreateInstance(_graph.Clear());

        [NotNull]
        private ILayoutGroup CreateInstance([NotNull] IDiagramGraph graph) => new LayoutGroup(_layoutGroupNodeId, graph);
    }
}