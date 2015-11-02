using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using QuickGraph;
using QuickGraph.Algorithms.Search;
using QuickGraph.Serialization;

namespace Codartis.SoftVis.Graphs
{
    public static class GraphExtensions
    {
        public static IEnumerable<TEdge> GetAllEdges<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph,
            TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            return graph.InEdges(vertex).Union(graph.OutEdges(vertex));
        }

        public static IEnumerable<TEdge> GetEdges<TVertex, TEdge>(
            this IBidirectionalGraph<TVertex, TEdge> graph, TVertex vertex, EdgeDirection edgeDirection)
            where TEdge : IEdge<TVertex>
        {
            return edgeDirection == EdgeDirection.In
                ? graph.InEdges(vertex)
                : graph.OutEdges(vertex);
        }

        public static IEnumerable<TVertex> GetAllNeighbours<TVertex, TEdge>(
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

        public static bool AreSiblings<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph,
            TVertex vertex1, TVertex vertex2, EdgeDirection edgeDirection)
            where TEdge : IEdge<TVertex>
        {
            var vertex1Parents = graph.GetNeighbours(vertex1, edgeDirection);
            var vertex2Parents = graph.GetNeighbours(vertex2, edgeDirection);
            return vertex1Parents.Intersect(vertex2Parents).Any();
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

        public static void ExecuteOnVerticesRecursive<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph,
            TVertex rootVertex, EdgeDirection edgeDirection, Action<TVertex> actionOnVertex)
            where TEdge : IEdge<TVertex>
        {
            actionOnVertex(rootVertex);
            foreach (var layoutEdge in graph.GetEdges(rootVertex, edgeDirection))
            {
                var nextVertex = layoutEdge.GetEndVertex(edgeDirection);
                graph.ExecuteOnVerticesRecursive(nextVertex, edgeDirection, actionOnVertex);
            }
        }

        public static void ExecuteOnEdgesRecursive<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph,
            TEdge edge, EdgeDirection edgeDirection, Action<TEdge> actionOnEdge)
            where TEdge : IEdge<TVertex>
        {
            actionOnEdge(edge);
            foreach (var nextEdge in graph.GetEdges(edge.GetEndVertex(edgeDirection), edgeDirection))
                graph.ExecuteOnEdgesRecursive(nextEdge, edgeDirection, actionOnEdge);
        }

        public static IEnumerable<TEdge> FindCycleEdges<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var cycleEdges = new List<TEdge>();

            var searchAlgorithm = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
            searchAlgorithm.BackEdge += cycleEdges.Add;
            searchAlgorithm.Compute();

            return cycleEdges;
        }

        public static bool HasCycle<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return FindCycleEdges(graph).Any();
        }

        public static string Serialize<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            using (var memoryStream = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8 };

                using (var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    var serializer = new GraphMLSerializer<TVertex, TEdge, IEdgeListGraph<TVertex, TEdge>>();
                    serializer.Serialize(xmlWriter, graph, v => v.ToString(), e => e.ToString());
                    xmlWriter.Flush();
                }

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public static void Save<TVertex, TEdge>(this IBidirectionalGraph<TVertex, TEdge> graph, string filename)
            where TEdge : IEdge<TVertex>
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.Write(Serialize(graph));
            }
        }
    }
}
