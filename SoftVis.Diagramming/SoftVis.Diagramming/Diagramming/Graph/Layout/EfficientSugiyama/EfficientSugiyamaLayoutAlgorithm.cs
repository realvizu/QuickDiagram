using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.EfficientSugiyama
{
    /// <summary>
    /// An Efficient Implementation of Sugiyama’s Algorithm for Layered Graph Drawing.
    /// See: http://jgaa.info/accepted/2005/EiglspergerSiebenhallerKaufmann2005.9.3.pdf
    /// </summary>
    /// <typeparam name="TVertex">The type of the graph's vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the graph's edges.</typeparam>
    internal class EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge> : IVertexPositioningAlgorithm<TVertex>, IEdgeRoutingAlgorithm<TVertex, TEdge>
        where TVertex : class, IExtent
        where TEdge : IEdge<TVertex>, IEdge<IExtent>
    {
        private readonly IBidirectionalGraph<TVertex, TEdge> _graph;
        private readonly EfficientSugiyamaLayoutParameters _layoutParameters;

        public IDictionary<TVertex, DiagramPoint> VertexCenters { get; private set; }
        public IDictionary<TEdge, DiagramPoint[]> EdgeRoutes { get; private set; }

        internal EfficientSugiyamaLayoutAlgorithm(IBidirectionalGraph<TVertex, TEdge> graph, EfficientSugiyamaLayoutParameters layoutParameters)
        {
            _graph = graph;
            _layoutParameters = layoutParameters;
        }

        public void Compute()
        {
            var originalEdges = _graph.Edges.OfType<IEdge<IExtent>>().ToList();

            var sugiGraph = new SugiGraph(_graph.Vertices, originalEdges);

            sugiGraph.RemoveLoops();
            sugiGraph.RemoveCycles();
            var isolatedVertices = sugiGraph.GetIsolatedVertices().ToList();
            sugiGraph.RemoveVertices(isolatedVertices);

            var sparseNormalizationAlgorithm = new SparseNormalizationAlgorithm(sugiGraph, _layoutParameters);
            sparseNormalizationAlgorithm.Compute();
            var layers = sparseNormalizationAlgorithm.Layers;
            var edgeToDummyVerticesMap = sparseNormalizationAlgorithm.EdgeToDummyVerticesMap;

            var crossingMinimizationAlgorithm = new CrossingMinimizationAlgorithm(sugiGraph, layers);
            crossingMinimizationAlgorithm.Compute();
            var sparseCompactionGraph = crossingMinimizationAlgorithm.SparseCompactionGraph;
            var sparseCompactionByLayerBackup = crossingMinimizationAlgorithm.SparseCompactionByLayerBackup;

            var positionCalculatorAlgorithm = new PositionCalculatorAlgorithm(sugiGraph, _layoutParameters,
                layers, sparseCompactionGraph, sparseCompactionByLayerBackup, isolatedVertices);
            positionCalculatorAlgorithm.Compute();
            VertexCenters = positionCalculatorAlgorithm.VertexCenters.ToDictionary(i => (TVertex)i.Key, i => i.Value);

            var edgeRoutingAlgorithm = new EdgeRoutingAlgorithm(originalEdges, sugiGraph, _layoutParameters, layers, edgeToDummyVerticesMap);
            edgeRoutingAlgorithm.Compute();
            EdgeRoutes = edgeRoutingAlgorithm.EdgeRoutes.ToDictionary(i => (TEdge)i.Key, i => i.Value);
        }
    }
}