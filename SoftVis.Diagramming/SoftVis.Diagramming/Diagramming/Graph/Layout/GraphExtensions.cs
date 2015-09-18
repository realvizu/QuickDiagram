using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout
{
    public static class GraphExtensions
    {
        public static IEnumerable<TVertex> GetVerticesWithNoOutEdges<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, IEnumerable<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
            return vertices.Where(graph.IsOutEdgesEmpty);
        }

        public static IEnumerable<TVertex> GetVerticesWithNoInEdges<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, IEnumerable<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
            return vertices.Where(graph.IsInEdgesEmpty);
        }

        public static IEnumerable<TVertex> GetOutNeighbours<TVertex, TEdge>(
            this IVertexAndEdgeListGraph<TVertex, TEdge> graph, TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.OutEdges(vertex).Select(e => e.Target).Distinct();
        }

        public static IEnumerable<TVertex> GetInNeighbours<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.InEdges(vertex).Select(e => e.Source).Distinct();
        }

        public static TVertex OtherVertex<TVertex>(this IEdge<TVertex> edge, TVertex thisVertex)
        {
            return edge.Source.Equals(thisVertex) ? edge.Target : edge.Source;
        }

        public static BidirectionalGraph<TVertex, TEdge> CopyToBidirectionalGraph<TVertex, TEdge>(
            this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var newGraph = new BidirectionalGraph<TVertex, TEdge>();
            newGraph.AddVerticesAndEdgeRange(graph.Edges);
            return newGraph;
        }
    }
}
