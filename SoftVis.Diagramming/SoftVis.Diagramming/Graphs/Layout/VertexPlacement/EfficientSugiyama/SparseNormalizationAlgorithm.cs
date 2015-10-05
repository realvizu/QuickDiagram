using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    /// <summary>
    /// Creates a sparse normalized graph with segments and dummy vertices.
    /// </summary>
    internal sealed class SparseNormalizationAlgorithm : IAlgorithm
    {
        private readonly SugiGraph _sugiGraph;
        private readonly EfficientSugiyamaLayoutParameters _layoutParameters;

        public Layers Layers { get; private set; }
        public EdgeToDummyVerticesMap EdgeToDummyVerticesMap { get; private set; }

        internal SparseNormalizationAlgorithm(SugiGraph sugiGraph, EfficientSugiyamaLayoutParameters layoutParameters)
        {
            _sugiGraph = sugiGraph;
            _layoutParameters = layoutParameters;
        }

        public void Compute()
        {
            Layers = CreateInitialLayering(_sugiGraph, _layoutParameters.LayoutDirection);

            if (_layoutParameters.LayoutDirection == LayoutDirection.SourcesAtTop &&
                _layoutParameters.MinimizeEdgeLength)
            {
                MinimizeEdgeLengths(_sugiGraph, Layers);
            }

            EdgeToDummyVerticesMap = CreateDummyVertices(_sugiGraph, Layers);
        }

        private static Layers CreateInitialLayering(SugiGraph sugiGraph, LayoutDirection layoutDirection)
        {
            var sortOrder = layoutDirection == LayoutDirection.SourcesAtTop
                ? TopologicalSortOrder.SourcesFirst
                : TopologicalSortOrder.SinksFirst;

            var sortAlgorithm = new ClusteredTopologicalSortAlgorithm<SugiVertex, SugiEdge>(sugiGraph, sortOrder);
            sortAlgorithm.Compute();

            return new Layers(sortAlgorithm.Clusters);
        }

        private static void MinimizeEdgeLengths(SugiGraph sugiGraph, Layers layers)
        {
            for (var i = layers.Count - 1; i >= 0; i--)
            {
                var layer = layers[i];
                foreach (var sugiVertex in layer.ToList())
                {
                    if (sugiGraph.OutDegree(sugiVertex) > 0)
                    {
                        var closestOutNeighbourLayerIndex = 
                            sugiGraph.OutEdges(sugiVertex).Min(edge => edge.Target.LayerIndex);

                        if (sugiVertex.LayerIndex < closestOutNeighbourLayerIndex - 1)
                            layers.MoveItem(sugiVertex, closestOutNeighbourLayerIndex - 1);
                    }
                }
            }
        }

        private static EdgeToDummyVerticesMap CreateDummyVertices(SugiGraph sugiGraph, Layers layers)
        {
            var edgeToDummyVerticesMap = new EdgeToDummyVerticesMap();

            foreach (var sugiEdge in sugiGraph.Edges.ToList())
            {
                var sourceLayerIndex = sugiEdge.Source.LayerIndex;
                var targetLayerIndex = sugiEdge.Target.LayerIndex;

                var layerSpan = targetLayerIndex - sourceLayerIndex;
                if (layerSpan < 1)
                    throw new Exception("Invalid layer span.");

                IEnumerable<SugiVertex> dummyVerticesOfEdge;

                switch (layerSpan)
                {
                    case 1:
                        continue;
                    case 2:
                        var rVertexLayer = layers[sourceLayerIndex + 1];
                        dummyVerticesOfEdge = InsertRVertex(sugiEdge, rVertexLayer, sugiGraph);
                        break;
                    default:
                        var pVertexLayer = layers[sourceLayerIndex + 1];
                        var qVertexLayer = layers[targetLayerIndex - 1];
                        dummyVerticesOfEdge = InsertSegment(sugiEdge, pVertexLayer, qVertexLayer, sugiGraph);
                        break;
                }

                edgeToDummyVerticesMap.Add(sugiEdge.OriginalEdge, dummyVerticesOfEdge.ToList());
            }

            return edgeToDummyVerticesMap;
        }

        private static IEnumerable<SugiVertex> InsertRVertex(SugiEdge sugiEdge, Layer layer, SugiGraph sugiGraph)
        {
            var rVertex = SugiVertex.CreateRVertex(layer);

            sugiGraph.BreakEdgeWithDummyVertex(sugiEdge, rVertex);

            yield return rVertex;
        }

        private static IEnumerable<SugiVertex> InsertSegment(SugiEdge sugiEdge, Layer pVertexLayer, Layer qVertexLayer, 
            SugiGraph sugiGraph)
        {
            var segment = Segment.Create(pVertexLayer, qVertexLayer);

            sugiGraph.BreakEdgeWithSegment(sugiEdge, segment);

            return ReverseVertexOrderForReversedEdge(sugiEdge, segment.Vertices);
        }

        private static IEnumerable<SugiVertex> ReverseVertexOrderForReversedEdge(SugiEdge sugiEdge, 
            IEnumerable<SugiVertex> sugiVertices)
        {
            var isReverseEdge = sugiEdge.Source.OriginalVertex != sugiEdge.OriginalEdge.Source;
            return isReverseEdge
                ? sugiVertices.Reverse()
                : sugiVertices;
        }
    }
}

