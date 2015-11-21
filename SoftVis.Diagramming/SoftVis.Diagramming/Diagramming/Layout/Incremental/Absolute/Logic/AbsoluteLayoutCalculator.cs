using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute.Logic
{
    /// <summary>
    /// Calculates diagram node positions and connector routes based on their relative layout.
    /// </summary>
    internal class AbsoluteLayoutCalculator : AbsoluteLayoutActionEventSource
    {
        private readonly IReadOnlyRelativeLayout _relativeLayout;
        private readonly double _horizontalGap;
        private readonly double _verticalGap;

        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;
        private readonly VertexPositioningLogic _vertexPositioningLogic;

        public AbsoluteLayoutCalculator(IReadOnlyRelativeLayout relativeLayout, double horizontalGap, double verticalGap)
        {
            _relativeLayout = relativeLayout;
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();

            _vertexPositioningLogic = new VertexPositioningLogic(_relativeLayout, horizontalGap, verticalGap);
            _vertexPositioningLogic.LayoutActionExecuted += RaiseLayoutAction;
        }

        private IReadOnlyLayeredLayoutGraph LayoutGraph => _relativeLayout.LayeredLayoutGraph;
        private IReadOnlyQuasiProperLayoutGraph ProperLayoutGraph => _relativeLayout.ProperLayeredLayoutGraph;
        private IReadOnlyLayoutVertexLayers Layers => _relativeLayout.LayoutVertexLayers;

        public void OnLayoutCleared()
        {
            _layoutPathToPreviousRouteMap.Clear();
        }

        public void OnDiagramNodeAdded(DiagramNodeLayoutVertex diagramNodeLayoutVertex, ILayoutAction causingAction)
        {
            Layers.UpdateLayerVerticalPositions(_verticalGap);

            _vertexPositioningLogic.PositionVertex(diagramNodeLayoutVertex, causingAction);
            // TODO: compact sibling-blocks
            _vertexPositioningLogic.Compact(causingAction);

            RerouteAllPaths(causingAction);
        }

        public void OnDiagramNodeRemoved(DiagramNodeLayoutVertex diagramNodeLayoutVertex, ILayoutAction causingAction)
        {
            Layers.UpdateLayerVerticalPositions(_verticalGap);

            _vertexPositioningLogic.Compact(causingAction);

            RerouteAllPaths(causingAction);
        }

        public void OnDiagramConnectorAdded(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            Layers.UpdateLayerVerticalPositions(_verticalGap);

            var affectedVertices = GetAffectedVertices(layoutPath).OrderBy(ProperLayoutGraph.GetLayerIndex).ToList();

            foreach (var vertex in affectedVertices)
                _vertexPositioningLogic.PositionVertex(vertex, causingAction);

            RerouteAllPaths(causingAction);
        }

        public void OnDiagramConnectorRemoved(LayoutPath layoutPath, ILayoutAction causingAction)
        {
        }

        private IEnumerable<LayoutVertexBase> GetAffectedVertices(LayoutPath layoutPath)
        {
            var diagramNodeVertices = LayoutGraph.GetVertexAndDescendants(layoutPath.PathSource).ToList();
            var dummyVertices = diagramNodeVertices.SelectMany(i => LayoutGraph.OutEdges(i))
                .SelectMany(i => i.InterimVertices);
            return diagramNodeVertices.Concat((IEnumerable<LayoutVertexBase>)dummyVertices);
        }

        private void RerouteAllPaths(ILayoutAction causingAction)
        {
            foreach (var layoutPath in LayoutGraph.Edges)
                ReroutePath(layoutPath, causingAction);
        }

        private void ReroutePath(LayoutPath path, ILayoutAction causingAction)
        {
            //if (path.IsFloating)
            //    return;

            if (path.Vertices.Any(i => i.Center == Point2D.Empty))
                return;

            var oldRoute = _layoutPathToPreviousRouteMap.Get(path);
            var newRoute = path.GetRoute();
            if (oldRoute == newRoute)
                return;

            _layoutPathToPreviousRouteMap.Set(path, newRoute);
            RaisePathLayoutAction("Reroute", path, oldRoute, newRoute, causingAction);
        }
    }
}