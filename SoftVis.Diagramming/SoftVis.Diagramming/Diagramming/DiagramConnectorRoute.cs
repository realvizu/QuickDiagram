using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Layout;

namespace Codartis.SoftVis.Diagramming
{
    internal class DiagramConnectorRoute
    {
        public EdgeRoutingType EdgeRoutingType { get; }
        public IDictionary<DiagramConnector, Point2D[]> InterimRoutePointsOfEdges { get; }

        public DiagramConnectorRoute(EdgeRoutingType edgeRoutingType,
            IDictionary<DiagramConnector, Point2D[]> interimRoutePointsOfEdges)
        {
            EdgeRoutingType = edgeRoutingType;
            InterimRoutePointsOfEdges = interimRoutePointsOfEdges;
        }
    }
}
