using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
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
    }
}