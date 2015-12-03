namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// An action performed by the layout logic.
    /// </summary>
    public interface ILayoutAction
    {
        DiagramShape DiagramShape { get; }
        void AcceptVisitor(ILayoutActionVisitor visitor);
    }
}