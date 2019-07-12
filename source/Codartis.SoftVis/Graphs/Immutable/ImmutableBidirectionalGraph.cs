using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// Implements an immutable bidirectional graph.
    /// All mutators return a new instance of the graph.
    /// </summary>
    /// <remarks>
    /// All vertices and edges must have a stable ID.
    /// When a vertex or an edge is updated, it is searched and replaced by ID.
    /// Because the IDs never change we keep track of the graph structure as the graph of vertex IDs and edge IDs.
    /// Vertices and edges are stored in dictionaries keyed by their IDs.
    /// WARNING: Descendants must override the method that creates a new object of the descendant type.
    /// </remarks>
    public sealed class ImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> :
        IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId>
        where TVertex : IImmutableVertex<TVertexId>
        where TVertexId : IEquatable<TVertexId>
        where TEdge : IImmutableEdge<TVertex, TVertexId, TEdge, TEdgeId>
        where TEdgeId : IEquatable<TEdgeId>
    {
        [NotNull] private static readonly IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> EmptyWithAllowParallelEdges = CreateEmpty(true);
        [NotNull] private static readonly IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> EmptyWithDisallowParallelEdges = CreateEmpty(false);

        private readonly ImmutableDictionary<TVertexId, TVertex> _vertices;
        private readonly ImmutableDictionary<TEdgeId, TEdge> _edges;
        private readonly BidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> _graph;
        private readonly bool _allowParallelEdges;

        public ImmutableBidirectionalGraph(
            ImmutableDictionary<TVertexId, TVertex> vertices,
            ImmutableDictionary<TEdgeId, TEdge> edges,
            BidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> graph)
        {
            _vertices = vertices;
            _edges = edges;
            _graph = graph;
            _allowParallelEdges = graph.AllowParallelEdges;
        }

        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;
        public bool ContainsVertex(TVertex v) => _graph.ContainsVertex(v.Id);
        public bool IsOutEdgesEmpty(TVertex v) => _graph.IsOutEdgesEmpty(v.Id);
        public int OutDegree(TVertex v) => _graph.OutDegree(v.Id);
        public IEnumerable<TEdge> OutEdges(TVertex v) => _graph.OutEdges(v.Id).Select(FromVertexIdEdge);
        public TEdge OutEdge(TVertex v, int index) => FromVertexIdEdge(_graph.OutEdge(v.Id, index));
        public bool ContainsEdge(TVertex source, TVertex target) => _graph.ContainsEdge(source.Id, target.Id);
        public bool IsVerticesEmpty => _graph.IsVerticesEmpty;
        public int VertexCount => _graph.VertexCount;
        public IEnumerable<TVertex> Vertices => _vertices.Values;
        public bool ContainsEdge(TEdge edge) => _edges.ContainsKey(edge.Id);
        public bool IsEdgesEmpty => _graph.IsEdgesEmpty;
        public int EdgeCount => _graph.EdgeCount;
        public IEnumerable<TEdge> Edges => _edges.Values;
        public bool IsInEdgesEmpty(TVertex v) => _graph.IsInEdgesEmpty(v.Id);
        public int InDegree(TVertex v) => _graph.InDegree(v.Id);
        public IEnumerable<TEdge> InEdges(TVertex v) => _graph.InEdges(v.Id).Select(FromVertexIdEdge);
        public TEdge InEdge(TVertex v, int index) => FromVertexIdEdge(_graph.InEdge(v.Id, index));
        public int Degree(TVertex v) => _graph.Degree(v.Id);

        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            var result = _graph.TryGetOutEdges(v.Id, out var vertexIdEdges);
            edges = vertexIdEdges.Select(FromVertexIdEdge);
            return result;
        }

        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            var result = _graph.TryGetInEdges(v.Id, out var vertexIdEdges);
            edges = vertexIdEdges.Select(FromVertexIdEdge);
            return result;
        }

        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            var result = _graph.TryGetEdges(source.Id, target.Id, out var vertexIdEdges);
            edges = vertexIdEdges.Select(FromVertexIdEdge);
            return result;
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (_graph.TryGetEdge(source.Id, target.Id, out var vertexIdEdge))
            {
                edge = FromVertexIdEdge(vertexIdEdge);
                return true;
            }

            edge = default;
            return false;
        }

        public IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> AddVertex(TVertex v)
        {
            EnsureNoVertexId(v.Id);

            var updatedVertices = _vertices.Add(v.Id, v);
            var updatedGraph = CloneAndMutateGraph(i => i.AddVertex(v.Id));
            return CreateInstance(updatedVertices, _edges, updatedGraph);
        }

        public IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> UpdateVertex(TVertex newVertex)
        {
            EnsureVertexId(newVertex.Id);

            var updatedVertices = _vertices.SetItem(newVertex.Id, newVertex);
            var updatedEdges = ReplaceSourceAndTargetVertexInEdges(newVertex);
            return CreateInstance(updatedVertices, updatedEdges, _graph);
        }

        public IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> RemoveVertex(TVertexId vertexId)
        {
            EnsureVertexId(vertexId);

            var updatedVertices = _vertices.Remove(vertexId);

            var updatedEdges = _edges;
            foreach (var edge in _edges.Values.Where(i => i.Source.Id.Equals(vertexId) || i.Target.Id.Equals(vertexId)))
                updatedEdges = updatedEdges.Remove(edge.Id);

            var updatedGraph = CloneAndMutateGraph(i => i.RemoveVertex(vertexId));
            return CreateInstance(updatedVertices, updatedEdges, updatedGraph);
        }

        public IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> AddEdge(TEdge edge)
        {
            EnsureNoEdgeId(edge.Id);
            EnsureVertexId(edge.Source.Id);
            EnsureVertexId(edge.Target.Id);

            var updatedEdges = _edges.Add(edge.Id, edge);
            var updatedGraph = CloneAndMutateGraph(i => i.AddEdge(ToVertexIdEdge(edge)));
            return CreateInstance(_vertices, updatedEdges, updatedGraph);
        }

        public IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> UpdateEdge(TEdge newEdge)
        {
            EnsureEdgeId(newEdge.Id);
            EnsureVertexId(newEdge.Source.Id);
            EnsureVertexId(newEdge.Target.Id);

            var updatedEdges = _edges.SetItem(newEdge.Id, newEdge);
            return CreateInstance(_vertices, updatedEdges, _graph);
        }

        public IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> RemoveEdge(TEdgeId edgeId)
        {
            EnsureEdgeId(edgeId);

            var updatedEdges = _edges.Remove(edgeId);
            var updatedGraph = CloneAndMutateGraph(i => i.RemoveEdgeIf(j => j.Id.Equals(edgeId)));
            return CreateInstance(_vertices, updatedEdges, updatedGraph);
        }

        public IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> Clear() => Empty(_allowParallelEdges);

        public bool PathExists(TVertexId sourceVertexId, TVertexId targetVertexId)
        {
            return _graph.PathExists(sourceVertexId, targetVertexId);
        }

        public IEnumerable<TVertex> GetAdjacentVertices(
            TVertexId vertexId,
            EdgeDirection direction,
            EdgePredicate<TVertex, TEdge> edgePredicate = null,
            bool recursive = false)
        {
            if (!_vertices.ContainsKey(vertexId))
                return Enumerable.Empty<TVertex>();

            var vertexIds = _graph.GetAdjacentVertices(
                vertexId,
                direction,
                edge => edgePredicate?.Invoke(FromEdgeId(edge.Id)) != false,
                recursive);

            return vertexIds.Select(FromVertexId);
        }

        private TVertexId ToVertexId(TVertex vertex) => vertex.Id;
        private TVertex FromVertexId(TVertexId vertexId) => _vertices[vertexId];
        private TEdge FromEdgeId(TEdgeId edgeId) => _edges[edgeId];

        private VertexIdEdge<TVertexId, TEdgeId> ToVertexIdEdge(TEdge edge) => new VertexIdEdge<TVertexId, TEdgeId>(edge.Id, edge.Source.Id, edge.Target.Id);
        private TEdge FromVertexIdEdge(VertexIdEdge<TVertexId, TEdgeId> vertexIdEdge) => FromEdgeId(vertexIdEdge.Id);
        private IEnumerable<TEdge> FromVertexIdEdge(IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges) => vertexIdEdges.Select(FromVertexIdEdge);

        private void EnsureVertexId(TVertexId id)
        {
            if (!_vertices.ContainsKey(id))
                throw new InvalidOperationException($"Graph does not contain a vertex with id: {id}");
        }

        private void EnsureNoVertexId(TVertexId id)
        {
            if (_vertices.ContainsKey(id))
                throw new InvalidOperationException($"Model already contains a vertex with id {id}");
        }

        private void EnsureEdgeId(TEdgeId id)
        {
            if (!_edges.ContainsKey(id))
                throw new InvalidOperationException($"Graph does not contain an edge with id: {id}");
        }

        private void EnsureNoEdgeId(TEdgeId id)
        {
            if (_edges.ContainsKey(id))
                throw new InvalidOperationException($"Model already contains a edge with id {id}");
        }

        private ImmutableDictionary<TEdgeId, TEdge> ReplaceSourceAndTargetVertexInEdges(TVertex newVertex)
        {
            var updatedEdges = _edges;
            foreach (var edge in _edges.Values)
            {
                var updatedEdge = edge;
                if (edge.Source.Id.Equals(newVertex.Id))
                    updatedEdge = edge.WithSource(newVertex);
                if (edge.Target.Id.Equals(newVertex.Id))
                    updatedEdge = edge.WithTarget(newVertex);

                if (!ReferenceEquals(edge, updatedEdge))
                    updatedEdges = updatedEdges.SetItem(edge.Id, updatedEdge);
            }

            return updatedEdges;
        }

        private BidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> CloneAndMutateGraph(
            [NotNull] Action<BidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>>> mutatorAction)
        {
            var graph = _graph.Clone();
            mutatorAction.Invoke(graph);
            return graph;
        }

        [NotNull]
        public static IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> Empty(bool allowParallelEdges = false)
        {
            return allowParallelEdges
                ? EmptyWithAllowParallelEdges
                : EmptyWithDisallowParallelEdges;
        }

        [NotNull]
        private static IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> CreateEmpty(bool allowParallelEdges)
        {
            return CreateInstance(
                ImmutableDictionary<TVertexId, TVertex>.Empty,
                ImmutableDictionary<TEdgeId, TEdge>.Empty,
                new BidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>>(allowParallelEdges));
        }

        [NotNull]
        private static IImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId> CreateInstance(
            ImmutableDictionary<TVertexId, TVertex> vertices,
            ImmutableDictionary<TEdgeId, TEdge> edges,
            BidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> graph)
        {
            return new ImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId>(vertices, edges, graph);
        }
    }
}