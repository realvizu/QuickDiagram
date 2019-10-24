using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public sealed class TestConnectorRoutingAlgorithm : IConnectorRoutingAlgorithm
    {
        public IDictionary<ModelRelationshipId, Route> Calculate(
            IEnumerable<IDiagramConnector> connectors,
            IDictionary<ModelNodeId, Rect2D> nodeRects)
        {
            return new Dictionary<ModelRelationshipId, Route>();
        }
    }
}