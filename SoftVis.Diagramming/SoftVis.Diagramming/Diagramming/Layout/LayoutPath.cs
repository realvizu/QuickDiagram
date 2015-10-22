using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A list of LayoutEdges that form a path,
    /// that is the target of an edge is the source of the next edge in the path.
    /// </summary>
    internal class LayoutPath : Path<LayoutVertexBase, LayoutEdge>
    {
        public LayoutPath(LayoutEdge layoutEdge)
            : base(layoutEdge)
        {
        }

        public LayoutPath(IEnumerable<LayoutEdge> layoutEdges)
            : base(layoutEdges)
        {
        }

        public LayoutPath(LayoutEdge layoutEdge, IEnumerable<LayoutVertexBase> intermediateVertices)
            :this(layoutEdge.Split(intermediateVertices))
        {
        }

        public IEnumerable<LayoutVertexBase> GetVertices() => this.Select(i => i.Source).Concat(this.Last().Target);
        public IEnumerable<DummyLayoutVertex> GetInterimVertices() => this.Skip(1).Select(i => i.Source).OfType<DummyLayoutVertex>();

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

        public override string ToString()
        {
            return GetVertices().ToDelimitedString("->");
        }
    }
}
