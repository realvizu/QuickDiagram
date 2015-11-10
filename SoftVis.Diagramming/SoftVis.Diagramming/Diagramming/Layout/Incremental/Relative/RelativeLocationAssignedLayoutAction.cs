namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// A layout action that assigns a relative location to a vertex.
    /// </summary>
    internal class RelativeLocationAssignedLayoutAction : LayoutVertexAction, IRelativeLayoutAction
    {
        public RelativeLocation To { get; }

        public RelativeLocationAssignedLayoutAction(LayoutVertexBase vertex, RelativeLocation to,
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
