using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Abstract base class for those classes of the incremental layout algorithm that publish layout action events.
    /// </summary>
    internal abstract class IncrementalLayoutActionEventSource : LayoutActionEventSource
    {
        protected ILayoutAction RaiseDiagramNodeLayoutAction(string action, DiagramNode diagramNode,
            ILayoutAction causingAction = null)
        {
            var layoutAction = new DiagramNodeAction(action, diagramNode, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected ILayoutAction RaiseDiagramConnectorLayoutAction(string action, 
            DiagramConnector diagramConnector, ILayoutAction causingAction = null)
        {
            var layoutAction = new DiagramConnectorAction(action, diagramConnector, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

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

        protected LayoutAction RaiseVertexLayoutAction(string action, LayoutVertexBase vertex, 
            ILayoutAction causingAction = null)
        {
            return RaiseVertexLayoutAction(action, vertex, null, causingAction);
        }

        protected LayoutAction RaiseVertexLayoutAction(string action, LayoutVertexBase vertex, 
            double? amount, ILayoutAction causingAction = null)
        {
            var layoutAction = new VertexAction(action, vertex, amount, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected LayoutAction RaiseEdgeLayoutAction(string action, LayoutEdge edge, 
            ILayoutAction causingAction = null)
        {
            var layoutAction = new EdgeAction(action, edge, null, causingAction);
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

        protected LayoutAction RaisePathLayoutAction(string action, LayoutPath path,
            ILayoutAction causingAction = null)
        {
            var layoutAction = new PathAction(action, path, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected LayoutAction RaiseLayoutAction(string action, double? amount = null, 
            ILayoutAction causingAction = null)
        {
            var layoutAction = new LayoutAction(action, amount, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        private void RaiseLayoutAction(ILayoutAction layoutAction)
        {
            RaiseLayoutAction(this, layoutAction);
        }
    }
}