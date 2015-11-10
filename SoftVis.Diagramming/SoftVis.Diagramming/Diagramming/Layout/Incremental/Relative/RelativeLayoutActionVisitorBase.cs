namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    internal abstract class RelativeLayoutActionVisitorBase : LayoutActionVisitorBase
    {
        public virtual void Visit(RelativeVertexAddLayoutAction layoutAction) { }
        public virtual void Visit(RelativeVertexMoveLayoutAction layoutAction) { }
    }
}
