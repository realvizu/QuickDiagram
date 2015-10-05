using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.SimplifiedSugiyama
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
        where TVertex : class, ISized
        where TEdge : IEdge<TVertex>, IEdge<ISized>
    {
        private readonly IBidirectionalGraph<TVertex, TEdge> _originalGraph;
        private readonly SimplifiedSugiyamaLayoutParameters _layoutParameters;

        public IDictionary<TVertex, Point2D> VertexCenters { get; private set; }
        public IDictionary<TEdge, Route> InterimRoutePointsOfEdges { get; private set; }

        internal SimplifiedSugiyamaLayoutAlgorithm(IBidirectionalGraph<TVertex, TEdge> originalGraph,
            SimplifiedSugiyamaLayoutParameters layoutParameters)
        {
            _originalGraph = originalGraph;
            _layoutParameters = layoutParameters;
        }

        public void Compute()
        {
            var originalVertice = _originalGraph.Vertices;
            var originalEdges = _originalGraph.Edges.OfType<IEdge<ISized>>().ToList();
            var layoutGraph = new LayoutGraph(originalVertice, originalEdges);

            // TODO? Lay out each connected sub-graph separately.

            var isolatedVertices = layoutGraph.RemoveIsolatedVertices();
            var rankLayers = CreateLayers(layoutGraph, _layoutParameters.HorizontalGap, _layoutParameters.VerticalGap);
            InsertVirtualNodes(layoutGraph, rankLayers);
            rankLayers = MinimizeCrossings(layoutGraph, rankLayers);
            AssignCoordinates(layoutGraph, rankLayers, isolatedVertices, _layoutParameters.SweepNumber);

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
            List<LayoutVertex> isolatedVertices, int sweepNumber)
        {
            rankLayers.CalculatePositions();
            //rankLayers.CalculatePositions();
            RemoveOverlapsInAllLayers(layoutGraph, rankLayers);
        }

        private static void RemoveOverlapsInAllLayers(LayoutGraph layoutGraph, RankLayers rankLayers)
        {
            foreach (var rankLayer in rankLayers)
            {
                foreach (var layoutVertex in rankLayer)
                {
                    RemoveOverlapsOfVertex(layoutVertex, rankLayer, layoutGraph);
                }
            }
        }

        private static void RemoveOverlapsOfVertex(LayoutVertex fixedVertex, RankLayer rankLayer, LayoutGraph layoutGraph)
        {
            foreach (var otherVertex in rankLayer)
            {
                if (otherVertex == fixedVertex)
                    continue;

                if (IsOverlap(fixedVertex, otherVertex, rankLayer.HorizontalGap))
                    RemoveOverlap(fixedVertex, otherVertex, rankLayer, layoutGraph);
            }
        }

        private static bool IsOverlap(LayoutVertex layoutVertex, LayoutVertex otherVertex, double margin)
        {
            return layoutVertex.Rect.WithMargin(margin / 2, 0).Intersect(otherVertex.Rect.WithMargin(margin / 2, 0)).Width > 0;
        }

        private static void RemoveOverlap(LayoutVertex fixedVertex, LayoutVertex movableVertex, RankLayer rankLayer, LayoutGraph layoutGraph)
        {
            var movementAmount = fixedVertex.Center.X >= movableVertex.Center.X
                ? fixedVertex.Left - movableVertex.Right - rankLayer.HorizontalGap
                : fixedVertex.Right - movableVertex.Left + rankLayer.HorizontalGap;

            HorizontalMoveWithNeighboursBy(movableVertex, movementAmount, rankLayer, layoutGraph);
        }

        private static void HorizontalMoveWithNeighboursBy(LayoutVertex movableVertex, double movementAmount,
            RankLayer rankLayer, LayoutGraph layoutGraph)
        {
            movableVertex.HorizontalTranslateCenterBy(movementAmount);
            RemoveOverlapsOfVertex(movableVertex, rankLayer, layoutGraph);
            //layoutGraph.GetOutNeighbours(movableVertex).ToList().ForEach(i => HorizontalMoveWithOutNeighboursBy(layoutGraph, i, movementAmount));
            //layoutGraph.GetInNeighbours(movableVertex).ToList().ForEach(i => HorizontalMoveWithInNeighboursBy(layoutGraph, i, movementAmount));
        }

        private static void HorizontalMoveWithOutNeighboursBy(LayoutGraph layoutGraph, LayoutVertex movableVertex, double movementAmount)
        {
            movableVertex.HorizontalTranslateCenterBy(movementAmount);
            layoutGraph.GetOutNeighbours(movableVertex).ToList().ForEach(i => HorizontalMoveWithOutNeighboursBy(layoutGraph, i, movementAmount));
        }

        private static void HorizontalMoveWithInNeighboursBy(LayoutGraph layoutGraph, LayoutVertex movableVertex, double movementAmount)
        {
            movableVertex.HorizontalTranslateCenterBy(movementAmount);
            layoutGraph.GetInNeighbours(movableVertex).ToList().ForEach(i => HorizontalMoveWithInNeighboursBy(layoutGraph, i, movementAmount));
        }

        private static void BalancePositions3(LayoutGraph layoutGraph, RankLayers rankLayers)
        {
            foreach (var rankLayer in rankLayers)
            {
                foreach (var layoutVertex in rankLayer)
                {
                    LayOutSmallTree(layoutVertex, layoutGraph, rankLayers.HorizontalGap);
                }
            }
        }

        private static void LayOutSmallTree(LayoutVertex layoutVertex, LayoutGraph layoutGraph, double horizontalGap)
        {
            var neighbours = layoutGraph.GetInNeighbours(layoutVertex).ToList();
            if (!neighbours.Any())
                return;

            var neighboursWidth = neighbours.GetWidth(horizontalGap);
            var x = layoutVertex.Center.X - neighboursWidth / 2;

            foreach (var neighbourVertex in neighbours)
            {
                x += neighbourVertex.Width / 2;
                neighbourVertex.HorizontalTranslateCenterTo(x);
                x += neighbourVertex.Width / 2 + horizontalGap;
            }
        }

        private static void BalancePositions(LayoutGraph layoutGraph, RankLayers rankLayers, int sweepNumber)
        {
            var maxSweep = sweepNumber;
            for (var sweepIndex = 0; sweepIndex < maxSweep; sweepIndex++)
            {
                if (sweepIndex % 2 == 0)
                {
                    for (var i = 0; i < rankLayers.Count - 1; i++)
                        AdjustHorizontalPositions(layoutGraph, rankLayers[i], SweepDirection.Down);
                }
                else
                {
                    for (var i = rankLayers.Count - 1; i > 0; i--)
                        AdjustHorizontalPositions(layoutGraph, rankLayers[i], SweepDirection.Up);
                }
            }
        }

        private static void AdjustHorizontalPositions(LayoutGraph layoutGraph, RankLayer rankLayer, SweepDirection sweepDirection)
        {
            foreach (var layoutVertex in rankLayer)
            {
                var edgeDirection = sweepDirection == SweepDirection.Down ? EdgeDirection.In : EdgeDirection.Out;
                var neighbours = layoutGraph.GetNeighbours(layoutVertex, edgeDirection).ToList();
                if (!neighbours.Any())
                    continue;

                var neighbourSpan = neighbours.GetHorizontalSpan();

                foreach (var neighbourVertex in neighbours)
                {
                    var reverseEdgeDirection = sweepDirection == SweepDirection.Down ? EdgeDirection.Out : EdgeDirection.In;
                    var siblings = layoutGraph.GetNeighbours(neighbourVertex, reverseEdgeDirection).ToList();
                    var siblingSpan = siblings.GetHorizontalSpan();

                    if (siblingSpan.Center < neighbourSpan.Center)
                        rankLayer.HorizontalTranslateAtLeastTo(layoutVertex, neighbourSpan.Center);
                }
            }
        }

        private static void BalancePositions2(LayoutGraph layoutGraph, RankLayers rankLayers, int sweepNumber)
        {
            var maxSweep = sweepNumber;
            for (var sweepIndex = 0; sweepIndex < maxSweep; sweepIndex++)
            {
                for (var i = 0; i < rankLayers.Count - 1; i++)
                    AdjustHorizontalPositions(layoutGraph, rankLayers[i], rankLayers[i + 1]);
            }
        }

        private static void AdjustHorizontalPositions(LayoutGraph layoutGraph, RankLayer upperLayer, RankLayer lowerLayer)
        {
            foreach (var upperVertex in upperLayer)
            {
                var lowerNeighbours = layoutGraph.GetInNeighbours(upperVertex).ToList();
                if (!lowerNeighbours.Any())
                    continue;

                var lowerSpan = lowerNeighbours.GetHorizontalSpan();

                foreach (var lowerVertex in lowerNeighbours)
                {
                    var upperNeighbours = layoutGraph.GetOutNeighbours(lowerVertex).ToList();
                    var upperSpan = upperNeighbours.GetHorizontalSpan();

                    if (upperSpan.Center < lowerSpan.Center)
                        upperLayer.HorizontalTranslateAtLeastTo(upperVertex, lowerSpan.Center);
                    else if (lowerSpan.Center < upperSpan.Center)
                        lowerLayer.HorizontalTranslateAtLeastTo(lowerVertex, upperSpan.Center);
                }
            }
        }

        private static void DumpLayer(RankLayer adjustedLayer, string tag)
        {
            Console.WriteLine($"Layer {adjustedLayer.Rank} {tag}:");
            foreach (var layoutVertex in adjustedLayer)
            {
                Console.WriteLine($"{layoutVertex.Width} , {layoutVertex.Center}");
            }
        }

        private static IDictionary<TVertex, Point2D> GetVertexCenters(LayoutGraph layoutGraph)
        {
            return layoutGraph.Vertices.Where(i => !i.IsDummy)
                .ToDictionary(i => (TVertex)i.OriginalVertex, i => i.Center);
        }

        private static IDictionary<TEdge, Route> GetInterimRoutePointsOfEdges(LayoutGraph layoutGraph)
        {
            return layoutGraph.GetInterimRoutePointsOfEdges().ToDictionary(i => (TEdge)i.Key, i => i.Value);
        }

        private enum SweepDirection
        {
            Up,
            Down
        }
    }
}