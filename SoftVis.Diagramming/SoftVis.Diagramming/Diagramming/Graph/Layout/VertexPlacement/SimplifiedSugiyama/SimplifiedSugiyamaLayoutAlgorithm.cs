using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.SimplifiedSugiyama
{
    /// <summary>
    /// Simplified Sugiyama-style layout algorithm.
    /// Based on: www.graphviz.org/Documentation/TSE93.pdf
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    internal class SimplifiedSugiyamaLayoutAlgorithm<TVertex, TEdge>
        : IVertexPositioningAlgorithm<TVertex>,
          IEdgeRoutingSupportAlgorithm<TVertex, TEdge>
        where TVertex : class, IExtent
        where TEdge : IEdge<TVertex>, IEdge<IExtent>
    {
        private readonly IBidirectionalGraph<TVertex, TEdge> _originalGraph;
        private readonly SimplifiedSugiyamaLayoutParameters _layoutParameters;

        public IDictionary<TVertex, DiagramPoint> VertexCenters { get; private set; }
        public IDictionary<TEdge, DiagramPoint[]> InterimRoutePointsOfEdges { get; private set; }

        internal SimplifiedSugiyamaLayoutAlgorithm(IBidirectionalGraph<TVertex, TEdge> originalGraph,
            SimplifiedSugiyamaLayoutParameters layoutParameters)
        {
            _originalGraph = originalGraph;
            _layoutParameters = layoutParameters;
        }

        public void Compute()
        {
            var originalVertice = _originalGraph.Vertices;
            var originalEdges = _originalGraph.Edges.OfType<IEdge<IExtent>>().ToList();
            var layoutGraph = new LayoutGraph(originalVertice, originalEdges);

            // TODO? Lay out each connected sub-graph separately.

            var isolatedVertices = layoutGraph.RemoveIsolatedVertices();
            var rankLayers = CreateLayers(layoutGraph, _layoutParameters.HorizontalGap, _layoutParameters.VerticalGap);
            InsertVirtualNodes(layoutGraph, rankLayers);
            rankLayers = MinimizeCrossings(layoutGraph, rankLayers);
            AssignCoordinates(layoutGraph, rankLayers, isolatedVertices);

            VertexCenters = GetVertexCenters(layoutGraph);
            InterimRoutePointsOfEdges = GetInterimRoutePointsOfEdges(layoutGraph);
        }

        private static RankLayers CreateLayers(LayoutGraph layoutGraph, double horizontalGap, double verticalGap)
        {
            // TODO? adjust for top-down layout
            var sortOrder = TopologicalSortOrder.SinksFirst;
            var sortAlgorithm = new ClusteredTopologicalSortAlgorithm<LayoutVertex, LayoutEdge>(layoutGraph, sortOrder);
            sortAlgorithm.Compute();
            return new RankLayers(horizontalGap, verticalGap, sortAlgorithm.Clusters);
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

        private static void ReplaceEdgeWithDummyNodes(LayoutEdge layoutEdge, LayoutGraph layoutGraph, RankLayers rankLayers)
        {
            var dummyNodes = new List<LayoutVertex>();

            // TODO: adjust for top-down layout
            for (var i = layoutEdge.Source.Rank - 1; i >= layoutEdge.Target.Rank + 1; i--)
            {
                var dummyNode = LayoutVertex.CreateDummy();
                rankLayers[i].AddItem(dummyNode);
                dummyNodes.Add(dummyNode);
            }

            layoutGraph.BreakEdgeWithInterimVertices(layoutEdge, dummyNodes);
        }

        private static RankLayers MinimizeCrossings(LayoutGraph layoutGraph, RankLayers rankLayers)
        {
            return InitialOrdering(layoutGraph, rankLayers);
        }

        private static RankLayers InitialOrdering(LayoutGraph layoutGraph, RankLayers rankLayers)
        {
            var reorderedRankLayers = new RankLayers(rankLayers.HorizontalGap, rankLayers.VerticalGap);
            reorderedRankLayers.AddLayer(rankLayers[0]);

            for (var i = 0; i < rankLayers.Count - 1; i++)
            {
                var newLayer = reorderedRankLayers.AddLayer();
                foreach (var layoutVertex in reorderedRankLayers[i])
                {
                    var inNeighbours = layoutGraph.GetInNeighbours(layoutVertex);
                    foreach (var inNeighbour in inNeighbours)
                    {
                        if (!newLayer.Contains(inNeighbour))
                            newLayer.AddItem(inNeighbour);
                    }
                }
            }

            return reorderedRankLayers;
        }

        private static void AssignCoordinates(LayoutGraph layoutGraph, RankLayers rankLayers,
            List<LayoutVertex> isolatedVertices)
        {
            rankLayers.CalculatePositions();
            BalancePositions(layoutGraph, rankLayers);
        }

        private static void BalancePositions(LayoutGraph layoutGraph, RankLayers rankLayers)
        {
            for (var i = 0; i < rankLayers.Count - 1; i++)
                AdjustHorizontalPositions(layoutGraph, rankLayers[i], rankLayers[i + 1]);
        }

        private static void AdjustHorizontalPositions(LayoutGraph layoutGraph, RankLayer upperLayer, RankLayer lowerLayer)
        {
            //DumpLayer(upperLayer, "before");

            foreach (var upperVertex in upperLayer)
            {
                //Debug.WriteLine($"{((DiagramNode)upperVertex.OriginalVertex)?.Name}");

                var lowerNeighbours = layoutGraph.GetInNeighbours(upperVertex).ToList();
                //Debug.WriteLine($"lowerNeighbours count = {lowerNeighbours.Count}");
                if (!lowerNeighbours.Any())
                    continue;

                var lowerSpan = lowerNeighbours.GetHorizontalSpan();
                //Debug.WriteLine($"lowerSpan = {lowerSpan}");

                foreach (var lowerVertex in lowerNeighbours)
                {
                    //Debug.WriteLine($"{((DiagramNode)lowerVertex.OriginalVertex)?.Name}");

                    var upperNeighbours = layoutGraph.GetOutNeighbours(lowerVertex).ToList();
                    //Debug.WriteLine($"upperNeighbours count = {upperNeighbours.Count}");
                    var upperSpan = upperNeighbours.GetHorizontalSpan();
                    //Debug.WriteLine($"upperSpan = {upperSpan}");

                    if (upperSpan.Center < lowerSpan.Center)
                        upperLayer.HorizontalTranslateAtLeastTo(upperVertex, lowerSpan.Center);
                    else if (lowerSpan.Center < upperSpan.Center)
                        lowerLayer.HorizontalTranslateAtLeastTo(lowerVertex, upperSpan.Center);
                }
            }

            //DumpLayer(upperLayer, "after");
            //Debug.WriteLine("---------------------------------");
        }

        private static void DumpLayer(RankLayer adjustedLayer, string tag)
        {
            Console.WriteLine($"Layer {adjustedLayer.Rank} {tag}:");
            foreach (var layoutVertex in adjustedLayer)
            {
                Console.WriteLine($"{layoutVertex.Width} , {layoutVertex.Center}");
            }
        }

        private static IDictionary<TVertex, DiagramPoint> GetVertexCenters(LayoutGraph layoutGraph)
        {
            return layoutGraph.Vertices.Where(i => !i.IsDummy)
                .ToDictionary(i => (TVertex)i.OriginalVertex, i => i.Center);
        }

        private static IDictionary<TEdge, DiagramPoint[]> GetInterimRoutePointsOfEdges(LayoutGraph layoutGraph)
        {
            return layoutGraph.GetInterimRoutePointsOfEdges().ToDictionary(i => (TEdge)i.Key, i => i.Value);
        }
    }
}