namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Abstract base class for those classes that publish relative layout action events.
    /// </summary>
    internal abstract class RelativeLayoutActionEventSource : IncrementalLayoutActionEventSource
    {
        protected void RaiseVertexAddLayoutAction(LayoutVertexBase vertex,
            RelativeLocation to, ILayoutAction causingAction)
        {
            var layoutAction = new RelativeVertexAddLayoutAction(vertex, to, causingAction);
            RaiseLayoutAction(this, layoutAction);
        }

        protected void RaiseVertexMoveLayoutAction(LayoutVertexBase vertex, 
            RelativeLocation from, RelativeLocation to, ILayoutAction causingAction)
        {
            var layoutAction = new RelativeVertexMoveLayoutAction(vertex, from, to, causingAction);
            RaiseLayoutAction(this, layoutAction);
        }

    }
}