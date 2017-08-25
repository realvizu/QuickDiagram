using System;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// A vertex with a unique identifier that can be used to correlate different versions of an immutable vertex.
    /// </summary>
    /// <remarks>
    /// The Id must be kept whenever a new version is created by mutating a previous one.
    /// </remarks>
    public interface IImmutableVertex<out TVertexId>
        where TVertexId : IEquatable<TVertexId>
    {
        TVertexId Id { get; }
    }
}
