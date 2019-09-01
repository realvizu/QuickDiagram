using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram connector represents a model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// Immutable.
    /// </summary>
    public interface IDiagramConnector : IDiagramShape, 
        IImmutableEdge<ModelNodeId, IDiagramConnector, ModelRelationshipId>
    {
        IModelRelationship ModelRelationship { get; }
        ConnectorType ConnectorType { get; }
        Route Route { get; }

        IDiagramConnector WithModelRelationship(IModelRelationship newModelRelationship);
        IDiagramConnector WithConnectorType(ConnectorType newConnectorType);
        IDiagramConnector WithRoute(Route newRoute);
    }
}
