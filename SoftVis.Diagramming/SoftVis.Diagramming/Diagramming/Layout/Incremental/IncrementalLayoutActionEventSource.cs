using Codartis.SoftVis.Diagramming.Layout.BaseActions;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Abstract base class for those classes that publish layout action events about LayoutVertex/Edge/Path objects.
    /// </summary>
    internal abstract class IncrementalLayoutActionEventSource : BaseLayoutActionEventSource
    {
        protected LayoutAction RaiseVertexLayoutAction(string action, LayoutVertexBase vertex, 
            ILayoutAction causingAction = null)
        {
            return RaiseVertexLayoutAction(action, vertex, null, causingAction);
        }

        protected LayoutAction RaiseVertexLayoutAction(string action, LayoutVertexBase vertex, double? amount, 
            ILayoutAction causingAction = null)
        {
            var layoutAction = new LayoutVertexAction(action, vertex, amount, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected LayoutAction RaiseEdgeLayoutAction(string action, LayoutEdge edge, ILayoutAction causingAction = null)
        {
            var layoutAction = new LayoutEdgeAction(action, edge, null, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected LayoutAction RaisePathLayoutAction(string action, LayoutPath path, ILayoutAction causingAction = null)
        {
            var layoutAction = new LayoutPathAction(action, path, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }
    }
}