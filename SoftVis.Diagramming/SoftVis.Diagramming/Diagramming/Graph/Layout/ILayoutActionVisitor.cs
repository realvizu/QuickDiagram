namespace Codartis.SoftVis.Diagramming.Graph.Layout
{
    /// <summary>
    /// A visitor that processes different types of layout actions. 
    /// </summary>
    public interface ILayoutActionVisitor
    {
        void Visit(IMoveDiagramNodeLayoutAction layoutAction);
        void Visit(IRerouteDiagramConnectorLayoutAction layoutAction);
    }
}
