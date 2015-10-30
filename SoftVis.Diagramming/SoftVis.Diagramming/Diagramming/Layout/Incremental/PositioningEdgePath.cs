using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A list of PositioningEdges that form a path.
    /// That is, the target of an edge is the source of the next edge in the path.
    /// </summary>
    /// <remarks>Interim vertices are dummies.</remarks>
    internal class PositioningEdgePath : Path<PositioningVertexBase, PositioningEdge>
    {
        public PositioningEdgePath(PositioningEdge positioningEdge)
            : this(new List<PositioningEdge> { positioningEdge })
        {
        }

        private PositioningEdgePath(IEnumerable<PositioningEdge> edges)
            : base(edges)
        {
        }

        public IEnumerable<PositioningVertexBase> Vertices => this.Select(i => i.Source).Concat(this.Last().Target);
        public IEnumerable<DummyPositioningVertex> InterimVertices => this.Skip(1).Select(i => i.Source).OfType<DummyPositioningVertex>();
        public bool IsFloating => Vertices.Any(i => i.IsFloating);

        public void Substitute(int atIndex, int removeEdgeCount, params PositioningEdge[] newEdges)
        {
            for (var i = 0; i < removeEdgeCount; i++)
                Edges.RemoveAt(atIndex);

            for (var i = 0; i < newEdges.Length; i++)
                Edges.Insert(atIndex+i, newEdges[i]);

            CheckInvariant();
        }

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

        public override string ToString()
        {
            return Vertices.ToDelimitedString("->");
        }
    }
}
