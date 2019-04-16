using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// Immutable graph with vertex and edge replace operations.
    /// </summary>
    public class UpdatableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
        : IBidirectionalGraph<TVertex, TEdge>,
        IUpdatableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
        where TVertex : IUpdatableImmutableVertex<TVertexId>
        where TVertexId : IEquatable<TVertexId>, IComparable<TVertexId>
        where TEdge : IUpdatableImmutableEdge<TVertex, TVertexId, TEdge, TEdgeId>
        where TEdgeId : IEquatable<TEdgeId>, IComparable<TEdgeId>
        where TGraph : UpdatableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
    {
        private readonly ImmutableDictionary<TVertexId, TVertex> _vertices;
        private readonly ImmutableDictionary<TEdgeId, TEdge> _edges;
        private readonly ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> _graph;

        public UpdatableImmutableBidirectionalGraph(bool allowParallelEdges)
            : this(ImmutableDictionary<TVertexId, TVertex>.Empty,
                  ImmutableDictionary<TEdgeId, TEdge>.Empty,
                  new ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>>(allowParallelEdges: allowParallelEdges))
        {
        }

        protected UpdatableImmutableBidirectionalGraph(
            ImmutableDictionary<TVertexId, TVertex> vertices,
            ImmutableDictionary<TEdgeId, TEdge> edges,
            ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> graph)
        {
            _vertices = vertices;
            _edges = edges;
            _graph = graph;
        }

        public bool ContainsVertex(TVertexId vertexId) => _vertices.ContainsKey(vertexId);

        public TVertex GetVertex(TVertexId vertexId)
        {
            EnsureVertexId(vertexId);
            return FromVertexId(vertexId);
        }

        public bool TryGetVertex(TVertexId vertexId, out TVertex vertex)
        {
            if (ContainsVertex(vertexId))
            {
                vertex = FromVertexId(vertexId);
                return true;
            }

            vertex = default(TVertex);
            return false;
        }

        public bool ContainsEdge(TEdgeId edgeId) => _edges.ContainsKey(edgeId);

        public TEdge GetEdge(TEdgeId edgeId)
        {
            EnsureEdgeId(edgeId);
            return FromEdgeId(edgeId);
        }

        public bool TryGetEdge(TEdgeId edgeId, out TEdge edge)
        {
            if (ContainsEdge(edgeId))
            {
                edge = FromEdgeId(edgeId);
                return true;
            }

            edge = default(TEdge);
            return false;
        }

        public bool PathExists(TVertexId sourceVertexId, TVertexId targetVertexId) => _graph.PathExists(sourceVertexId, targetVertexId);

        public IEnumerable<TVertex> Vertices => _vertices.Values;
        public bool ContainsVertex(TVertex vertex) => ContainsVertex(vertex.Id);
        public int VertexCount => _vertices.Count;
        public bool IsVerticesEmpty => !_vertices.Any();

        public IEnumerable<TEdge> Edges => _edges.Values;
        public bool ContainsEdge(TEdge edge) => ContainsEdge(edge.Id);
        public int EdgeCount => _edges.Count;
        public bool IsEdgesEmpty => !_edges.Any();

        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;
        public bool ContainsEdge(TVertex source, TVertex target) => _graph.ContainsEdge(ToVertexId(source), ToVertexId(target));

        public bool IsOutEdgesEmpty(TVertex v)
        {
            EnsureVertexId(v.Id);
            return _graph.IsOutEdgesEmpty(ToVertexId(v));
        }

        public bool IsInEdgesEmpty(TVertex v)
        {
            EnsureVertexId(v.Id);
            return _graph.IsInEdgesEmpty(ToVertexId(v));
        }

        public int OutDegree(TVertex v)
        {
            EnsureVertexId(v.Id);
            return _graph.OutDegree(ToVertexId(v));
        }

        public IEnumerable<TEdge> OutEdges(TVertex v)
        {
            EnsureVertexId(v.Id);
            return _graph.OutEdges(ToVertexId(v)).Select(FromVertexIdEdge);
        }

        public TEdge OutEdge(TVertex v, int index)
        {
            EnsureVertexId(v.Id);
            return FromVertexIdEdge(_graph.OutEdge(ToVertexId(v), index));
        }

        public int InDegree(TVertex v)
        {
            EnsureVertexId(v.Id);
            return _graph.InDegree(ToVertexId(v));
        }

        public IEnumerable<TEdge> InEdges(TVertex v)
        {
            EnsureVertexId(v.Id);
            return FromVertexIdEdge(_graph.InEdges(ToVertexId(v)));
        }

        public TEdge InEdge(TVertex v, int index)
        {
            EnsureVertexId(v.Id);
            return FromVertexIdEdge(_graph.InEdge(ToVertexId(v), index));
        }

        public int Degree(TVertex v)
        {
            EnsureVertexId(v.Id);
            return _graph.Degree(ToVertexId(v));
        }

        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            EnsureVertexId(v.Id);
            var result = _graph.TryGetOutEdges(ToVertexId(v), out IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges);
            edges = FromVertexIdEdge(vertexIdEdges);
            return result;
        }

        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            EnsureVertexId(v.Id);
            var result = _graph.TryGetInEdges(ToVertexId(v), out IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges);
            edges = FromVertexIdEdge(vertexIdEdges);
            return result;
        }

        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            EnsureVertexId(source.Id);
            EnsureVertexId(target.Id);
            var result = _graph.TryGetEdges(ToVertexId(source), ToVertexId(target), out IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges);
            edges = FromVertexIdEdge(vertexIdEdges);
            return result;
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            EnsureVertexId(source.Id);
            EnsureVertexId(target.Id);
            var result = _graph.TryGetEdge(ToVertexId(source), ToVertexId(target), out VertexIdEdge<TVertexId, TEdgeId> vertexIdEdge);
            edge = FromVertexIdEdge(vertexIdEdge);
            return result;
        }

        public IEnumerable<TEdge> GetAllEdges(TVertexId vertexId) 
            => _graph.GetAllEdges(vertexId).Select(FromVertexIdEdge);

        public IEnumerable<TVertex> GetAdjacentVertices(TVertexId vertexId, EdgeDirection direction,
            EdgePredicate<TVertex, TEdge> edgePredicate = null, bool recursive = false)
        {
            if (!ContainsVertex(vertexId))
                return Enumerable.Empty<TVertex>();

            var vertexIds = _graph.GetAdjacentVertices(vertexId, direction,
                edge => edgePredicate == null || edgePredicate(GetEdge(edge.Id)),
                recursive);

            return vertexIds.Select(GetVertex);
        }

        public TGraph Clear()
        {
            return CreateInstance(_vertices.Clear(), _edges.Clear(), _graph.Clear());
        }

        public TGraph AddVertex(TVertex vertex)
        {
            EnsureNoVertexId(vertex.Id);

            var updatedVertices = _vertices.Add(vertex.Id, vertex);
            var updatedGraph = _graph.AddVertex(ToVertexId(vertex));
            return CreateInstance(updatedVertices, _edges, updatedGraph);
        }

        public TGraph RemoveVertex(TVertexId vertexId)
        {
            EnsureVertexId(vertexId);

            var updatedVertices = _vertices.Remove(vertexId);

            var updatedEdges = _edges;
            foreach (var edge in _edges.Values.Where(i => i.Source.Id.Equals(vertexId) || i.Target.Id.Equals(vertexId)))
                updatedEdges = updatedEdges.Remove(edge.Id);

            var updatedGraph = _graph.RemoveVertex(vertexId);
            return CreateInstance(updatedVertices, updatedEdges, updatedGraph);
        }

        public TGraph AddEdge(TEdge edge)
        {
            EnsureNoEdgeId(edge.Id);
            EnsureVertexId(edge.Source.Id);
            EnsureVertexId(edge.Target.Id);

            var updatedEdges = _edges.Add(edge.Id,edge);
            var updatedGraph = _graph.AddEdge(ToVertexIdEdge(edge));
            return CreateInstance(_vertices, updatedEdges, updatedGraph);
        }

        public TGraph RemoveEdge(TEdgeId edgeId)
        {
            EnsureEdgeId(edgeId);

            var updatedEdges = _edges.Remove(edgeId);
            var updatedGraph = _graph.RemoveEdgeIf(i => i.Id.Equals(edgeId));
            return CreateInstance(_vertices, updatedEdges, updatedGraph);
        }

        public TGraph UpdateVertex(TVertex newVertex)
        {
            EnsureVertexId(newVertex.Id);

            var updatedVertices = _vertices.SetItem(newVertex.Id, newVertex);
            var updatedEdges = ReplaceSourceAndTargetVertexInEdges(newVertex);
            return CreateInstance(updatedVertices, updatedEdges, _graph);
        }

        private ImmutableDictionary<TEdgeId, TEdge> ReplaceSourceAndTargetVertexInEdges(TVertex newVertex)
        {
            var updatedEdges = _edges;
            foreach (var edge in _edges.Values)
            {
                var updatedEdge = edge;
                if (edge.Source.Id.Equals(newVertex.Id)) updatedEdge = edge.WithSource(newVertex);
                if (edge.Target.Id.Equals(newVertex.Id)) updatedEdge = edge.WithTarget(newVertex);

                if (!ReferenceEquals(edge, updatedEdge))
                    updatedEdges = updatedEdges.SetItem(edge.Id, updatedEdge);
            }
            return updatedEdges;
        }

        public TGraph UpdateEdge(TEdge newEdge)
        {
            EnsureEdgeId(newEdge.Id);
            EnsureVertexId(newEdge.Source.Id);
            EnsureVertexId(newEdge.Target.Id);

            var updatedEdges = _edges.SetItem(newEdge.Id, newEdge);
            return CreateInstance(_vertices, updatedEdges, _graph);
        }

        protected virtual TGraph CreateInstance(
            ImmutableDictionary<TVertexId, TVertex> vertices,
            ImmutableDictionary<TEdgeId, TEdge> edges,
            ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> graph)
            => (TGraph)new UpdatableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>(vertices, edges, graph);

        private void EnsureVertexId(TVertexId id)
        {
            if (!ContainsVertex(id))
                throw new InvalidOperationException($"Graph does not contain a vertex with id: {id}");
        }

        private void EnsureNoVertexId(TVertexId id)
        {
            if (ContainsVertex(id))
                throw new InvalidOperationException($"Model already contains a vertex with id {id}");
        }

        private void EnsureEdgeId(TEdgeId id)
        {
            if (!ContainsEdge(id))
                throw new InvalidOperationException($"Graph does not contain an edge with id: {id}");
        }

        private void EnsureNoEdgeId(TEdgeId id)
        {
            if (ContainsEdge(id))
                throw new InvalidOperationException($"Model already contains a edge with id {id}");
        }

        private TVertexId ToVertexId(TVertex vertex) => vertex.Id;
        private TVertex FromVertexId(TVertexId vertexId) => _vertices[vertexId];
        private TEdge FromEdgeId(TEdgeId edgeId) => _edges[edgeId];

        private VertexIdEdge<TVertexId, TEdgeId> ToVertexIdEdge(TEdge edge) => new VertexIdEdge<TVertexId, TEdgeId>(edge.Id, edge.Source.Id, edge.Target.Id);
        private TEdge FromVertexIdEdge(VertexIdEdge<TVertexId, TEdgeId> vertexIdEdge) => GetEdge(vertexIdEdge.Id);
        private IEnumerable<TEdge> FromVertexIdEdge(IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges) => vertexIdEdges.Select(FromVertexIdEdge);
    }
}