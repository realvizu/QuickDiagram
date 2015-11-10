namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Abstract base for visitors that process layout actions. 
    /// </summary>
    public abstract class LayoutActionVisitorBase
    {
        public virtual void DefaultVisit(ILayoutAction layoutAction) { }
        public virtual void Visit(IMoveDiagramNodeLayoutAction layoutAction) { }
        public virtual void Visit(IRerouteDiagramConnectorLayoutAction layoutAction) { }
    }
}
