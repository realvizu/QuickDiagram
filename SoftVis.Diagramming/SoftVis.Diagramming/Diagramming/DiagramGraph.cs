using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Geometry;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// The graph formed by the nodes and connectors of a diagram.
    /// Has a layout engine that calculates how to arrange nodes and connectors.
    /// </summary>
    internal sealed class DiagramGraph : BidirectionalGraph<DiagramNode, DiagramConnector>
    {
        private const double HorizontalGap = 20;
        private const double VerticalGap = 40;
        private readonly IncrementalLayoutEngine _layoutEngine;

        public ILayoutActionGraph LastLayoutActionGraph => _layoutEngine.LastLayoutActionGraph;
        public int TotalVertexMoveCount => _layoutEngine.TotalVertexMoveCount;

        public DiagramGraph() : base(false)
        {
            _layoutEngine = new IncrementalLayoutEngine(HorizontalGap, VerticalGap);
            _layoutEngine.DiagramNodeCenterChanged += OnDiagramNodeCenterChanged;
            _layoutEngine.DiagramConnectorRouteChanged += OnDiagramConnectorRouteChanged;
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

        public override bool RemoveVertex(DiagramNode node)
        {
            var isRemoved = base.RemoveVertex(node);
            if (isRemoved)
                _layoutEngine.Remove(node);
            return isRemoved;
        }

        public override bool AddEdge(DiagramConnector connector)
        {
            var isAdded = base.AddEdge(connector);
            if (isAdded)
                _layoutEngine.Add(connector);
            return isAdded;
        }

        public override bool RemoveEdge(DiagramConnector connector)
        {
            var isRemoved = base.RemoveEdge(connector);
            if (isRemoved)
                _layoutEngine.Remove(connector);
            return isRemoved;
        }

        public IEnumerable<DiagramPath> GetShortestPaths(DiagramNode source, DiagramNode target, int pathCount)
        {
            return this.RankedShortestPathHoffmanPavley(i => 1, source, target, pathCount).Select(i => new DiagramPath(i));
        }

        private static void OnDiagramNodeCenterChanged(object sender, RectMove args)
        {
            ((DiagramNode)sender).Center = args.ToCenter;
        }

        private static void OnDiagramConnectorRouteChanged(object sender, Route routePoints)
        {
            ((DiagramConnector)sender).RoutePoints = routePoints;
        }
    }
}
