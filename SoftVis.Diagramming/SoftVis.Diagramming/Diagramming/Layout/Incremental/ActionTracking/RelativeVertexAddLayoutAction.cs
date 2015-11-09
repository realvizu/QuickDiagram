using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    internal class RelativeVertexAddLayoutAction : VertexAction, IRelativeVertexLayoutAction
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
