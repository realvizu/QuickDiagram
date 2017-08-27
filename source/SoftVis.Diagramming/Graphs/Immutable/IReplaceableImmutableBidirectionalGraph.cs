using System;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// Defines replace operations for immutable bidirectional graphs.
    /// Vertices and edges must have a unique, stable ID that identifies them across versions.
    /// All mutators return a new instance of the graph.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices. Must be immutable.</typeparam>
    /// <typeparam name="TVertexId">The type of the vertex id.</typeparam>
    /// <typeparam name="TEdge">The type of the edges. Must be immutable.</typeparam>
    /// <typeparam name="TEdgeId">The type of the edge id.</typeparam>
    /// <typeparam name="TGraph">The implementer type. This is type returned by mutators.</typeparam>
    /// <remarks>
    /// Replace operations require that vertices and edges have a stable identifier that doesn't change when mutators create new nodes and edges.
    /// WARNING: use only immutable types as TVertex and TEdge!
    /// </remarks>
    public interface IReplaceableImmutableBidirectionalGraph<TVertex, in TVertexId, TEdge, in TEdgeId, out TGraph>
        : IImmutableBidirectionalGraph<TVertex, TEdge, TGraph>
        where TVertex : IImmutableVertex<TVertexId>
        where TVertexId : IEquatable<TVertexId>
        where TEdge : IImmutableEdge<TVertex, TEdge, TEdgeId>
        where TEdgeId : IEquatable<TEdgeId>
        where TGraph : IReplaceableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
    {
        bool ContainsVertexId(TVertexId id);
        bool ContainsEdgeId(TEdgeId id);

        TVertex GetVertexById(TVertexId id);
        bool TryGetVertexById(TVertexId id, out TVertex vertex);
        TEdge GetEdgeById(TEdgeId id);
        bool TryGetEdgeById(TEdgeId id, out TEdge edge);

        bool PathExistsById(TVertexId sourceId, TVertexId targetId);

        TGraph ReplaceVertex(TVertex oldVertex, TVertex newVertex);
        TGraph ReplaceEdge(TEdge oldEdge, TEdge newEdge);
    }
}
