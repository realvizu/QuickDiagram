using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    internal class RelativeVertexMoveLayoutAction : VertexAction, IRelativeVertexLayoutAction
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
