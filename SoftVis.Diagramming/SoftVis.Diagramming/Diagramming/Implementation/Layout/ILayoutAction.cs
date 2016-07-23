namespace Codartis.SoftVis.Diagramming.Implementation.Layout
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