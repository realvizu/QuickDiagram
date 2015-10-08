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

        public readonly IncrementalLayoutEngine LayoutEngine;

        public DiagramGraph()
        {
            LayoutEngine = new IncrementalLayoutEngine(HorizontalGap, VerticalGap);
            LayoutEngine.VertexCenterChanged += OnVertexCenterChanged;
            LayoutEngine.EdgeRouteChanged += OnEdgeRouteChanged;
            Cleared += OnCleared;
        }

        private void OnCleared(object sender, EventArgs e)
        {
            LayoutEngine.Clear();
        }

        public override bool AddVertex(DiagramNode node)
        {
            var isAdded = base.AddVertex(node);

            if (isAdded)
                LayoutEngine.Add(node);

            return isAdded;
        }

        public override bool AddEdge(DiagramConnector connector)
        {
            var isAdded = base.AddEdge(connector);

            if (isAdded)
                LayoutEngine.Add(connector);

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
