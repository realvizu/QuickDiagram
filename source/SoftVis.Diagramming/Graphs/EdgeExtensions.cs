using System;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    internal static class EdgeExtensions
    {
        public static bool ContainsVertex<TVertex>(this IEdge<TVertex> edge, TVertex vertex)
        {
            return vertex.Equals(edge.Source) || vertex.Equals(edge.Target);
        }

        public static bool ContainsNodeOnGivenEnd<TVertex>(this IEdge<TVertex> edge, TVertex vertex, EdgeDirection direction)
        {
            switch (direction)
            {
                case EdgeDirection.Out : return vertex.Equals(edge.Source);
                case EdgeDirection.In : return vertex.Equals(edge.Target);
                default: throw new ArgumentException($"Unexpected EdgeDirection: {direction}");
            }
        }

        public static TVertex GetEndVertex<TVertex>(this IEdge<TVertex> edge, EdgeDirection edgeDirection)
        {
            return edgeDirection == EdgeDirection.In
                ? edge.Source
                : edge.Target;
        }

        public static TVertex GetOtherEnd<TVertex>(this IEdge<TVertex> edge, TVertex thisVertex)
        {
            return edge.Source.Equals(thisVertex) 
                ? edge.Target 
                : edge.Source;
        }
    }
}
