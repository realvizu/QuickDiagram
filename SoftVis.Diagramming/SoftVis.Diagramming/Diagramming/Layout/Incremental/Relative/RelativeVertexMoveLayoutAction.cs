namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    internal class RelativeVertexMoveLayoutAction : LayoutVertexAction, IRelativeLayoutAction
    {
        public RelativeLocation From { get; }
        public RelativeLocation To { get; }

        public RelativeVertexMoveLayoutAction(LayoutVertexBase vertex, RelativeLocation from, RelativeLocation to,
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
