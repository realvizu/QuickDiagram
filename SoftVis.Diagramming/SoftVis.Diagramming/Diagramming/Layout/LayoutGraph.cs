using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A graph created from a DiagramGraph that the layout logic works upon.
    /// It also performs the layerig of the vertices where each edge points to a layer with lower index.
    /// It also offers methods the determine the parents, children and siblings of vertices.
    /// </summary>
    /// <remarks>
    /// LayoutGraph differs from the original DiagramGraph in the following ways:
    /// <para>Dummy vertices can be added to break long edges and ensure that neighbours are always on adjacent layers.</para>
    /// <para>An edge in the original graph can be represented by a path (a list of edges) in the LayoutGraph.</para>
    /// <para>Edges can be reversed to ensure an acyclic graph.</para>
    /// </remarks>
    internal sealed class LayoutGraph : BidirectionalGraph<LayoutVertexBase, LayoutEdge>
    {
        private readonly LayoutVertexLayers _vertexLayers;

        public LayoutGraph(double verticalGap) : base(false)
        {
            _vertexLayers = new LayoutVertexLayers(verticalGap);

            Cleared += OnCleared;
        }

        public IEnumerable<LayoutVertexLayer> Layers => _vertexLayers;

        public override bool AddVertex(LayoutVertexBase layoutVertex)
        {
            var isAdded = base.AddVertex(layoutVertex);
            if (isAdded)
                _vertexLayers.AddVertex(layoutVertex);

            return isAdded;
        }

        public override bool RemoveVertex(LayoutVertexBase layoutVertex)
        {
            var isRemoved = base.RemoveVertex(layoutVertex);
            if (isRemoved)
                _vertexLayers.RemoveVertex(layoutVertex);

            return isRemoved;
        }

        public override bool AddEdge(LayoutEdge newEdge)
        {
            var isAdded = base.AddEdge(newEdge);
            if (isAdded)
                newEdge.ExecuteOnDescendantEdges(i => _vertexLayers.EnsureValidLayering(i.Source, i.Target));

            return isAdded;
        }

        public override bool RemoveEdge(LayoutEdge layoutEdge)
        {
            var isRemoved = base.RemoveEdge(layoutEdge);
            return isRemoved;
        }

        public void AddPath(LayoutPath layoutPath)
        {
            foreach (var layoutEdge in layoutPath)
            {
                AddVertex(layoutEdge.Source);
                AddVertex(layoutEdge.Target);
                AddEdge(layoutEdge);
            }
        }

        public IEnumerable<LayoutVertexBase> GetParents(LayoutVertexBase vertex)
        {
            return this.GetOutNeighbours(vertex);
        }

        public LayoutVertexBase GetPrimaryParent(LayoutVertexBase layoutVertex)
        {
            return GetOrderedParents(layoutVertex).FirstOrDefault();
        }

        public IEnumerable<LayoutVertexBase> GetNonPrimaryParents(LayoutVertexBase layoutVertex)
        {
            return GetOrderedParents(layoutVertex).Skip(1);
        }

        public IEnumerable<LayoutVertexBase> GetChildren(LayoutVertexBase vertex)
        {
            return this.GetInNeighbours(vertex);
        }

        public IEnumerable<LayoutVertexBase> GetPrimaryChildren(LayoutVertexBase layoutVertex)
        {
            return GetChildren(layoutVertex).Where(i => i.GetPrimaryParent() == layoutVertex);
        }

        public IEnumerable<LayoutVertexBase> GetPrimaryPositionedChildren(LayoutVertexBase layoutVertex)
        {
            return GetPrimaryChildren(layoutVertex).Where(i => !i.IsFloating);
        }

        public IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase layoutVertex)
        {
            return GetPrimaryParent(layoutVertex)?.GetPrimaryChildren().Where(i => i != layoutVertex)
                ?? Enumerable.Empty<LayoutVertexBase>();
        }

        public IEnumerable<LayoutEdge> GetPrimaryEdges(LayoutVertexBase layoutVertex)
        {
            return InEdges(layoutVertex).Where(i => i.Source.GetPrimaryParent() == layoutVertex);
        }

        public void ExecuteOnPrimaryDescendantVertices(LayoutVertexBase rootVertex, Action<LayoutVertexBase> actionOnVertex)
        {
            actionOnVertex(rootVertex);
            foreach (var layoutEdge in GetPrimaryEdges(rootVertex))
            {
                var nextVertex = layoutEdge.Source;
                ExecuteOnPrimaryDescendantVertices(nextVertex, actionOnVertex);
            }
        }

        public void ExecuteOnDescendantEdges(LayoutEdge edge, Action<LayoutEdge> action)
        {
            this.ExecuteOnEdgesRecursive(edge, EdgeDirection.In, action);
        }

        public LayoutVertexLayer GetLayer(LayoutVertexBase layoutVertex) => _vertexLayers.GetLayer(layoutVertex);
        public int GetLayerIndex(LayoutVertexBase layoutVertex) => _vertexLayers.GetLayerIndex(layoutVertex);

        private IEnumerable<LayoutVertexBase> GetOrderedParents(LayoutVertexBase layoutVertex)
        {
            var layerIndex = layoutVertex.LayerIndex;

            return GetParents(layoutVertex)
                .OrderByDescending(i => i.Priority)
                .ThenBy(i => layerIndex - i.LayerIndex)
                .ThenBy(i => i.ToString());
        }

        private void OnCleared(object sender, EventArgs args)
        {
            _vertexLayers.Clear();
        }
    }
}
