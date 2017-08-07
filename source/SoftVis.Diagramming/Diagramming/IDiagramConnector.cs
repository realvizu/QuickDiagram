using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram connector represents a model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// </summary>
    public interface IDiagramConnector : IDiagramShape
    {
        IDiagramNode Source { get; }
        IDiagramNode Target { get; }
        Route RoutePoints { get; set; }
        string RelationshipType { get; }
        ConnectorType ConnectorType { get; }

        event Action<IDiagramConnector, Route, Route> RouteChanged;
    }
}
