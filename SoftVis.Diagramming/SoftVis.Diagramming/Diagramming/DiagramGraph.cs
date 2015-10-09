using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// The graph formed by the nodes and connectors of a diagram.
    /// </summary>
    internal sealed class DiagramGraph : BidirectionalGraph<DiagramNode, DiagramConnector>
    {
        private const double HorizontalGap = 20;
        private const double VerticalGap = 40;
        private readonly IncrementalLayoutEngine _layoutEngine;

        public List<RectMove> LastEdgeTriggeredVertexMoves => _layoutEngine.LastEdgeTriggeredVertexMoves;
        public int TotalVertexMoveCount => _layoutEngine.TotalVertexMoveCount;

        public DiagramGraph()
        {
            _layoutEngine = new IncrementalLayoutEngine(HorizontalGap, VerticalGap);
            _layoutEngine.VertexCenterChanged += OnVertexCenterChanged;
            _layoutEngine.EdgeRouteChanged += OnEdgeRouteChanged;
            Cleared += OnCleared;
        }

        private void OnCleared(object sender, EventArgs e)
        {
            _layoutEngine.Clear();
        }

        public override bool AddVertex(DiagramNode node)
        {
            var isAdded = base.AddVertex(node);

            if (isAdded)
                _layoutEngine.Add(node);

            return isAdded;
        }

        public override bool AddEdge(DiagramConnector connector)
        {
            var isAdded = base.AddEdge(connector);

            if (isAdded)
                _layoutEngine.Add(connector);

            return isAdded;
        }

        public IEnumerable<DiagramPath> GetShortestPaths(
            DiagramNode source, DiagramNode target, int pathCounts)
        {
            return this.RankedShortestPathHoffmanPavley(i => 1, source, target, pathCounts).Select(i => new DiagramPath(i));
        }

        private static void OnVertexCenterChanged(object sender, MoveEventArgs args)
        {
            ((DiagramNode)sender).Center = args.To;
        }

        private static void OnEdgeRouteChanged(object sender, Route routePoints)
        {
            ((DiagramConnector)sender).RoutePoints = routePoints;
        }
    }
}
