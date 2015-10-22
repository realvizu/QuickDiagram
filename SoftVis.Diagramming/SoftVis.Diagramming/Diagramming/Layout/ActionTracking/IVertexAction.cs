namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// An action of a layout logic run that effects a vertex.
    /// </summary>
    public interface IVertexAction : ILayoutAction
    {
        DiagramNode DiagramNode { get; }
    }
}