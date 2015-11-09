using Codartis.SoftVis.Diagramming.Layout.ActionTracking;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    internal abstract class RelativeLayoutActionVisitorBase : LayoutActionVisitorBase
    {
        public virtual void Visit(RelativeVertexAddLayoutAction layoutAction) { }
        public virtual void Visit(RelativeVertexMoveLayoutAction layoutAction) { }
    }
}
