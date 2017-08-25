using System;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An immutable graph edge.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edge implementing type.</typeparam>
    /// <typeparam name="TEdgeId">The type of the edge id. Must be equatable.</typeparam>
    public interface IImmutableEdge<TVertex, out TEdge, out TEdgeId> : IEdge<TVertex>
        where TEdgeId : IEquatable<TEdgeId>
    {
        TEdgeId Id { get; }

        TEdge WithSource(TVertex vertex);
        TEdge WithTarget(TVertex vertex);
    }
}
