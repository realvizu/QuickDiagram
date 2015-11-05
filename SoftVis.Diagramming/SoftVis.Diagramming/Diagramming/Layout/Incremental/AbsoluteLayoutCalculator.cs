using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates vertex positions based on their relative arrangement in LayoutGraph and LayoutVertexLayers.
    /// </summary>
    internal class AbsoluteLayoutCalculator : IncrementalLayoutActionEventSource
    {
        private readonly IReadOnlyDiagramGraph _diagramGraph;
        private readonly IReadOnlyLayoutGraph _layoutGraph;
        private readonly IReadOnlyLayoutVertexLayers _layers;
        private readonly double _horizontalGap;
        private readonly double _verticalGap;

        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;
        private readonly VertexPositioningLogic _vertexPositioningLogic;

        public AbsoluteLayoutCalculator(IReadOnlyDiagramGraph diagramGraph,
            IReadOnlyLayoutGraph layoutGraph, IReadOnlyLayoutVertexLayers layers,
            double horizontalGap, double verticalGap)
        {
            _diagramGraph = diagramGraph;
            _layoutGraph = layoutGraph;
            _layers = layers;
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();

            _vertexPositioningLogic = new VertexPositioningLogic(horizontalGap, verticalGap,
                _layoutGraph, _layers);
            _vertexPositioningLogic.LayoutActionExecuted += RaiseLayoutAction;
        }

        public void Clear()
        {
            _layoutPathToPreviousRouteMap.Clear();
        }

        public void Add(DiagramNodeLayoutVertex diagramNodeVertex)
        {
            var layoutAction = RaiseVertexLayoutAction("Absolute.AddVertex", diagramNodeVertex);

            _layers.UpdateLayerVerticalPositions(_verticalGap);

            _vertexPositioningLogic.PositionVertex(diagramNodeVertex, layoutAction);
            _vertexPositioningLogic.Compact(layoutAction);
        }

        public void Remove(DiagramNodeLayoutVertex diagramNodeVertex)
        {
            var layoutAction = RaiseVertexLayoutAction("Absolute.RemoveNode", diagramNodeVertex);

            // TODO: instead of this, compact all sibling-blocks?
            // (do not rely on info that was available only before deleting from the relative view)
            //_vertexPositioningLogic.CoverUpVertex(layoutVertex, layoutAction);

            _vertexPositioningLogic.Compact(layoutAction);
        }

        public void Add(LayoutPath layoutPath)
        {
            var layoutAction = RaisePathLayoutAction("Absolute.AddPath", layoutPath);

            _layers.UpdateLayerVerticalPositions(_verticalGap);

            RepositionVerticesOnInEdgesRecursive(layoutPath.Last(), layoutAction);
            // TODO: compact all sibling-blocks?
            _vertexPositioningLogic.Compact(layoutAction);
        }

        public void Remove(LayoutPath layoutPath)
        {
            var layoutAction = RaisePathLayoutAction("Absolute.RemovePath", layoutPath);

            //TODO: ???
            //foreach (var interimVertex in layoutPath.InterimVertices)
            //    _vertexPositioningLogic.CoverUpVertex(interimVertex, layoutAction);
            //_vertexPositioningLogic.CenterPrimaryParent(positioningEdgePath.Source, layoutAction);
            //RepositionVerticesOnInEdgesRecursive(diagramConnector.Source, layoutAction);

            _vertexPositioningLogic.Compact(layoutAction);
        }

        private void RepositionVerticesOnInEdgesRecursive(LayoutEdge edge, ILayoutAction causingAction)
        {
            _layoutGraph.ExecuteOnEdgesRecursive(edge, EdgeDirection.In,
                i => _vertexPositioningLogic.PositionVertex(edge.Source, causingAction, edge.Target));
        }
    }
}