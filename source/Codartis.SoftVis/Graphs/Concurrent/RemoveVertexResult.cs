using System;
using System.Collections.Generic;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Concurrent
{
    /// <summary>
    /// Describes the result of a RemoveVertex operations. Contains the list of edges that were removed together with the vertex.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    public struct RemoveVertexResult<TVertex, TEdge> : IEquatable<RemoveVertexResult<TVertex, TEdge>> where TEdge: IEdge<TVertex>
    {
        public TVertex RemovedVertex { get; }
        public IReadOnlyList<TEdge> RemovedEdges { get; }

        public static RemoveVertexResult<TVertex, TEdge> Empty = new RemoveVertexResult<TVertex, TEdge>();

        public RemoveVertexResult(TVertex removedVertex = default, IReadOnlyList<TEdge> removedEdges = null)
        {
            RemovedVertex = removedVertex;
            RemovedEdges = removedEdges;
        }

        public bool Equals(RemoveVertexResult<TVertex, TEdge> other)
        {
            return EqualityComparer<TVertex>.Default.Equals(RemovedVertex, other.RemovedVertex) 
                && Equals(RemovedEdges, other.RemovedEdges);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RemoveVertexResult<TVertex, TEdge> && Equals((RemoveVertexResult<TVertex, TEdge>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TVertex>.Default.GetHashCode(RemovedVertex)*397) ^ (RemovedEdges != null ? RemovedEdges.GetHashCode() : 0);
            }
        }

        public static bool operator ==(RemoveVertexResult<TVertex, TEdge> left, RemoveVertexResult<TVertex, TEdge> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RemoveVertexResult<TVertex, TEdge> left, RemoveVertexResult<TVertex, TEdge> right)
        {
            return !left.Equals(right);
        }
    }
}
