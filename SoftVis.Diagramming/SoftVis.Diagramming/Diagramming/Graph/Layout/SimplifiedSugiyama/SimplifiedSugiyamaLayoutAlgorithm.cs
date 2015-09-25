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
    /// <remarks>
    /// Terminology differences to the original with paper:
    /// <para>rank = layer</para>
    /// </remarks>
    internal class SimplifiedSugiyamaLayoutAlgorithm<TVertex, TEdge>
        : IVertexPositioningAlgorithm<TVertex>, IEdgeRoutingAlgorithm<TVertex, TEdge>
        where TVertex : class, IExtent
        where TEdge : IEdge<TVertex>, IEdge<IExtent>
    {
        private readonly IBidirectionalGraph<TVertex, TEdge> _originalGraph;
        private readonly SimplifiedSugiyamaLayoutParameters _layoutParameters;
        private readonly LayoutGraph _layoutGraph;

        public IDictionary<TVertex, DiagramPoint> VertexCenters { get; private set; }
        public IDictionary<TEdge, DiagramPoint[]> EdgeRoutes { get; private set; }

        internal SimplifiedSugiyamaLayoutAlgorithm(IBidirectionalGraph<TVertex, TEdge> originalGraph, 
            SimplifiedSugiyamaLayoutParameters layoutParameters)
        {
            _originalGraph = originalGraph;
            _layoutParameters = layoutParameters;

            _layoutGraph = new LayoutGraph(_originalGraph.Vertices, _originalGraph.Edges.OfType<IEdge<IExtent>>());
        }

        public void Compute()
        {
            // TODO? Lay out each connected sub-graph separately.

            var isolatedVertices = _layoutGraph.RemoveIsolatedVertices();
            var layoutVertexLayers = CreateLayers(_layoutGraph);
            //InsertVirtualNodes(_layoutGraph, layoutVertexLayers);

            // TODO lay out isolated vertices
        }

        private static LayoutVertexLayers CreateLayers(LayoutGraph layoutGraph)
        {
            // TODO? Set sort order based on layout parameter (top-down or bottom-up layout)
            var sortOrder = TopologicalSortOrder.SinksFirst;
            var sortAlgorithm = new ClusteredTopologicalSortAlgorithm<LayoutVertex, LayoutEdge>(layoutGraph, sortOrder);
            sortAlgorithm.Compute();
            return new LayoutVertexLayers(sortAlgorithm.Clusters);
        }

        //private void InsertVirtualNodes(LayoutGraph layoutGraph, LayoutVertexLayers layoutVertexLayers)
        //{
        //    foreach (var layoutEdge in layoutGraph.Edges)
        //        if (layoutEdge.Source.LayerIndex - layoutEdge.Target.LayerIndex > 1)
        //            ReplaceEdgeWithDummyNodes(layoutEdge, layoutVertexLayers);
        //}

        //private void ReplaceEdgeWithDummyNodes(LayoutEdge layoutEdge, LayoutVertexLayers layoutVertexLayers)
        //{
        //    for (var i = layoutEdge.Target.LayerIndex + 1; i < layoutEdge.Source.LayerIndex - 1; i++)
        //    {
        //        var dummyNode = _layoutGraph.AddDummyNode()
        //        layoutVertexLayers[i].AddItem(dummyNode);
        //        _layoutGraph.a
        //    }
        //}
    }
}