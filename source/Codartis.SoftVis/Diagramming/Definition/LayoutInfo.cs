using System.Collections.Generic;
using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct LayoutInfo
    {
        [NotNull] public readonly IDictionary<ModelNodeId, Rect2D> VertexRects;
        [NotNull] public readonly IDictionary<ModelRelationshipId, Route> EdgeRoutes;

        public LayoutInfo(
            [NotNull] IDictionary<ModelNodeId, Rect2D> vertexRects,
            [NotNull] IDictionary<ModelRelationshipId, Route> edgeRoutes)
        {
            VertexRects = vertexRects;
            EdgeRoutes = edgeRoutes;
        }

        public LayoutInfo([NotNull] IDictionary<ModelRelationshipId, Route> edgeRoutes)
            : this(ImmutableDictionary.Create<ModelNodeId, Rect2D>(), edgeRoutes)
        {
            EdgeRoutes = edgeRoutes;
        }
    }
}