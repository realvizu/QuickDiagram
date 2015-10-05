using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
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

        public static IEnumerable<TVertex> GetNeighbours<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph,
            TVertex vertex, EdgeDirection edgeDirection)
            where TEdge : IEdge<TVertex>
        {
            switch (edgeDirection)
            {
                case (EdgeDirection.In):
                    return graph.GetInNeighbours(vertex);
                case (EdgeDirection.Out):
                    return graph.GetOutNeighbours(vertex);
                default:
                    throw new ArgumentException($"Unexpected edge direction: {edgeDirection}");
            }
        }

        public static IEnumerable<TEdge> GetEdges<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph,
            TVertex vertex, EdgeDirection edgeDirection)
            where TEdge : IEdge<TVertex>
        {
            switch (edgeDirection)
            {
                case (EdgeDirection.In):
                    return graph.InEdges(vertex);
                case (EdgeDirection.Out):
                    return graph.OutEdges(vertex);
                default:
                    throw new ArgumentException($"Unexpected edge direction: {edgeDirection}");
            }
        }

        public static TVertex OtherVertex<TVertex>(this IEdge<TVertex> edge, TVertex thisVertex)
        {
            return edge.Source.Equals(thisVertex) ? edge.Target : edge.Source;
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
