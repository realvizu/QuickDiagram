using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    internal class LayoutPath : Path<LayoutVertex, LayoutEdge>
    {
        public LayoutPath(LayoutEdge layoutEdge)
            : base(layoutEdge)
        {
        }

        public LayoutPath(IEnumerable<LayoutEdge> layoutEdges)
            : base(layoutEdges)
        {
        }

        public IEnumerable<LayoutVertex> GetInterimVertices()
        {
            return this.Skip(1).Select(i => i.Source);
        }

        public Route GetRoute()
        {
            var sourceRect = Source.Rect;
            var interimRoutePoints = GetInterimVertices().Select(i => i.Center).ToArray();
            var targetRect = Target.Rect;

            var secondPoint = interimRoutePoints.Any()
                ? interimRoutePoints.First()
                : targetRect.Center;
            var penultimatePoint = interimRoutePoints.Any()
                ? interimRoutePoints.Last()
                : sourceRect.Center;

            return new Route
            {
                sourceRect.GetAttachPointToward(secondPoint),
                interimRoutePoints,
                targetRect.GetAttachPointToward(penultimatePoint)
            };
        }
    }
}
