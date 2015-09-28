using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.EdgeRouting
{
    internal class EdgeRoutingSpecification
    {
        public EdgeRoutingType EdgeRoutingType { get; }
        public IDictionary<DiagramConnector, DiagramPoint[]> InterimRoutePointsOfEdges { get; }

        public EdgeRoutingSpecification(EdgeRoutingType edgeRoutingType,
            IDictionary<DiagramConnector, DiagramPoint[]> interimRoutePointsOfEdges)
        {
            EdgeRoutingType = edgeRoutingType;
            InterimRoutePointsOfEdges = interimRoutePointsOfEdges;
        }
    }
}
