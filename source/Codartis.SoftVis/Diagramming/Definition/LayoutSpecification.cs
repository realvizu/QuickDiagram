using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct LayoutSpecification
    {
        [NotNull] public IDictionary<ModelNodeId, Point2D> NodeTopLeftPositions { get; }
        [NotNull] public IDictionary<ModelRelationshipId, Route> ConnectorRoutes { get; }

        public LayoutSpecification(
            [NotNull] IDictionary<ModelNodeId, Point2D> nodeTopLeftPositions,
            [NotNull] IDictionary<ModelRelationshipId, Route> connectorRoutes)
        {
            NodeTopLeftPositions = nodeTopLeftPositions;
            ConnectorRoutes = connectorRoutes;
        }
    }
}