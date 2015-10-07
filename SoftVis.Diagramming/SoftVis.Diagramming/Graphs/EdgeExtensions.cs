using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    internal static class EdgeExtensions
    {
        public static TVertex OtherVertex<TVertex>(this IEdge<TVertex> edge, TVertex thisVertex)
        {
            return edge.Source.Equals(thisVertex) ? edge.Target : edge.Source;
        }
    }
}
