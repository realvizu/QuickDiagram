using System;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An immutable edge with a unique identifier that can be used to correlate updated versions of the same edge.
    /// </summary>
    /// <typeparam name="TVertexId">The type of the vertex id. Must be equatable.</typeparam>
    /// <typeparam name="TEdgeId">The type of the edge id. Must be equatable.</typeparam>
    /// <typeparam name="TEdge">The implementing type. It will be returned by mutators.</typeparam>
    public interface IImmutableEdge<TVertexId, out TEdge, out TEdgeId> : IEdge<TVertexId>
        where TVertexId : IEquatable<TVertexId>
        where TEdge : IImmutableEdge<TVertexId, TEdge, TEdgeId>
        where TEdgeId : IEquatable<TEdgeId>
    {
        /// <summary>
        /// Identifies the edge through updated versions. Must be kept by mutators.
        /// </summary>
        TEdgeId Id { get; }
    }
}
