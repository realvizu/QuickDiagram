using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct LayoutSpecification
    {
        [NotNull] public ImmutableDictionary<ModelNodeId, Point2D> NodeTopLeftPositions { get; }
        [NotNull] public ImmutableDictionary<ModelRelationshipId, Route> ConnectorRoutes { get; }

        public LayoutSpecification(
            [NotNull] ImmutableDictionary<ModelNodeId, Point2D> nodeTopLeftPositions,
            [NotNull] ImmutableDictionary<ModelRelationshipId, Route> connectorRoutes)
        {
            NodeTopLeftPositions = nodeTopLeftPositions;
            ConnectorRoutes = connectorRoutes;
        }
    }
}