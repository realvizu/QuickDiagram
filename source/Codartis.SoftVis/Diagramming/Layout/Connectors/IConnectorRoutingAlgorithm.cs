using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Layout.Connectors
{
    public interface IConnectorRoutingAlgorithm
    {
        IDictionary<ModelRelationshipId, Route> Calculate(IEnumerable<IDiagramConnector> connectors);
    }
}
