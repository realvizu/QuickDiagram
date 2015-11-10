namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// A layout action that changes the relative location of a vertex.
    /// </summary>
    internal class RelativeLocationChangedLayoutAction : LayoutVertexAction, IRelativeLayoutAction
    {
        public RelativeLocation From { get; }
        public RelativeLocation To { get; }

        public RelativeLocationChangedLayoutAction(LayoutVertexBase vertex, RelativeLocation from, RelativeLocation to,
            ILayoutAction causingLayoutAction = null)
            : base("RelativeVertexMove", vertex, null, causingLayoutAction)
        {
            From = from;
            To = to;
        }

        public void AcceptVisitor(RelativeLayoutActionVisitorBase visitor)
        {
            visitor.Visit(this);
        }
    }
}
