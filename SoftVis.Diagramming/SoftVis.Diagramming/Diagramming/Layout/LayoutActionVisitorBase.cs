namespace Codartis.SoftVis.Diagramming.Layout
{
    public abstract class LayoutActionVisitorBase
    {
        public virtual void DefaultVisit(ILayoutAction layoutAction) { }
        public virtual void Visit(IMoveDiagramNodeAction layoutAction) { }
        public virtual void Visit(IRerouteDiagramConnectorAction layoutAction) { }
    }
}
