using Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Abstract base class for classes the implement logic for the incremental layout engine.
    /// </summary>
    /// <remarks>
    /// Contains action tracking helper functions.
    /// </remarks>
    internal abstract class LayoutLogicBase
    {
        protected readonly double HorizontalGap;
        protected readonly double VerticalGap;
        protected readonly LayoutGraph LayoutGraph;
        protected readonly LayoutActionGraph LayoutActionGraph;

        protected LayoutLogicBase(double horizontalGap, double verticalGap, LayoutGraph layoutGraph, LayoutActionGraph layoutActionGraph)
        {
            HorizontalGap = horizontalGap;
            VerticalGap = verticalGap;
            LayoutGraph = layoutGraph;
            LayoutActionGraph = layoutActionGraph;
        }

        protected LayoutAction RecordVertexMoveAction(LayoutVertexBase layoutVertex, Point2D oldCenter, Point2D newCenter, LayoutAction previousAction = null)
        {
            var vertexMove = new VertexMoveAction(layoutVertex, oldCenter, newCenter);
            RecordAction(vertexMove, previousAction);
            return vertexMove;
        }

        protected LayoutAction RecordVertexAction(string action, LayoutVertexBase layoutVertex, LayoutAction previousAction = null)
        {
            return RecordVertexAction(action, layoutVertex, null, previousAction);
        }

        protected LayoutAction RecordVertexAction(string action, LayoutVertexBase layoutVertex, double? amount, LayoutAction previousAction = null)
        {
            var layoutAction = new VertexAction(action, layoutVertex, amount);
            RecordAction(layoutAction, previousAction);
            return layoutAction;
        }

        protected LayoutAction RecordEdgeAction(string action, LayoutEdge layoutEdge, LayoutAction previousAction = null)
        {
            var layoutAction = new EdgeAction(action, layoutEdge);
            RecordAction(layoutAction, previousAction);
            return layoutAction;
        }

        protected LayoutAction RecordPathAction(string action, LayoutPath layoutPath, LayoutAction previousAction = null)
        {
            var layoutAction = new PathAction(action, layoutPath);
            RecordAction(layoutAction, previousAction);
            return layoutAction;
        }

        protected LayoutAction RecordAction(string action, double? amount = null, LayoutAction previousAction = null)
        {
            var layoutAction = new LayoutAction(action, amount);
            RecordAction(layoutAction, previousAction);
            return layoutAction;
        }

        private void RecordAction(LayoutAction layoutAction, LayoutAction previousAction = null)
        {
            if (!LayoutActionGraph.ContainsVertex(layoutAction))
                LayoutActionGraph.AddVertex(layoutAction);

            if (previousAction != null)
            {
                var edge = new LayoutActionEdge(previousAction, layoutAction);
                LayoutActionGraph.AddEdge(edge);
            }
        }
    }
}