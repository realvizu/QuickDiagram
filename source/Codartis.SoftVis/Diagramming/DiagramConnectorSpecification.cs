using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// The information required to create a diagram connector.
    /// </summary>
    public struct DiagramConnectorSpecification
    {
        public IModelRelationship ModelRelationship { get; }
        public ConnectorType ConnectorType { get; }
        public Route Route { get; }

        public DiagramConnectorSpecification(IModelRelationship modelRelationship, ConnectorType connectorType)
            : this(modelRelationship, connectorType, Route.Empty)
        {
        }

        public DiagramConnectorSpecification(IModelRelationship modelRelationship, ConnectorType connectorType, Route route)
        {
            ModelRelationship = modelRelationship;
            ConnectorType = connectorType;
            Route = route;
        }
    }
}