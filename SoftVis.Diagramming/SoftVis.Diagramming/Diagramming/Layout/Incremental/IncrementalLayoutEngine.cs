using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A stateful layout calculator that gets diagram shape actions and calculates layout actions.
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
    internal sealed class IncrementalLayoutEngine : IIncrementalLayoutEngine, IDiagramActionConsumer
    {
        private readonly Map<IDiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<IDiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Map<LayoutPath, Route> _layoutPathToPreviousInterimRouteMap;
        private LayoutVertexToPointMap _previousVertexCenters;
        private readonly RelativeLayoutCalculator _relativeLayoutCalculator;
        private readonly DiagramActionDispatcherVisitor _diagramActionDispatcherVisitor;

        private const double HorizontalGap = DiagramDefaults.HorizontalGap;
        private const double VerticalGap = DiagramDefaults.VerticalGap;

        public IncrementalLayoutEngine()
        {
            _diagramNodeToLayoutVertexMap = new Map<IDiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<IDiagramConnector, LayoutPath>();
            _layoutPathToPreviousInterimRouteMap = new Map<LayoutPath, Route>();
            _previousVertexCenters = new LayoutVertexToPointMap();
            _relativeLayoutCalculator = new RelativeLayoutCalculator();
            _diagramActionDispatcherVisitor = new DiagramActionDispatcherVisitor(this);
        }

        private IReadOnlyRelativeLayout RelativeLayout => _relativeLayoutCalculator.RelativeLayout;

        public void Clear()
        {
            _relativeLayoutCalculator.OnDiagramCleared();

            _layoutPathToPreviousInterimRouteMap.Clear();
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

            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            _relativeLayoutCalculator.OnDiagramNodeAdded(diagramNodeLayoutVertex);
        }

        public void RemoveDiagramNode(IDiagramNode diagramNode)
        {
            if (!_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            _relativeLayoutCalculator.OnDiagramNodeRemoved(diagramNodeLayoutVertex);
        }

        public void ResizeDiagramNode(IDiagramNode diagramNode)
        {
            if (!_diagramNodeToLayoutVertexMap.Contains(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            diagramNodeLayoutVertex.Resize(diagramNode.Size);
        }

        public void AddDiagramConnector(IDiagramConnector diagramConnector)
        {
            if (_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            _relativeLayoutCalculator.OnDiagramConnectorAdded(layoutPath);
        }

        public void RemoveDiagramConnector(IDiagramConnector diagramConnector)
        {
            if (!_diagramConnectorToLayoutPathMap.Contains(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} not found.");

            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            _relativeLayoutCalculator.OnDiagramConnectorRemoved(layoutPath);
        }

        private LayoutPath CreateLayoutPath(IDiagramConnector diagramConnector)
        {
            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            return new LayoutPath(sourceVertex, targetVertex, diagramConnector);
        }

        private IEnumerable<ILayoutAction> CalculateAbsoluteLayout()
        {
            var newVertexCenters = AbsolutePositionCalculator.GetVertexCenters(
                _relativeLayoutCalculator.RelativeLayout, HorizontalGap, VerticalGap);

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
            _layoutPathToPreviousInterimRouteMap.Clear();

            foreach (var layoutPath in RelativeLayout.LayeredLayoutGraph.Edges)
            {
                var currentInterimRoute = GetInterimPoints(layoutPath, newVertexCenters);
                _layoutPathToPreviousInterimRouteMap.Set(layoutPath, currentInterimRoute);
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
                var oldInterimRoute = _layoutPathToPreviousInterimRouteMap.Get(layoutPath);
                var newInterimRoute = GetInterimPoints(layoutPath, newVertexCenters);

                // Layout actions must be created for undefined interim routes too
                // because the connector can exist even if its interim points have disappeared.
                if (newInterimRoute != oldInterimRoute)
                    yield return new ReroutePathLayoutAction(layoutPath, oldInterimRoute, newInterimRoute);
            }
        }

        private static Route GetInterimPoints(LayoutPath layoutPath, LayoutVertexToPointMap vertexCenters)
        {
            var interimPoints = layoutPath.InterimVertices.Select(vertexCenters.Get);
            return new Route(interimPoints);
        }

        private static Point2D GetVertexCenterOrUndefined(LayoutVertexToPointMap vertexCenters, LayoutVertexBase vertex)
        {
            return vertexCenters.Contains(vertex)
                ? vertexCenters.Get(vertex)
                : Point2D.Undefined;
        }
    }
}
