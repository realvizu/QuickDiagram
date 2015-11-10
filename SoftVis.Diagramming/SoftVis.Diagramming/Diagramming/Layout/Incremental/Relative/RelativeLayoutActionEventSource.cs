namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Abstract base class for those classes that publish relative layout action events.
    /// </summary>
    internal abstract class RelativeLayoutActionEventSource : IncrementalLayoutActionEventSource
    {
        protected void RaiseRelativeLocationAssignedLayoutAction(LayoutVertexBase vertex,
            RelativeLocation to, ILayoutAction causingAction)
        {
            var layoutAction = new RelativeLocationAssignedLayoutAction(vertex, to, causingAction);
            RaiseLayoutAction(this, layoutAction);
        }

        protected void RaiseRelativeLocationChangedLayoutAction(LayoutVertexBase vertex, 
            RelativeLocation from, RelativeLocation to, ILayoutAction causingAction)
        {
            var layoutAction = new RelativeLocationChangedLayoutAction(vertex, from, to, causingAction);
            RaiseLayoutAction(this, layoutAction);
        }

    }
}