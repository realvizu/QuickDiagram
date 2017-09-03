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
    public class ReplaceableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
        : IBidirectionalGraph<TVertex, TEdge>,
        IReplaceableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
        where TVertex : IImmutableVertex<TVertexId>
        where TVertexId : IEquatable<TVertexId>, IComparable<TVertexId>
        where TEdge : IImmutableEdge<TVertex, TEdge, TEdgeId>
        where TEdgeId : IEquatable<TEdgeId>, IComparable<TEdgeId>
        where TGraph : ReplaceableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
    {
        private readonly ImmutableDictionary<TVertexId, TVertex> _vertices;
        private readonly ImmutableDictionary<TEdgeId, TEdge> _edges;
        private readonly ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> _graph;

        public ReplaceableImmutableBidirectionalGraph(bool allowParallelEdges)
            : this(ImmutableDictionary<TVertexId, TVertex>.Empty,
                  ImmutableDictionary<TEdgeId, TEdge>.Empty,
                  new ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>>(allowParallelEdges: allowParallelEdges))
        {
        }

        protected ReplaceableImmutableBidirectionalGraph(
            ImmutableDictionary<TVertexId, TVertex> vertices,
            ImmutableDictionary<TEdgeId, TEdge> edges,
            ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> graph)
        {
            _vertices = vertices;
            _edges = edges;
            _graph = graph;
        }

        public TVertex GetVertexById(TVertexId id)
        {
            EnsureVertexId(id);
            return FromVertexId(id);
        }

        public bool TryGetVertexById(TVertexId id, out TVertex vertex)
        {
            if (ContainsVertexId(id))
            {
                vertex = FromVertexId(id);
                return true;
            }

            vertex = default(TVertex);
            return false;
        }

        public TEdge GetEdgeById(TEdgeId id)
        {
            EnsureEdgeId(id);
            return FromEdgeId(id);
        }

        public bool TryGetEdgeById(TEdgeId id, out TEdge edge)
        {
            if (ContainsEdgeId(id))
            {
                edge = FromEdgeId(id);
                return true;
            }

            edge = default(TEdge);
            return false;
        }

        public IEnumerable<TVertex> Vertices => _vertices.Values;
        public bool ContainsVertex(TVertex vertex) => _vertices.ContainsValue(vertex);
        public bool ContainsVertexId(TVertexId id) => _vertices.ContainsKey(id);
        public int VertexCount => _vertices.Count;
        public bool IsVerticesEmpty => !_vertices.Any();

        public IEnumerable<TEdge> Edges => _edges.Values;
        public bool ContainsEdge(TEdge edge) => _edges.ContainsValue(edge);
        public bool ContainsEdgeId(TEdgeId id) => _edges.ContainsKey(id);
        public int EdgeCount => _edges.Count;
        public bool IsEdgesEmpty => !_edges.Any();

        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;
        public bool IsOutEdgesEmpty(TVertex v) => _graph.IsOutEdgesEmpty(ToVertexId(v));
        public bool IsInEdgesEmpty(TVertex v) => _graph.IsInEdgesEmpty(ToVertexId(v));
        public bool ContainsEdge(TVertex source, TVertex target) => _graph.ContainsEdge(ToVertexId(source), ToVertexId(target));

        public int OutDegree(TVertex v)
        {
            EnsureVertex(v);
            return _graph.OutDegree(ToVertexId(v));
        }

        public IEnumerable<TEdge> OutEdges(TVertex v)
        {
            EnsureVertex(v);
            return _graph.OutEdges(ToVertexId(v)).Select(FromVertexIdEdge);
        }

        public TEdge OutEdge(TVertex v, int index)
        {
            EnsureVertex(v);
            return FromVertexIdEdge(_graph.OutEdge(ToVertexId(v), index));
        }

        public int InDegree(TVertex v)
        {
            EnsureVertex(v);
            return _graph.InDegree(ToVertexId(v));
        }

        public IEnumerable<TEdge> InEdges(TVertex v)
        {
            EnsureVertex(v);
            return FromVertexIdEdge(_graph.InEdges(ToVertexId(v)));
        }

        public TEdge InEdge(TVertex v, int index)
        {
            EnsureVertex(v);
            return FromVertexIdEdge(_graph.InEdge(ToVertexId(v), index));
        }

        public int Degree(TVertex v)
        {
            EnsureVertex(v);
            return _graph.Degree(ToVertexId(v));
        }

        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            EnsureVertex(v);
            var result = _graph.TryGetOutEdges(ToVertexId(v), out IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges);
            edges = FromVertexIdEdge(vertexIdEdges);
            return result;
        }

        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            EnsureVertex(v);
            var result = _graph.TryGetInEdges(ToVertexId(v), out IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges);
            edges = FromVertexIdEdge(vertexIdEdges);
            return result;
        }

        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            EnsureVertex(source);
            EnsureVertex(target);
            var result = _graph.TryGetEdges(ToVertexId(source), ToVertexId(target), out IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges);
            edges = FromVertexIdEdge(vertexIdEdges);
            return result;
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            EnsureVertex(source);
            EnsureVertex(target);
            var result = _graph.TryGetEdge(ToVertexId(source), ToVertexId(target), out VertexIdEdge<TVertexId, TEdgeId> vertexIdEdge);
            edge = FromVertexIdEdge(vertexIdEdge);
            return result;
        }

        public bool PathExistsById(TVertexId sourceId, TVertexId targetId) => _graph.PathExists(sourceId, targetId);

        public IEnumerable<TVertex> GetConnectedVerticesById(TVertex vertex,
            Func<TVertex, TEdge, bool> edgePredicate, bool recursive = false)
        {
            if (!ContainsVertexId(vertex.Id))
                return Enumerable.Empty<TVertex>();

            var vertexIds = _graph.GetConnectedVertices(vertex.Id,
                (id, edge) => edgePredicate(GetVertexById(id), GetEdgeById(edge.Id)),
                recursive);

            return vertexIds.Select(GetVertexById);
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

        public TGraph AddVertexRange(IEnumerable<TVertex> vertices)
        {
            var result = default(TGraph);

            foreach (var vertex in vertices)
                result = AddVertex(vertex);

            return result;
        }

        public TGraph RemoveVertex(TVertex vertex)
        {
            EnsureVertexId(vertex.Id);

            var updatedVertices = _vertices.Remove(vertex.Id);

            var updatedEdges = _edges;
            foreach (var edge in _edges.Values.Where(i => i.Source.Id.Equals(vertex.Id) || i.Target.Id.Equals(vertex.Id)))
                updatedEdges = updatedEdges.Remove(edge.Id);

            var updatedGraph = _graph.RemoveVertex(ToVertexId(vertex));
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

        public TGraph AddEdgeRange(IEnumerable<TEdge> edges)
        {
            var result = default(TGraph);

            foreach (var edge in edges)
                result = AddEdge(edge);

            return result;
        }

        public TGraph RemoveEdge(TEdge edge)
        {
            EnsureEdgeId(edge.Id);

            var updatedEdges = _edges.Remove(edge.Id);
            var updatedGraph = _graph.RemoveEdge(ToVertexIdEdge(edge));
            return CreateInstance(_vertices, updatedEdges, updatedGraph);
        }

        public TGraph RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
            => throw new NotImplementedException();

        public TGraph ClearOutEdges(TVertex v)
            => throw new NotImplementedException();

        public TGraph TrimEdgeExcess()
            => throw new NotImplementedException();

        public TGraph RemoveVertexIf(VertexPredicate<TVertex> pred)
            => throw new NotImplementedException();

        public TGraph RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
            => throw new NotImplementedException();

        public TGraph AddVerticesAndEdge(TEdge edge)
            => throw new NotImplementedException();

        public TGraph AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
            => throw new NotImplementedException();

        public TGraph RemoveInEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> edgePredicate)
            => throw new NotImplementedException();

        public TGraph ClearInEdges(TVertex v)
            => throw new NotImplementedException();

        public TGraph ClearEdges(TVertex v)
            => throw new NotImplementedException();

        public TGraph ReplaceVertex(TVertex oldVertex, TVertex newVertex)
        {
            EnsureVertexId(oldVertex.Id);
            EnsureSameVertexId(oldVertex, newVertex);

            var updatedVertices = _vertices.SetItem(oldVertex.Id, newVertex);
            var updatedEdges = ReplaceSourceAndTargetVertexInEdges(oldVertex, newVertex);
            return CreateInstance(updatedVertices, updatedEdges, _graph);
        }

        private ImmutableDictionary<TEdgeId, TEdge> ReplaceSourceAndTargetVertexInEdges(TVertex oldVertex, TVertex newVertex)
        {
            var updatedEdges = _edges;
            foreach (var edge in _edges.Values)
            {
                var updatedEdge = edge;
                if (edge.Source.Id.Equals(oldVertex.Id)) updatedEdge = edge.WithSource(newVertex);
                if (edge.Target.Id.Equals(oldVertex.Id)) updatedEdge = edge.WithTarget(newVertex);

                if (!updatedEdge.Equals(edge))
                    updatedEdges = updatedEdges.SetItem(edge.Id, updatedEdge);
            }
            return updatedEdges;
        }

        public TGraph ReplaceEdge(TEdge oldEdge, TEdge newEdge)
        {
            EnsureEdgeId(oldEdge.Id);
            EnsureSameEdgeId(oldEdge, newEdge);
            EnsureVertexId(newEdge.Source.Id);
            EnsureVertexId(newEdge.Target.Id);

            var updatedEdges = _edges.SetItem(oldEdge.Id, newEdge);
            return CreateInstance(_vertices, updatedEdges, _graph);
        }

        protected virtual TGraph CreateInstance(
            ImmutableDictionary<TVertexId, TVertex> vertices,
            ImmutableDictionary<TEdgeId, TEdge> edges,
            ImmutableBidirectionalGraph<TVertexId, VertexIdEdge<TVertexId, TEdgeId>> graph)
            => (TGraph)new ReplaceableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>(vertices, edges, graph);

        private void EnsureVertexId(TVertexId id)
        {
            if (!ContainsVertexId(id))
                throw new InvalidOperationException($"Graph does not contain a vertex with id: {id}");
        }

        private void EnsureVertex(TVertex vertex)
        {
            EnsureVertexId(vertex.Id);
            if (!FromVertexId(vertex.Id).Equals(vertex))
                throw new InvalidOperationException($"Graph contains a different vertex with id: {vertex.Id}");
        }

        private void EnsureNoVertexId(TVertexId id)
        {
            if (ContainsVertexId(id))
                throw new InvalidOperationException($"Model already contains a vertex with id {id}");
        }

        private static void EnsureSameVertexId(TVertex oldVertex, TVertex newVertex)
        {
            if (!oldVertex.Id.Equals(newVertex.Id))
                throw new InvalidOperationException($"Old vertex with id {oldVertex.Id} does not match new vertex with id {newVertex.Id}");
        }

        private void EnsureEdgeId(TEdgeId id)
        {
            if (!ContainsEdgeId(id))
                throw new InvalidOperationException($"Graph does not contain an edge with id: {id}");
        }

        private void EnsureEdge(TEdge edge)
        {
            EnsureEdgeId(edge.Id);
            if (!FromEdgeId(edge.Id).Equals(edge))
                throw new InvalidOperationException($"Graph contains a different edge with id: {edge.Id}");
        }

        private void EnsureNoEdgeId(TEdgeId id)
        {
            if (ContainsEdgeId(id))
                throw new InvalidOperationException($"Model already contains a edge with id {id}");
        }

        private static void EnsureSameEdgeId(TEdge oldEdge, TEdge newEdge)
        {
            if (!oldEdge.Id.Equals(newEdge.Id))
                throw new InvalidOperationException($"Old edge with id {oldEdge.Id} does not match new edge with id {newEdge.Id}");
        }

        private TVertexId ToVertexId(TVertex vertex) => vertex.Id;
        private TVertex FromVertexId(TVertexId vertexId) => _vertices[vertexId];

        private TEdge FromEdgeId(TEdgeId edgeId) => _edges[edgeId];

        private VertexIdEdge<TVertexId, TEdgeId> ToVertexIdEdge(TEdge edge) => new VertexIdEdge<TVertexId, TEdgeId>(edge.Id, edge.Source.Id, edge.Target.Id);
        private TEdge FromVertexIdEdge(VertexIdEdge<TVertexId, TEdgeId> vertexIdEdge) => GetEdgeById(vertexIdEdge.Id);
        private IEnumerable<TEdge> FromVertexIdEdge(IEnumerable<VertexIdEdge<TVertexId, TEdgeId>> vertexIdEdges) => vertexIdEdges.Select(FromVertexIdEdge);
    }
}