using System;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// Defines the mutator operations of an immutable bidirectional graph except replace operations.
    /// All mutators return a new instance of the graph.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices. Must be immutable.</typeparam>
    /// <typeparam name="TEdge">The type of the edges. Must be immutable.</typeparam>
    /// <typeparam name="TGraph">The implementer type. This is type returned by mutators.</typeparam>
    /// <remarks>
    /// WARNING: use only immutable types as TVertex and TEdge!
    /// </remarks>
    [Immutable]
    public interface IImmutableBidirectionalGraph<TVertex, TEdge, out TGraph> 
        where TEdge : IEdge<TVertex>
        where TGraph : IImmutableBidirectionalGraph<TVertex, TEdge, TGraph>
    {
        TGraph AddVertex(TVertex vertex);
        TGraph AddEdge(TEdge edge);
        TGraph RemoveVertex(TVertex vertex);
        TGraph RemoveVertexIf(VertexPredicate<TVertex> vertexPredicate);
        TGraph RemoveEdge(TEdge edge);
        TGraph RemoveEdgeIf(EdgePredicate<TVertex, TEdge> edgePredicate);
        TGraph Clear();
    }
}