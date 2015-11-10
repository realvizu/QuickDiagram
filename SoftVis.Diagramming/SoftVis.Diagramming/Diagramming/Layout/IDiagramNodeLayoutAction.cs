namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A layout action that affects a diagram node.
    /// </summary>
    public interface IDiagramNodeLayoutAction : ILayoutAction
    {
        DiagramNode DiagramNode { get; }
    }
}