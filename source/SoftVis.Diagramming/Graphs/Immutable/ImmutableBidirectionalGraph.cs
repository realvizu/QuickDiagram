using System;
using System.Collections.Generic;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An immutable graph. Mutators return a modified copy of the original graph.
    /// Does not implement vertex/edge replace operations.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices. Must be immutable.</typeparam>
    /// <typeparam name="TEdge">The type of the edges. Must be immutable.</typeparam>
    /// <remarks>
    /// WARNING: Descendants must override the method that creates a new object of the descendant type.
    /// </remarks>
    public class ImmutableBidirectionalGraph<TVertex, TEdge> :
        IBidirectionalGraph<TVertex, TEdge>,
        IImmutableBidirectionalGraph<TVertex, TEdge, ImmutableBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private readonly BidirectionalGraph<TVertex, TEdge> _graph;

        public ImmutableBidirectionalGraph(bool allowParallelEdges)
            : this(new BidirectionalGraph<TVertex, TEdge>(allowParallelEdges))
        {
        }

        protected ImmutableBidirectionalGraph(BidirectionalGraph<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;
        public bool ContainsVertex(TVertex vertex) => _graph.ContainsVertex(vertex);
        public bool IsOutEdgesEmpty(TVertex v) => _graph.IsOutEdgesEmpty(v);
        public int OutDegree(TVertex v) => _graph.OutDegree(v);
        public IEnumerable<TEdge> OutEdges(TVertex v) => _graph.OutEdges(v);
        public TEdge OutEdge(TVertex v, int index) => _graph.OutEdge(v, index);
        public bool ContainsEdge(TVertex source, TVertex target) => _graph.ContainsEdge(source, target);
        public bool IsVerticesEmpty => _graph.IsVerticesEmpty;
        public int VertexCount => _graph.VertexCount;
        public IEnumerable<TVertex> Vertices => _graph.Vertices;
        public bool ContainsEdge(TEdge edge) => _graph.ContainsEdge(edge);
        public bool IsEdgesEmpty => _graph.IsEdgesEmpty;
        public int EdgeCount => _graph.EdgeCount;
        public IEnumerable<TEdge> Edges => _graph.Edges;
        public bool IsInEdgesEmpty(TVertex v) => _graph.IsInEdgesEmpty(v);
        public int InDegree(TVertex v) => _graph.InDegree(v);
        public IEnumerable<TEdge> InEdges(TVertex v) => _graph.InEdges(v);
        public TEdge InEdge(TVertex v, int index) => _graph.InEdge(v, index);
        public int Degree(TVertex v) => _graph.Degree(v);
        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges) => _graph.TryGetOutEdges(v, out edges);
        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges) => _graph.TryGetInEdges(v, out edges);
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges) => _graph.TryGetEdges(source, target, out edges);
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge) => _graph.TryGetEdge(source, target, out edge);

        public ImmutableBidirectionalGraph<TVertex, TEdge> Clear()
            => CloneAndMutate(i => i.Clear());

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVertex(TVertex v)
            => CloneAndMutate(i => i.AddVertex(v));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVertexRange(IEnumerable<TVertex> vertices)
            => CloneAndMutate(i => i.AddVertexRange(vertices));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
            => CloneAndMutate(i => i.RemoveOutEdgeIf(v, predicate.Invoke));

        public ImmutableBidirectionalGraph<TVertex, TEdge> ClearOutEdges(TVertex v)
            => CloneAndMutate(i => i.ClearOutEdges(v));

        public ImmutableBidirectionalGraph<TVertex, TEdge> TrimEdgeExcess()
            => CloneAndMutate(i => i.TrimEdgeExcess());

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveVertex(TVertex v)
            => CloneAndMutate(i => i.RemoveVertex(v));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveVertexIf(VertexPredicate<TVertex> pred)
            => CloneAndMutate(i => i.RemoveVertexIf(e => pred.Invoke((e))));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddEdge(TEdge edge)
            => CloneAndMutate(i => i.AddEdge(edge));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddEdgeRange(IEnumerable<TEdge> edges)
            => CloneAndMutate(i => i.AddEdgeRange(edges));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveEdge(TEdge edge)
            => CloneAndMutate(i => i.RemoveEdge(edge));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
            => CloneAndMutate(i => i.RemoveEdgeIf(predicate.Invoke));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVerticesAndEdge(TEdge edge)
            => CloneAndMutate(i => i.AddVerticesAndEdge(edge));

        public ImmutableBidirectionalGraph<TVertex, TEdge> AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
            => CloneAndMutate(i => i.AddVerticesAndEdgeRange(edges));

        public ImmutableBidirectionalGraph<TVertex, TEdge> RemoveInEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> edgePredicate)
            => CloneAndMutate(i => i.RemoveInEdgeIf(v, edgePredicate.Invoke));

        public ImmutableBidirectionalGraph<TVertex, TEdge> ClearInEdges(TVertex v)
            => CloneAndMutate(i => i.ClearInEdges(v));

        public ImmutableBidirectionalGraph<TVertex, TEdge> ClearEdges(TVertex v)
            => CloneAndMutate(i => i.ClearEdges(v));

        protected virtual ImmutableBidirectionalGraph<TVertex, TEdge> CreateInstance(BidirectionalGraph<TVertex, TEdge> graph)
            => new ImmutableBidirectionalGraph<TVertex, TEdge>(graph);

        private ImmutableBidirectionalGraph<TVertex, TEdge> CloneAndMutate(Action<BidirectionalGraph<TVertex, TEdge>> mutatorAction)
        {
            var graph = _graph.Clone();
            mutatorAction.Invoke(graph);
            return CreateInstance(graph);
        }
    }
}
