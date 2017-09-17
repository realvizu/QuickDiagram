using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An immutable bidirectional graph where the vertices/edges can be updated,
    /// that is, replaced with another version having the same ID.
    /// All mutators return a new instance of the graph.
    /// All selectors treat vertex/edge equality as ID equality.
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
    public interface IUpdatableImmutableBidirectionalGraph<TVertex, in TVertexId, TEdge, in TEdgeId, out TGraph>
        where TVertex : IUpdatableImmutableVertex<TVertexId>
        where TVertexId : IEquatable<TVertexId>
        where TEdge : IUpdatableImmutableEdge<TVertex, TVertexId, TEdge, TEdgeId>
        where TEdgeId : IEquatable<TEdgeId>
        where TGraph : IUpdatableImmutableBidirectionalGraph<TVertex, TVertexId, TEdge, TEdgeId, TGraph>
    {
        bool ContainsVertex(TVertexId vertexId);
        TVertex GetVertex(TVertexId vertexId);
        bool TryGetVertex(TVertexId vertexId, out TVertex vertex);

        bool ContainsEdge(TEdgeId edgeId);
        TEdge GetEdge(TEdgeId edgeId);
        bool TryGetEdge(TEdgeId edgeId, out TEdge edge);

        IEnumerable<TEdge> GetAllEdges(TVertexId vertexId);
        IEnumerable<TVertex> GetConnectedVertices(TVertexId vertexId, Func<TVertex, TEdge, bool> edgePredicate, bool recursive = false);
        bool PathExists(TVertexId sourceVertexId, TVertexId targetVertexId);

        TGraph AddVertex(TVertex vertex);
        TGraph AddEdge(TEdge edge);
        TGraph RemoveVertex(TVertexId vertexId);
        TGraph RemoveEdge(TEdgeId edgeId);
        TGraph Clear();

        /// <summary>
        /// Replaces the vertex with the same ID with a new version.
        /// </summary>
        /// <param name="newVertex">The new vertex version.</param>
        /// <returns>The mutated graph.</returns>
        TGraph UpdateVertex(TVertex newVertex);

        /// <summary>
        /// Replaces the edge with the same ID with a new version.
        /// </summary>
        /// <param name="newEdge">The new edge version.</param>
        /// <returns>The mutated graph.</returns>
        TGraph UpdateEdge(TEdge newEdge);
    }
}
