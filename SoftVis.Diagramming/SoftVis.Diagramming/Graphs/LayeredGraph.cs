using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Util;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    /// <summary>
    /// TODO: ensure acyclicity by turning around edges when necessary.
    /// A graph that contains no directed cycles and its vertices are arranged into layers.
    /// Layers are indexed from 0 (topmost). 
    /// All edges point upwards (to a vertex with lower layer index).
    /// </summary>
    /// <remarks>
    /// Terms:
    /// <para>Higher layer: a layer with a lower index.</para>
    /// <para>Parents: direct neighbours of a vertex on higher layers.</para>
    /// <para>Children: direct neighbours of a vertex on lower layers.</para>
    /// <para>Siblings: vertices that have a common parent.</para>
    /// Invariant:
    /// <para>For all vertices and their parents: LayerIndex(vertex) > LayerIndex(vertex.Parent)</para>
    /// </remarks>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    internal class LayeredGraph<TVertex, TEdge> : ConcurrentBidirectionalGraph<TVertex, TEdge>,
        IReadOnlyLayeredGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly Map<TVertex, int> _vertexToLayerIndexMap;

        public LayeredGraph()
        {
            _vertexToLayerIndexMap = new Map<TVertex, int>();
        }

        public override bool AddVertex(TVertex vertex)
        {
            if (!base.AddVertex(vertex))
                return false;

            _vertexToLayerIndexMap.Set(vertex, 0);

            CheckInvariant();
            return true;
        }

        public override bool RemoveVertex(TVertex vertex)
        {
            if (!base.RemoveVertex(vertex))
                return false;

            _vertexToLayerIndexMap.Remove(vertex);

            CheckInvariant();
            return true;
        }

        public override bool AddEdge(TEdge edge)
        {
            if (!base.AddEdge(edge))
                return false;

            ExecuteOnVertexAndDescendants(edge.Source, UpdateLayerIndex);

            CheckInvariant();
            return true;
        }

        public override bool RemoveEdge(TEdge edge)
        {
            if (!base.RemoveEdge(edge))
                return false;

            ExecuteOnVertexAndDescendants(edge.Source, UpdateLayerIndex);

            CheckInvariant();
            return true;
        }

        public IEnumerable<TVertex> GetParents(TVertex vertex)
        {
            return this.GetOutNeighbours(vertex);
        }

        public IEnumerable<TVertex> GetChildren(TVertex vertex)
        {
            return this.GetInNeighbours(vertex);
        }

        public IEnumerable<TVertex> GetSiblings(TVertex vertex)
        {
            var parentVertices = GetParents(vertex).ToList();
            return Vertices.Where(i => !vertex.Equals(i) && parentVertices.Intersect(GetParents(i)).Any());
        }

        public IEnumerable<TVertex> GetDescendants(TVertex vertex)
        {
            return GetChildren(vertex).SelectMany(GetVertexAndDescendants).Distinct();
        }

        public IEnumerable<TVertex> GetVertexAndDescendants(TVertex vertex)
        {
            yield return vertex;
            foreach (var descendant in GetDescendants(vertex))
                yield return descendant;
        }

        public bool HasChildren(TVertex vertex)
        {
            return InDegree(vertex) > 0;
        }

        /// <summary>
        /// Returns a value that is one higher than the rank of all parent vertices, or zero if there's no parent vertex.
        /// </summary>
        /// <param name="vertex">A vertex.</param>
        /// <returns>The rank of the given vertex.</returns>
        /// <remarks>Rank(vertex) >= LayerIndex(vertex)</remarks>
        public int GetRank(TVertex vertex)
        {
            return GetParents(vertex).Select(GetRank).DefaultIfEmpty(-1).Max() + 1;
        }

        public int GetLayerIndex(TVertex vertex)
        {
            return _vertexToLayerIndexMap.Get(vertex);
        }

        protected void ExecuteOnVertexAndDescendants(TVertex rootVertex, Action<TVertex> actionOnVertex)
        {
            actionOnVertex(rootVertex);

            foreach (var child in GetChildren(rootVertex))
                ExecuteOnVertexAndDescendants(child, actionOnVertex);
        }

        private void UpdateLayerIndex(TVertex vertex)
        {
            var newLayerIndex = CalculateLayerIndex(vertex);
            _vertexToLayerIndexMap.Set(vertex, newLayerIndex);
        }

        private int CalculateLayerIndex(TVertex vertex)
        {
            return GetRank(vertex);
        }

        private void CheckInvariant()
        {
            foreach (var vertex in Vertices)
            {
                var layerIndex = GetLayerIndex(vertex);
                foreach (var parentVertex in GetParents(vertex))
                {
                    var parentLayerIndex = GetLayerIndex(parentVertex);
                    if (layerIndex <= parentLayerIndex)
                        throw new Exception($"Vertex {vertex} at layer {layerIndex} should be higher then its parent {parentVertex} at {parentLayerIndex}.");
                }
            }
        }
    }
}
