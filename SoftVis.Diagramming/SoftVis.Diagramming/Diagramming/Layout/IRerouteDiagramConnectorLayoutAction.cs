using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A layout action that changes the route of a diagram connector.
    /// </summary>
    public interface IRerouteDiagramConnectorLayoutAction : ILayoutAction
    {
        DiagramConnector DiagramConnector { get; }
        Route OldRoute { get; }
        Route NewRoute { get; }
    }
}