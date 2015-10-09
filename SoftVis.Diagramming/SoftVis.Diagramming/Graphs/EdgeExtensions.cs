using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    internal static class EdgeExtensions
    {
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
