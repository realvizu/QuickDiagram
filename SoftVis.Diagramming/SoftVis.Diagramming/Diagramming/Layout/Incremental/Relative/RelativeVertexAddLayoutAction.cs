namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    internal class RelativeVertexAddLayoutAction : LayoutVertexAction, IRelativeLayoutAction
    {
        public RelativeLocation To { get; }

        public RelativeVertexAddLayoutAction(LayoutVertexBase vertex, RelativeLocation to,
            ILayoutAction causingLayoutAction = null)
            : base("RelativeVertexAdd", vertex, null, causingLayoutAction)
        {
            To = to;
        }

        public void AcceptVisitor(RelativeLayoutActionVisitorBase visitor)
        {
            visitor.Visit(this);
        }
    }
}
