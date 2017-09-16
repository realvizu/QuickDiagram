using System;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An immutable bidirectional graph where the vertices/edges can be replaced
    /// with other versions of the same vertex/edge (meaning their IDs must be the same).
    /// All mutators return a new instance of the graph.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices. Must be immutable.</typeparam>
    /// <typeparam name="TVertexId">The type of the vertex id.</typeparam>
    /// <typeparam name="TEdge">The type of the edges. Must be immutable.</typeparam>
    /// <typeparam name="TEdgeId">The type of the edge id.</typeparam>
    /// <typeparam name="TGraph">The implementer type. This is type returned by mutators.</typeparam>
    /// <remarks>
    /// WARNING: use only immutable types as TVertex and TEdge!
    /// TVertex and TEdge must have a unique, stable ID that identifies instances across versions.
    /// </remarks>
    public interface IReplaceableImmutableBidirectionalGraph<TVertex, in TVertexId, TEdge, in TEdgeId, out TGraph>
        : IImmutableBidirectionalGraph<TVertex, TEdge, TGraph>
        where TVertex : IReplaceableImmutableVertex<TVertexId>
        where TVertexId : IEquatable<TVertexId>
        where TEdge : IReplaceableImmutableEdge<TVertex, TEdgeId, TEdge>
        where TEdgeId : IEquatable<TEdgeId>
        where TGraph : IReplaceableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
    {
        bool ContainsVertexId(TVertexId id);
        TVertex GetVertexById(TVertexId id);
        bool TryGetVertexById(TVertexId id, out TVertex vertex);

        bool ContainsEdgeId(TEdgeId id);
        TEdge GetEdgeById(TEdgeId id);
        bool TryGetEdgeById(TEdgeId id, out TEdge edge);

        bool PathExistsById(TVertexId sourceId, TVertexId targetId);

        /// <summary>
        /// Replaces a vertex with a new version of the same vertex.
        /// </summary>
        /// <param name="oldVertex">The vertex to be replaced.</param>
        /// <param name="newVertex">The new vertex.</param>
        /// <returns>The mutated graph.</returns>
        /// <remarks>
        /// A ID of the old and the new vertex must be the same.
        /// </remarks>
        TGraph ReplaceVertex(TVertex oldVertex, TVertex newVertex);

        /// <summary>
        /// Replaces an edge with a new version of the same edge.
        /// </summary>
        /// <param name="oldEdge">The edge to be replaced.</param>
        /// <param name="newEdge">The new edge.</param>
        /// <returns>The mutated graph.</returns>
        /// <remarks>
        /// A ID of the old and the new edge must be the same.
        /// </remarks>
        TGraph ReplaceEdge(TEdge oldEdge, TEdge newEdge);
    }
}
