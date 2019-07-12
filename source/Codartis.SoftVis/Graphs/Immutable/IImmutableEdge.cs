using System;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An immutable edge with a unique identifier that can be used to correlate updated versions of the same edge.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices that the edge connects.</typeparam>
    /// <typeparam name="TVertexId">The type of the vertex id. Must be equatable.</typeparam>
    /// <typeparam name="TEdgeId">The type of the edge id. Must be equatable.</typeparam>
    /// <typeparam name="TEdge">The implementing type. It will be returned by mutators.</typeparam>
    public interface IImmutableEdge<TVertex, TVertexId, out TEdge, out TEdgeId> : IEdge<TVertex>
        where TVertex : IImmutableVertex<TVertexId>
        where TVertexId : IEquatable<TVertexId>
        where TEdge : IImmutableEdge<TVertex, TVertexId, TEdge, TEdgeId>
        where TEdgeId : IEquatable<TEdgeId>
    {
        /// <summary>
        /// Identifies the edge through updated versions. Must be kept by mutators.
        /// </summary>
        TEdgeId Id { get; }

        /// <summary>
        /// Creates a new version of the edge by replacing the source vertex with an updated version.
        /// </summary>
        /// <param name="vertex">The new vertex.</param>
        /// <returns>A new version of the edge with the source vertex replaced.</returns>
        /// <remarks>
        /// A ID of the old and the new vertex must be the same.
        /// </remarks>
        TEdge WithSource(TVertex vertex);

        /// <summary>
        /// Creates a new version of the edge by replacing the target vertex with a new version.
        /// </summary>
        /// <param name="vertex">The new vertex.</param>
        /// <returns>A new version of the edge with the target vertex replaced.</returns>
        /// <remarks>
        /// A ID of the old and the new vertex must be the same.
        /// </remarks>
        TEdge WithTarget(TVertex vertex);
    }
}
