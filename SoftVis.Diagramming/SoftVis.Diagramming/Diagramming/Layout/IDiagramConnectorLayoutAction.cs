namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A layout action that affects a diagram connector.
    /// </summary>
    public interface IDiagramConnectorLayoutAction : ILayoutAction
    {
        DiagramConnector DiagramConnector { get; }
    }
}