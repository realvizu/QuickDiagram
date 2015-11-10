using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A layout action that changes the route of a diagram connector.
    /// </summary>
    public interface IRerouteDiagramConnectorAction : IDiagramConnectorAction
    {
        Route OldRoute { get; }
        Route NewRoute { get; }
    }
}