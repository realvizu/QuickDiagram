using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama.Absolute;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama.Relative;
using Codartis.SoftVis.Geometry;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A stateful layout calculator that gets diagram shape actions and calculates layout actions.
    /// Not thread-safe!
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
    internal sealed class IncrementalLayoutCalculator : IIncrementalLayoutCalculator, IDiagramActionConsumer
    {
        private readonly ILayoutPriorityProvider _layoutPriorityProvider;
        private readonly Map<IDiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<IDiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;
        private LayoutVertexToPointMap _previousVertexCenters;
        private readonly StatefulRelativeLayoutCalculator _statefulRelativeLayoutCalculator;
        private readonly DiagramActionDispatcherVisitor _diagramActionDispatcherVisitor;

        private const double HorizontalGap = DiagramLayoutDefaults.HorizontalGap;
        private const double VerticalGap = DiagramLayoutDefaults.VerticalGap;

        public IncrementalLayoutCalculator(ILayoutPriorityProvider layoutPriorityProvider)
        {
            _layoutPriorityProvider = layoutPriorityProvider ?? throw new ArgumentNullException(nameof(layoutPriorityProvider));
            _diagramNodeToLayoutVertexMap = new Map<IDiagramNode, DiagramNodeLayoutVertex>(new DiagramNodeIdEqualityComparer());
            _diagramConnectorToLayoutPathMap = new Map<IDiagramConnector, LayoutPath>(new DiagramConnectorIdEqualityComparer());
            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();
            _previousVertexCenters = new LayoutVertexToPointMap();
            _statefulRelativeLayoutCalculator = new StatefulRelativeLayoutCalculator();
            _diagramActionDispatcherVisitor = new DiagramActionDispatcherVisitor(this);
        }

        private IReadOnlyRelativeLayout RelativeLayout => _statefulRelativeLayoutCalculator.RelativeLayout;

        public void Clear()
        {
            _statefulRelativeLayoutCalculator.OnDiagramCleared();

            _layoutPathToPreviousRouteMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
            _diagramNodeToLayoutVertexMap.Clear();
            _previousVertexCenters.Clear();
        }

        public IEnumerable<ILayoutAction> CalculateLayoutActions(IEnumerable<DiagramAction> diagramActions)
        {
            foreach (var diagramAction in diagramActions)
                diagramAction.Accept(_diagramActionDispatcherVisitor);

            return CalculateAbsoluteLayout();
        }

        public void AddDiagramNode(IDiagramNode diagramNode)
        {
            if (_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var diagramNodeLayoutPriority = _layoutPriorityProvider.GetPriority(diagramNode);
            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode, diagramNode.Name, diagramNodeLayoutPriority);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            _statefulRelativeLayoutCalculator.OnDiagramNodeAdded(diagramNodeLayoutVertex);
        }

        public void RemoveDiagramNode(IDiagramNode diagramNode)
        {
            if (!_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            _statefulRelativeLayoutCalculator.OnDiagramNodeRemoved(diagramNodeLayoutVertex);
        }

        public void ResizeDiagramNode(IDiagramNode diagramNode, Size2D newSize)
        {
            if (!_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            diagramNodeLayoutVertex.Resize(newSize);
        }

        public void AddDiagramConnector(IDiagramConnector diagramConnector)
        {
            if (_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            _statefulRelativeLayoutCalculator.OnDiagramConnectorAdded(layoutPath);
        }

        public void RemoveDiagramConnector(IDiagramConnector diagramConnector)
        {
            if (!_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} not found.");

            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            _statefulRelativeLayoutCalculator.OnDiagramConnectorRemoved(layoutPath);
        }

        public void ClearDiagram() => Clear();

        private LayoutPath CreateLayoutPath(IDiagramConnector diagramConnector)
        {
            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            return new LayoutPath(sourceVertex, targetVertex, diagramConnector);
        }

        private IEnumerable<ILayoutAction> CalculateAbsoluteLayout()
        {
            var newVertexCenters = AbsolutePositionCalculator.GetVertexCenters(
                _statefulRelativeLayoutCalculator.RelativeLayout, HorizontalGap, VerticalGap);

            var layoutActions = CreateLayoutActions(newVertexCenters);

            SaveCurrentPositions(newVertexCenters);
            return layoutActions;
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
                var currentRoute = GetRoutePoints(layoutPath, newVertexCenters);
                _layoutPathToPreviousRouteMap.Set(layoutPath, currentRoute);
            }
        }

        private List<ILayoutAction> CreateLayoutActions(LayoutVertexToPointMap newVertexCenters)
        {
            var layoutActions = new List<ILayoutAction>();
            layoutActions.AddRange(CreateLayoutActionsForMovedDiagramNodes(newVertexCenters));
            layoutActions.AddRange(CreateLayoutActionsForRerouteAllPaths(newVertexCenters));
            return layoutActions;
        }

        private IEnumerable<ILayoutAction> CreateLayoutActionsForMovedDiagramNodes(LayoutVertexToPointMap newVertexCenters)
        {
            foreach (var diagramNodeLayoutVertex in RelativeLayout.LayeredLayoutGraph.Vertices)
            {
                var oldCenter = GetVertexCenterOrUndefined(_previousVertexCenters, diagramNodeLayoutVertex);
                var newCenter = GetVertexCenterOrUndefined(newVertexCenters, diagramNodeLayoutVertex);

                if (newCenter.IsDefined && newCenter != oldCenter)
                    yield return new MoveDiagramNodeLayoutAction(diagramNodeLayoutVertex, oldCenter, newCenter);
            }
        }

        private IEnumerable<ILayoutAction> CreateLayoutActionsForRerouteAllPaths(LayoutVertexToPointMap newVertexCenters)
        {
            foreach (var layoutPath in RelativeLayout.LayeredLayoutGraph.Edges)
            {
                _layoutPathToPreviousRouteMap.TryGet(layoutPath, out var oldRoute, valueForMissingKey: Route.Empty);
                var newRoute = GetRoutePoints(layoutPath, newVertexCenters);

                if (newRoute.IsDefined && newRoute != oldRoute)
                    yield return new ReroutePathLayoutAction(layoutPath, oldRoute, newRoute);
            }
        }

        private static Route GetRoutePoints(LayoutPath layoutPath, LayoutVertexToPointMap vertexCenters)
        {
            var sourceRect = vertexCenters.GetRect(layoutPath.PathSource);
            var targetRect = vertexCenters.GetRect(layoutPath.PathTarget);

            var routePoints = new Route.Builder
            { 
                sourceRect.Center,
                layoutPath.InterimVertices.Select(vertexCenters.Get),
                targetRect.Center
            }.ToRoute();

            return routePoints.AttachToSourceRectAndTargetRect(sourceRect, targetRect);
        }

        private static Point2D GetVertexCenterOrUndefined(LayoutVertexToPointMap vertexCenters, LayoutVertexBase vertex)
        {
            vertexCenters.TryGet(vertex, out var center, valueForMissingKey: Point2D.Undefined);
            return center;
        }
    }
}
