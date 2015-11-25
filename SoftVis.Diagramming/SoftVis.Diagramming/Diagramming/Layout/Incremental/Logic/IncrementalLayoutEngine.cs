using System;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute.Logic;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Logic
{
    /// <summary>
    /// Attaches to a DiagramGraph's events and calculates and publishes layout action events
    /// whenever vertices and edges are added or removed.
    /// </summary>
    /// <remarks>
    /// Layout rules:
    /// <para>Adding a new node adds it to the first layer in node name order.</para>
    /// <para>Adding a new inheritance connection moves the source node under the target node.
    /// The source node brings all its children with it.</para>
    /// <para>If the source node has siblings then it is placed among them based on node name order.</para>
    /// <para>If the source node has no siblings then it is placed between the children of the parent's preceding and following nodes.
    /// It ensures that there won't be any inheritance edge crossings.</para>
    /// </remarks>
    internal sealed class IncrementalLayoutEngine : AbsoluteLayoutActionEventSource
    {
        private readonly DiagramGraph _diagramGraph;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;
        private LayoutVertexToPointMap _previousVertexCenters;

        private readonly RelativeLayoutCalculator _relativeLayoutCalculator;
        private readonly AbsolutePositionCalculator _absolutePositionCalculator;

        public IncrementalLayoutEngine(DiagramGraph diagramGraph)
        {
            const double horizontalGap = DiagramDefaults.HorizontalGap;
            const double verticalGap = DiagramDefaults.VerticalGap;

            _diagramGraph = diagramGraph;

            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();
            _previousVertexCenters = new LayoutVertexToPointMap();

            _relativeLayoutCalculator = new RelativeLayoutCalculator();
            _relativeLayoutCalculator.LayoutActionExecuted += OnLayoutActionExecuted;

            _absolutePositionCalculator = new AbsolutePositionCalculator(_relativeLayoutCalculator.RelativeLayout,
                horizontalGap, verticalGap);
            _absolutePositionCalculator.LayoutActionExecuted += OnLayoutActionExecuted;

            HookIntoDiagramGraphEvents();
        }

        private IReadOnlyRelativeLayout RelativeLayout => _relativeLayoutCalculator.RelativeLayout;

        private void HookIntoDiagramGraphEvents()
        {
            _diagramGraph.Cleared += OnDiagramGraphCleared;
            _diagramGraph.VertexAdded += OnDiagramNodeAdded;
            _diagramGraph.VertexRemoved += OnDiagramNodeRemoved;
            _diagramGraph.EdgeAdded += OnDiagramConnectorAdded;
            _diagramGraph.EdgeRemoved += OnDiagramConnectorRemoved;
        }

        private void OnDiagramGraphCleared(object sender, EventArgs e)
        {
            _relativeLayoutCalculator.OnDiagramCleared();

            _layoutPathToPreviousRouteMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
            _diagramNodeToLayoutVertexMap.Clear();
            _previousVertexCenters.Clear();
        }

        private void OnDiagramNodeAdded(DiagramNode diagramNode)
        {
            if (_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var layoutAction = RaiseDiagramNodeLayoutAction("AddDiagramNode", diagramNode);

            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            _relativeLayoutCalculator.OnDiagramNodeAdded(diagramNodeLayoutVertex, layoutAction);
            CalculateAbsolutePositions();
        }

        private void OnDiagramNodeRemoved(DiagramNode diagramNode)
        {
            if (!_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var layoutAction = RaiseDiagramNodeLayoutAction("RemoveDiagramNode", diagramNode);

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            _relativeLayoutCalculator.OnDiagramNodeRemoved(diagramNodeLayoutVertex, layoutAction);
            CalculateAbsolutePositions();
        }

        private void OnDiagramConnectorAdded(DiagramConnector diagramConnector)
        {
            if (_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var layoutAction = RaiseDiagramConnectorLayoutAction("AddDiagramConnector", diagramConnector);

            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            _relativeLayoutCalculator.OnDiagramConnectorAdded(layoutPath, layoutAction);
            CalculateAbsolutePositions();
        }

        private void OnDiagramConnectorRemoved(DiagramConnector diagramConnector)
        {
            if (!_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} not found.");

            var layoutAction = RaiseDiagramConnectorLayoutAction("RemoveDiagramConnector", diagramConnector);

            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            _relativeLayoutCalculator.OnDiagramConnectorRemoved(layoutPath, layoutAction);
            CalculateAbsolutePositions();
        }

        private LayoutPath CreateLayoutPath(DiagramConnector diagramConnector)
        {
            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            return new LayoutPath(sourceVertex, targetVertex, diagramConnector);
        }

        private void OnLayoutActionExecuted(object sender, ILayoutAction layoutAction)
        {
            RaiseLayoutAction(sender, layoutAction);
        }

        private void CalculateAbsolutePositions()
        {
            var newVertexCenters = _absolutePositionCalculator.CalculateVertexCenters(DiagramDefaults.DefaultLayoutStartingPoint);
            RaiseChangeEvents(newVertexCenters);
            SaveCurrentPositions(newVertexCenters);
        }

        private void SaveCurrentPositions(LayoutVertexToPointMap newVertexCenters)
        {
            _previousVertexCenters = newVertexCenters;
            SaveCurrentLayoutPaths(newVertexCenters);
        }

        private void SaveCurrentLayoutPaths(LayoutVertexToPointMap newVertexCenters)
        {
            _layoutPathToPreviousRouteMap.Clear();

            foreach (var layoutPath in RelativeLayout.LayeredLayoutGraph.Edges)
            {
                var currentRoute = layoutPath.GetRoute(newVertexCenters);
                _layoutPathToPreviousRouteMap.Set(layoutPath, currentRoute);
            }
        }

        private void RaiseChangeEvents(LayoutVertexToPointMap newVertexCenters)
        {
            RaiseEventForMovedDiagramNodes(newVertexCenters);
            RerouteAllPaths(newVertexCenters);
        }

        private void RaiseEventForMovedDiagramNodes(LayoutVertexToPointMap newVertexCenters)
        {
            foreach (var diagramNodeLayoutVertex in newVertexCenters.GetDiagramNodeLayoutVertexMap().Keys)
            {
                var previousCenter = GetVertexCenterOrNull(_previousVertexCenters, diagramNodeLayoutVertex);
                var newCenter = GetVertexCenterOrNull(newVertexCenters, diagramNodeLayoutVertex);

                if (newCenter != null)
                    RaiseMoveDiagramNodeAction(diagramNodeLayoutVertex, Point2D.Empty, newCenter.Value, null);
            }
        }

        private static Point2D? GetVertexCenterOrNull(LayoutVertexToPointMap vertexCenters, LayoutVertexBase vertex)
        {
            return vertexCenters.Contains(vertex)
                ? vertexCenters.Get(vertex)
                : (Point2D?)null;
        }

        private void RerouteAllPaths(LayoutVertexToPointMap newVertexCenters)
        {
            foreach (var layoutPath in RelativeLayout.LayeredLayoutGraph.Edges)
                ReroutePath(newVertexCenters, layoutPath);
        }

        private void ReroutePath(LayoutVertexToPointMap newVertexCenters, LayoutPath layoutPath)
        {
            var previousRoute = _layoutPathToPreviousRouteMap.Get(layoutPath);
            var newRoute = layoutPath.GetRoute(newVertexCenters);

            if (previousRoute != newRoute)
                RaiseReroutePathLayoutAction(layoutPath, previousRoute, newRoute, null);
        }
    }
}
