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
        public event EventHandler<DummyPositioningVertex> InterimVertexAdded;
        public event EventHandler<DummyPositioningVertex> InterimVertexRemoved;
        public event EventHandler<PositioningEdge> EdgeAdded;
        public event EventHandler<PositioningEdge> EdgeRemoved;

        public PositioningEdgePath(IEnumerable<PositioningEdge> edges)
            : base(edges)
        {
        }

        public PositioningEdgePath(PositioningEdge positioningEdge)
            : this(new List<PositioningEdge> { positioningEdge })
        {
        }

        public IEnumerable<PositioningVertexBase> Vertices => this.Select(i => i.Source).Concat(this.Last().Target);
        public IEnumerable<DummyPositioningVertex> InterimVertices => this.Skip(1).Select(i => i.Source).OfType<DummyPositioningVertex>();

        public void SplitEdge(int atIndex, int times)
        {
            for (var i = 0; i < times; i++)
                SplitEdge(atIndex);

            CheckInvariant();
        }

        private void SplitEdge(int atIndex)
        {
            var edgeToSplit = Edges[atIndex];

            Edges.Remove(edgeToSplit);
            EdgeRemoved?.Invoke(this, edgeToSplit);

            // TODO: a layerIndex automatikusan álljon be AddEdge-nél? egyáltalán kell layerindex?
            var interimVertex = new DummyPositioningVertex(edgeToSplit.Target.LayerIndex + 1, true);
            InterimVertexAdded?.Invoke(this, interimVertex);

            var newEdge1 = new PositioningEdge(edgeToSplit.Source, interimVertex);
            Edges.Insert(atIndex, newEdge1);
            EdgeAdded?.Invoke(this, newEdge1);

            var newEdge2 = new PositioningEdge(interimVertex, edgeToSplit.Target);
            Edges.Insert(atIndex + 1, newEdge2);
            EdgeAdded?.Invoke(this, newEdge2);
        }

        public void MergeEdgeWithNext(int atIndex, int times)
        {
            for (var i = 0; i < times; i++)
                MergeEdgeWithNext(atIndex);
        }

        private void MergeEdgeWithNext(int atIndex)
        {
            var firstEdge = Edges[atIndex];
            var nextEdge = Edges[atIndex + 1];

            Edges.Remove(firstEdge);
            EdgeRemoved?.Invoke(this, firstEdge);
            Edges.Remove(nextEdge);
            EdgeRemoved?.Invoke(this, nextEdge);

            var vertexToRemove = firstEdge.Target as DummyPositioningVertex;
            if (vertexToRemove == null)
                throw new Exception("FirstEdge.Target is null or not dummy!");
            InterimVertexRemoved?.Invoke(this, vertexToRemove);

            var mergedEdge = new PositioningEdge(firstEdge.Source, nextEdge.Target);
            Edges.Insert(atIndex, mergedEdge);
            EdgeAdded?.Invoke(this, mergedEdge);
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
