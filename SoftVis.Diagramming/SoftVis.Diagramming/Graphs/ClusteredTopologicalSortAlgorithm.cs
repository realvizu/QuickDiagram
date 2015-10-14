using System.Collections.Generic;
using Codartis.SoftVis.Common;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    /// <summary>
    /// This algorithm takes a directed acyclic graph (DAG) and arranges its vertices into an ordered list of clusters
    /// so between any two clusters all edges point in the same direction.
    /// The direction of the edges is determined by the sort order.
    /// In case of SourcesFirst sort order: all edges point to higher-index clusters 
    /// and cluster 0 contains sources only (vertices with no incoming edges).
    /// In case of SinksFirst sort order: all edges point to lower-index clusters 
    /// and cluster 0 contains sinks only (vertices with no outgoing edges).
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    internal class ClusteredTopologicalSortAlgorithm<TVertex, TEdge> : IAlgorithm
        where TVertex : class
        where TEdge : IEdge<TVertex>
    {
        private readonly IMutableBidirectionalGraph<TVertex, TEdge> _unprocessedGraph;
        private readonly TopologicalSortOrder _sortOrder;

        public List<HashSet<TVertex>> Clusters { get; private set; }

        internal ClusteredTopologicalSortAlgorithm(IVertexAndEdgeListGraph<TVertex, TEdge> graph, TopologicalSortOrder sortOrder)
        {
            _unprocessedGraph = graph.CopyTo<TVertex, TEdge, BidirectionalGraph<TVertex, TEdge>>();
            _sortOrder = sortOrder;
        }

        public void Compute()
        {
            Clusters = new List<HashSet<TVertex>>();

            var currentCluster = new HashSet<TVertex>(GetTopologicallyFirstVertices(_unprocessedGraph.Vertices));
            while (currentCluster.Count > 0)
            {
                var nextCluster = new HashSet<TVertex>();

                foreach (var vertex in currentCluster)
                {
                    var neighbours = GetTopologicallyNextVertices(vertex);

                    _unprocessedGraph.RemoveVertex(vertex);

                    nextCluster.Add(GetTopologicallyFirstVertices(neighbours));
                }

                Clusters.Add(currentCluster);
                currentCluster = nextCluster;
            }

            if (!_unprocessedGraph.IsVerticesEmpty)
                throw new NonAcyclicGraphException();
        }

        private IEnumerable<TVertex> GetTopologicallyFirstVertices(IEnumerable<TVertex> vertices)
        {
            return _sortOrder == TopologicalSortOrder.SinksFirst
                ? _unprocessedGraph.GetSinks(vertices)
                : _unprocessedGraph.GetSources(vertices);
        }

        private IEnumerable<TVertex> GetTopologicallyNextVertices(TVertex vertex)
        {
            return _sortOrder == TopologicalSortOrder.SinksFirst
                ? _unprocessedGraph.GetInNeighbours(vertex)
                : _unprocessedGraph.GetOutNeighbours(vertex);
        }
    }
}