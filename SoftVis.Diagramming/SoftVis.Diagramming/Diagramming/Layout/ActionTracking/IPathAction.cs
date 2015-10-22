namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// An action of a layout logic run that effects a path (a series of edges).
    /// </summary>
    public interface IPathAction : ILayoutAction
    {
        DiagramConnector DiagramConnector { get; }
    }
}