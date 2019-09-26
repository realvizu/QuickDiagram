using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct DiagramLayoutInfo
    {
        [NotNull] public IImmutableDictionary<ModelNodeId, Point2D> NodeTopLeftPositions { get; }
        [NotNull] public IImmutableDictionary<ModelRelationshipId, Route> ConnectorRoutes { get; }

        public DiagramLayoutInfo(
            [NotNull] IImmutableDictionary<ModelNodeId, Point2D> nodeTopLeftPositions,
            [NotNull] IImmutableDictionary<ModelRelationshipId, Route> connectorRoutes)
        {
            NodeTopLeftPositions = nodeTopLeftPositions;
            ConnectorRoutes = connectorRoutes;
        }
    }
}