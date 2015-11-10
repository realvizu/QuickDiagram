using Codartis.SoftVis.Diagramming.Layout.BaseActions;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute
{
    /// <summary>
    /// Abstract base class for those classes that publish absolute layout action events.
    /// </summary>
    internal abstract class AbsoluteLayoutActionEventSource : IncrementalLayoutActionEventSource
    {
        protected ILayoutAction RaiseVertexMoveLayoutAction(LayoutVertexBase vertex, 
            Point2D oldCenter, Point2D newCenter, ILayoutAction causingAction = null)
        {
            return vertex is DummyLayoutVertex
                ? RaiseMoveDummyVertexAction((DummyLayoutVertex)vertex, oldCenter, newCenter, causingAction)
                : RaiseMoveDiagramNodeAction((DiagramNodeLayoutVertex)vertex, oldCenter, newCenter, causingAction);
        }

        private ILayoutAction RaiseMoveDiagramNodeAction(DiagramNodeLayoutVertex vertex, 
            Point2D oldCenter, Point2D newCenter, ILayoutAction causingAction)
        {
            var layoutAction = new MoveDiagramNodeAction(vertex, oldCenter, newCenter, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        private ILayoutAction RaiseMoveDummyVertexAction(DummyLayoutVertex vertex, 
            Point2D oldCenter, Point2D newCenter, ILayoutAction causingAction)
        {
            var layoutAction = new MoveDummyVertexAction(vertex, oldCenter, newCenter, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected LayoutAction RaisePathLayoutAction(string action, LayoutPath path,
            Route oldRoute, Route newRoute, ILayoutAction causingAction = null)
        {
            var layoutAction = new ReroutePathAction(action, path, oldRoute, newRoute, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

    }
}