namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// An action of a layout logic run that effects an edge.
    /// </summary>
    public interface IEdgeAction : ILayoutAction
    {
        DiagramConnector DiagramConnector { get; }
    }
}