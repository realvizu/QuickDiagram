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
    /// <remarks>Interim vertices a dummies.</remarks>
    internal class LayoutPath : Path<LayoutVertexBase, LayoutEdge>
    {
        public LayoutPath(IEnumerable<LayoutEdge> layoutEdges)
            : base(layoutEdges)
        {
        }

        public LayoutPath(LayoutEdge layoutEdge, IEnumerable<DummyLayoutVertex> intermediateVertices)
            :this(layoutEdge.Split(intermediateVertices))
        {
        }

        public DiagramConnector DiagramConnector => this.FirstOrDefault()?.DiagramConnector;
        public int LayerSpan => Source.LayerIndex - Target.LayerIndex;
        public IEnumerable<LayoutVertexBase> Vertices => this.Select(i => i.Source).Concat(this.Last().Target);
        public IEnumerable<DummyLayoutVertex> InterimVertices => this.Skip(1).Select(i => i.Source).OfType<DummyLayoutVertex>();

        public Route GetRoute()
        {
            var sourceRect = Source.Rect;
            var interimRoutePoints = InterimVertices.Select(i => i.Center).ToArray();
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

        public static LayoutPath GetFromOutEdge(LayoutEdge layoutEdge)
        {
            return new LayoutPath(GetEdgesToNextNonDummyVertex(layoutEdge));
        }

        private static IEnumerable<LayoutEdge> GetEdgesToNextNonDummyVertex(LayoutEdge layoutEdge)
        {
            yield return layoutEdge;

            if (layoutEdge.Target is DiagramNodeLayoutVertex)
                yield break;

            var nextEdge = layoutEdge.Target.OutEdges.First();

            foreach (var edge in GetEdgesToNextNonDummyVertex(nextEdge))
                yield return edge;
        }

        public static LayoutPath GetFromInEdge(LayoutEdge layoutEdge)
        {
            return new LayoutPath(GetEdgesFromNextNonDummyVertex(layoutEdge).Reverse());
        }

        private static IEnumerable<LayoutEdge> GetEdgesFromNextNonDummyVertex(LayoutEdge layoutEdge)
        {
            yield return layoutEdge;

            if (layoutEdge.Source is DiagramNodeLayoutVertex)
                yield break;

            var previousEdge = layoutEdge.Source.InEdges.First();

            foreach (var edge in GetEdgesFromNextNonDummyVertex(previousEdge))
                yield return edge;
        }

        public override string ToString()
        {
            return Vertices.ToDelimitedString("->");
        }
    }
}
