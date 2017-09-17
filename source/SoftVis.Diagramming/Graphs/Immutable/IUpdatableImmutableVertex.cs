using System;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An immutable vertex with a unique identifier that can be used to correlate updated versions of the same vertex.
    /// </summary>
    public interface IUpdatableImmutableVertex<out TVertexId>
        where TVertexId : IEquatable<TVertexId>
    {
        /// <summary>
        /// Identifies the vertex through updated versions. Must be kept by mutators.
        /// </summary>
        TVertexId Id { get; }
    }
}
