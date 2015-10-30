namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// A layout action that affects a diagram node.
    /// </summary>
    public interface IDiagramNodeAction : ILayoutAction
    {
        DiagramNode DiagramNode { get; }
    }
}