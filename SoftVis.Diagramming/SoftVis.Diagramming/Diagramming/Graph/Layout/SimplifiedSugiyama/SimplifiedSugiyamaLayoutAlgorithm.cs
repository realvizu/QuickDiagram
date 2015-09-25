using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    /// <summary>
    /// Simplified Sugiyama-style layout algorithm.
    /// Based on: www.graphviz.org/Documentation/TSE93.pdf
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    internal class SimplifiedSugiyamaLayoutAlgorithm<TVertex, TEdge>
        : IVertexPositioningAlgorithm<TVertex>,
          IEdgeRoutingAlgorithm<TVertex, TEdge>
        where TVertex : class, IExtent
        where TEdge : IEdge<TVertex>, IEdge<IExtent>
    {
        private readonly IBidirectionalGraph<TVertex, TEdge> _originalGraph;
        private readonly SimplifiedSugiyamaLayoutParameters _layoutParameters;

        public IDictionary<TVertex, DiagramPoint> VertexCenters { get; private set; }
        public IDictionary<TEdge, DiagramPoint[]> EdgeRoutes { get; private set; }

        internal SimplifiedSugiyamaLayoutAlgorithm(IBidirectionalGraph<TVertex, TEdge> originalGraph,
            SimplifiedSugiyamaLayoutParameters layoutParameters)
        {
            _originalGraph = originalGraph;
            _layoutParameters = layoutParameters;
        }

        public void Compute()
        {
            var layoutGraph = new LayoutGraph(_originalGraph.Vertices, _originalGraph.Edges.OfType<IEdge<IExtent>>());

            // TODO? Lay out each connected sub-graph separately.

            var isolatedVertices = layoutGraph.RemoveIsolatedVertices();
            var rankLayers = CreateLayers(layoutGraph);
            InsertVirtualNodes(layoutGraph, rankLayers);

            // TODO lay out isolated vertices
        }

        private static RankLayers CreateLayers(LayoutGraph layoutGraph)
        {
            // TODO? adjust for top-down layout
            var sortOrder = TopologicalSortOrder.SinksFirst;
            var sortAlgorithm = new ClusteredTopologicalSortAlgorithm<LayoutVertex, LayoutEdge>(layoutGraph, sortOrder);
            sortAlgorithm.Compute();
            return new RankLayers(sortAlgorithm.Clusters);
        }

        private static void InsertVirtualNodes(LayoutGraph layoutGraph, RankLayers rankLayers)
        {
            foreach (var layoutEdge in layoutGraph.Edges.ToList())
            { 
                // TODO: adjust for top-down layout
                if (layoutEdge.Source.Rank - layoutEdge.Target.Rank > 1)
                {
                    ReplaceEdgeWithDummyNodes(layoutEdge, layoutGraph, rankLayers);
                }
            }
        }

        private static void ReplaceEdgeWithDummyNodes(LayoutEdge layoutEdge, LayoutGraph layoutGraph,  RankLayers rankLayers)
        {
            var dummyNodes = new List<LayoutVertex>();

            // TODO: adjust for top-down layout
            for (var i = layoutEdge.Source.Rank - 1; i > layoutEdge.Target.Rank + 1; i--)
            {
                var dummyNode = LayoutVertex.CreateDummy();
                rankLayers[i].AddItem(dummyNode);
                dummyNodes.Add(dummyNode);
            }

            layoutGraph.BreakEdgeWithInterimVertices(layoutEdge, dummyNodes);
        }
    }
}