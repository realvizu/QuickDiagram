using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute.Logic
{
    /// <summary>
    /// Calculates diagram node positions and connector routes based on their relative layout.
    /// </summary>
    internal class AbsoluteLayoutCalculator : AbsoluteLayoutActionEventSource,
        IRelativeLayoutChangeConsumer
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

        private IReadOnlyLayeredLayoutGraph LayeredLayoutGraph => _relativeLayout.LayeredLayoutGraph;
        private IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph => _relativeLayout.ProperLayeredLayoutGraph;
        private IReadOnlyLayoutVertexLayers Layers => _relativeLayout.LayoutVertexLayers;

        public void OnLayoutCleared()
        {
            _layoutPathToPreviousRouteMap.Clear();
        }

        public void OnVertexAdded(LayoutVertexBase vertex, RelativeLocation newLocation, ILayoutAction causingAction)
        {
            Layers.UpdateLayerVerticalPositions(_verticalGap);

            _vertexPositioningLogic.PositionVertex(vertex, causingAction);
            // TODO: compact sibling-blocks
            _vertexPositioningLogic.Compact(causingAction);
        }

        public void OnVertexRemoved(LayoutVertexBase vertex, RelativeLocation oldLocation, ILayoutAction causingAction)
        {
            Layers.UpdateLayerVerticalPositions(_verticalGap);

            _vertexPositioningLogic.Compact(causingAction);
        }

        public void OnVertexMoved(LayoutVertexBase vertex, RelativeLocation oldLocation, RelativeLocation newLocation, ILayoutAction causingAction)
        {
            OnVertexRemoved(vertex, oldLocation, causingAction);
            OnVertexAdded(vertex, newLocation, causingAction);

            var diagramNodeLayoutVertex = vertex as DiagramNodeLayoutVertex;
            if (diagramNodeLayoutVertex != null)
                RerouteAllAttachedPaths(diagramNodeLayoutVertex, causingAction);
        }

        public void OnPathAdded(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            ReroutePath(layoutPath, causingAction);
        }

        public void OnPathRemoved(LayoutPath layoutPath, ILayoutAction causingAction)
        {
        }

        private void RerouteAllAttachedPaths(DiagramNodeLayoutVertex diagramNodeLayoutVertex, ILayoutAction causingAction)
        {
            foreach (var path in LayeredLayoutGraph.GetAllPaths(diagramNodeLayoutVertex))
                ReroutePath(path, causingAction);
        }

        private void ReroutePath(LayoutPath path, ILayoutAction causingAction)
        {
            if (path.IsFloating)
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