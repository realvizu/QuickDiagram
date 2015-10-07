using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    public static class GraphExtensions
    {
        public static IEnumerable<TEdge> GetEdges<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, TVertex vertex, EdgeDirection edgeDirection)
            where TEdge : IEdge<TVertex>
        {
            return edgeDirection == EdgeDirection.In
                ? graph.InEdges(vertex)
                : graph.OutEdges(vertex);
        }

        public static IEnumerable<TVertex> GetNeighbours<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.GetInNeighbours(vertex).Union(graph.GetOutNeighbours(vertex));
        }

        public static IEnumerable<TVertex> GetNeighbours<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, TVertex vertex, EdgeDirection edgeDirection)
            where TEdge : IEdge<TVertex>
        {
            return edgeDirection == EdgeDirection.In
                ? graph.GetInNeighbours(vertex)
                : graph.GetOutNeighbours(vertex);
        }

        public static IEnumerable<TVertex> GetInNeighbours<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.InEdges(vertex).Select(e => e.Source).Distinct();
        }

        public static IEnumerable<TVertex> GetOutNeighbours<TVertex, TEdge>(
            this IVertexAndEdgeListGraph<TVertex, TEdge> graph, TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.OutEdges(vertex).Select(e => e.Target).Distinct();
        }

        public static IEnumerable<TVertex> GetSources<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, IEnumerable<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
            return vertices.Where(graph.IsInEdgesEmpty);
        }

        public static IEnumerable<TVertex> GetSinks<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, IEnumerable<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
            return vertices.Where(graph.IsOutEdgesEmpty);
        }

        public static TGraphImplementation CopyTo<TVertex, TEdge, TGraphImplementation>(
            this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            where TGraphImplementation : IMutableVertexSet<TVertex>, IMutableEdgeListGraph<TVertex, TEdge>, new()
        {
            var newGraph = new TGraphImplementation();
            newGraph.AddVertexRange(graph.Vertices);
            newGraph.AddEdgeRange(graph.Edges);
            return newGraph;
        }
    }
}
