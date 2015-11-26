using System;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute;
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
    internal sealed class IncrementalLayoutEngine : ILayoutActionEventSource
    {
        public event EventHandler<ILayoutAction> LayoutAction;

        private readonly IReadOnlyDiagramGraph _diagramGraph;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;
        private LayoutVertexToPointMap _previousVertexCenters;

        private readonly RelativeLayoutCalculator _relativeLayoutCalculator;
        private readonly AbsolutePositionCalculator _absolutePositionCalculator;

        public IncrementalLayoutEngine(IReadOnlyDiagramGraph diagramGraph)
        {
            const double horizontalGap = DiagramDefaults.HorizontalGap;
            const double verticalGap = DiagramDefaults.VerticalGap;

            _diagramGraph = diagramGraph;

            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();
            _previousVertexCenters = new LayoutVertexToPointMap();

            _relativeLayoutCalculator = new RelativeLayoutCalculator();
            _absolutePositionCalculator = new AbsolutePositionCalculator(_relativeLayoutCalculator.RelativeLayout, 
                horizontalGap, verticalGap);

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

            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            _relativeLayoutCalculator.OnDiagramNodeAdded(diagramNodeLayoutVertex);
            CalculateAbsolutePositions();
        }

        private void OnDiagramNodeRemoved(DiagramNode diagramNode)
        {
            if (!_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            _relativeLayoutCalculator.OnDiagramNodeRemoved(diagramNodeLayoutVertex);
            CalculateAbsolutePositions();
        }

        private void OnDiagramConnectorAdded(DiagramConnector diagramConnector)
        {
            if (_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            _relativeLayoutCalculator.OnDiagramConnectorAdded(layoutPath);
            CalculateAbsolutePositions();
        }

        private void OnDiagramConnectorRemoved(DiagramConnector diagramConnector)
        {
            if (!_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} not found.");

            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            _relativeLayoutCalculator.OnDiagramConnectorRemoved(layoutPath);
            CalculateAbsolutePositions();
        }

        private LayoutPath CreateLayoutPath(DiagramConnector diagramConnector)
        {
            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            return new LayoutPath(sourceVertex, targetVertex, diagramConnector);
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
                    RaiseMoveDiagramNodeAction(diagramNodeLayoutVertex, Point2D.Empty, newCenter.Value);
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
                RaiseReroutePathLayoutAction(layoutPath, previousRoute, newRoute);
        }

        private void RaiseMoveDiagramNodeAction(DiagramNodeLayoutVertex vertex, Point2D oldCenter, Point2D newCenter)
        {
            var layoutAction = new MoveDiagramNodeLayoutAction(vertex, oldCenter, newCenter);
            RaiseLayoutAction(layoutAction);
        }

        private void RaiseReroutePathLayoutAction(LayoutPath path, Route oldRoute, Route newRoute)
        {
            var layoutAction = new ReroutePathLayoutAction(path, oldRoute, newRoute);
            RaiseLayoutAction(layoutAction);
        }

        private void RaiseLayoutAction(object sender, ILayoutAction layoutAction)
        {
            LayoutAction?.Invoke(sender, layoutAction);
        }

        private void RaiseLayoutAction(ILayoutAction layoutAction)
        {
            RaiseLayoutAction(this, layoutAction);
        }
    }
}
