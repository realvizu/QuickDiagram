using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    /// <summary>
    /// An immutable graph. Mutators return a modified copy of the original graph.
    /// </summary>
    public class ImmutableBidirectionalGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// We store the vertices and edges in wrapper object to make it possible to replace any of them without rewiring the graph.
        /// </summary>
        private readonly BidirectionalGraph<VertexWrapper, EdgeWrapper> _graph;

        private ImmutableBidirectionalGraph(BidirectionalGraph<VertexWrapper, EdgeWrapper> graph)
        {
            _graph = graph;
        }

        public ImmutableBidirectionalGraph(bool allowParallelEdges)
            : this(new BidirectionalGraph<VertexWrapper, EdgeWrapper>(allowParallelEdges))
        {
        }

        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;
        public bool ContainsVertex(TVertex vertex) => _graph.ContainsVertex(Wrap(vertex));
        public bool IsOutEdgesEmpty(TVertex v) => _graph.IsOutEdgesEmpty(Wrap(v));
        public int OutDegree(TVertex v) => _graph.OutDegree(Wrap(v));
        public IEnumerable<TEdge> OutEdges(TVertex v) => _graph.OutEdges(Wrap(v)).Select(Unwrap);
        public TEdge OutEdge(TVertex v, int index) => Unwrap(_graph.OutEdge(Wrap(v), index));
        public bool ContainsEdge(TVertex source, TVertex target) => _graph.ContainsEdge(Wrap(source), Wrap(target));
        public bool IsVerticesEmpty => _graph.IsVerticesEmpty;
        public int VertexCount => _graph.VertexCount;
        public IEnumerable<TVertex> Vertices => _graph.Vertices.Select(Unwrap);
        public bool ContainsEdge(TEdge edge) => _graph.ContainsEdge(Wrap(edge));
        public bool IsEdgesEmpty => _graph.IsEdgesEmpty;
        public int EdgeCount => _graph.EdgeCount;
        public IEnumerable<TEdge> Edges => _graph.Edges.Select(Unwrap);
        public bool IsInEdgesEmpty(TVertex v) => _graph.IsInEdgesEmpty(Wrap(v));
        public int InDegree(TVertex v) => _graph.InDegree(Wrap(v));
        public IEnumerable<TEdge> InEdges(TVertex v) => _graph.InEdges(Wrap(v)).Select(Unwrap);
        public TEdge InEdge(TVertex v, int index) => Unwrap(_graph.InEdge(Wrap(v), index));
        public int Degree(TVertex v) => _graph.Degree(Wrap(v));

        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            var result = _graph.TryGetOutEdges(Wrap(v), out IEnumerable<EdgeWrapper> edgeWrappers);
            edges = edgeWrappers.Select(Unwrap);
            return result;
        }

        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            var result = _graph.TryGetEdges(Wrap(source), Wrap(target), out IEnumerable<EdgeWrapper> edgeWrappers);
            edges = edgeWrappers.Select(Unwrap);
            return result;
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            var result = _graph.TryGetEdge(Wrap(source), Wrap(target), out EdgeWrapper edgeWrapper);
            edge = Unwrap(edgeWrapper);
            return result;
        }

        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            var result = _graph.TryGetInEdges(Wrap(v), out IEnumerable<EdgeWrapper> edgeWrappers);
            edges = edgeWrappers.Select(Unwrap);
            return result;
        }

        public ImmutableBidirectionalGraph<TVertex, TEdge> Clear() 
            => new ImmutableBidirectionalGraph<TVertex, TEdge>(_graph.AllowParallelEdges);

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVertex(TVertex v) 
            => CloneAndMutate(i => i.AddVertex(Wrap(v)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVertexRange(IEnumerable<TVertex> vertices) 
            => CloneAndMutate(i => i.AddVertexRange(vertices.Select(Wrap)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
            => CloneAndMutate(i => i.RemoveOutEdgeIf(Wrap(v), e => predicate.Invoke(Unwrap(e))));

        public ImmutableBidirectionalGraph<TVertex, TEdge> ClearOutEdges(TVertex v)
            => CloneAndMutate(i => i.ClearOutEdges(Wrap(v)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> TrimEdgeExcess()
            => CloneAndMutate(i => i.TrimEdgeExcess());

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveVertex(TVertex v)
            => CloneAndMutate(i => i.RemoveVertex(Wrap(v)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveVertexIf(VertexPredicate<TVertex> pred)
            => CloneAndMutate(i => i.RemoveVertexIf(e => pred.Invoke(Unwrap(e))));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddEdge(TEdge edge)
            => CloneAndMutate(i => i.AddEdge(Wrap(edge)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddEdgeRange(IEnumerable<TEdge> edges)
            => CloneAndMutate(i => i.AddEdgeRange(edges.Select(Wrap)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveEdge(TEdge edge)
            => CloneAndMutate(i => i.RemoveEdge(Wrap(edge)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
            => CloneAndMutate(i => i.RemoveEdgeIf(e => predicate.Invoke(Unwrap(e))));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVerticesAndEdge(TEdge edge)
            => CloneAndMutate(i => i.AddVerticesAndEdge(Wrap(edge)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
            => CloneAndMutate(i => i.AddVerticesAndEdgeRange(edges.Select(Wrap)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveInEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> edgePredicate)
            => CloneAndMutate(i => i.RemoveInEdgeIf(Wrap(v), e => edgePredicate.Invoke(Unwrap(e))));

        public ImmutableBidirectionalGraph<TVertex, TEdge> ClearInEdges(TVertex v)
            => CloneAndMutate(i => i.ClearInEdges(Wrap(v)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> ClearEdges(TVertex v)
            => CloneAndMutate(i => i.ClearEdges(Wrap(v)));

        public ImmutableBidirectionalGraph<TVertex, TEdge> ReplaceVertex(TVertex oldVertex, TVertex newVertex)
        {
            var graph = _graph.Clone();

            foreach (var vertexWrapper in graph.Vertices)
                ReplaceVertexInWrapper(vertexWrapper, oldVertex, newVertex);

            foreach (var edgeWrapper in graph.Edges)
            {
                ReplaceVertexInWrapper(edgeWrapper.Source, oldVertex, newVertex);
                ReplaceVertexInWrapper(edgeWrapper.Target, oldVertex, newVertex);
            }

            return new ImmutableBidirectionalGraph<TVertex, TEdge>(graph);
        }

        private static void ReplaceVertexInWrapper(VertexWrapper vertexWrapper, TVertex oldVertex, TVertex newVertex)
        {
            if (Equals(vertexWrapper.Vertex, oldVertex))
                vertexWrapper.Vertex = newVertex;
        }

        private ImmutableBidirectionalGraph<TVertex, TEdge> CloneAndMutate(Action<BidirectionalGraph<VertexWrapper, EdgeWrapper>> mutatorAction)
        {
            var graph = _graph.Clone();
            mutatorAction.Invoke(graph);
            return new ImmutableBidirectionalGraph<TVertex, TEdge>(graph);
        }

        private static VertexWrapper Wrap(TVertex vertex) => new VertexWrapper(vertex);
        private static TVertex Unwrap(VertexWrapper i) => i.Vertex;

        private static EdgeWrapper Wrap(TEdge edge) => new EdgeWrapper(Wrap(edge.Source), Wrap(edge.Target), edge);
        private static TEdge Unwrap(EdgeWrapper i) => i.Edge;

        private class VertexWrapper
        {
            public TVertex Vertex { get; set; }

            public VertexWrapper(TVertex vertex)
            {
                Vertex = vertex;
            }
        }

        private class EdgeWrapper : IEdge<VertexWrapper>
        {
            public VertexWrapper Source { get; }
            public VertexWrapper Target { get; }
            public TEdge Edge { get; }

            public EdgeWrapper(VertexWrapper source, VertexWrapper target, TEdge edge)
            {
                Source = source;
                Target = target;
                Edge = edge;
            }
        }
    }
}
