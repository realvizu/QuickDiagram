using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// A rank layer is an ordered list of LayoutVertex items with the same rank.
    /// Each vertex is aware of which layer it belongs to (has a Rank property).
    /// </summary>
    /// <remarks>
    /// Responsible for calculating the height of the layer and the horizontal positions of the vertices.
    /// If a new or moved vertex overlaps with another then automatically move the other away (recursively),
    /// to make room for the new/moved vertex.
    /// 
    /// WARNING: the order of the vertices in the list does not necessarily correspond 
    /// to their horizontal position order.
    /// </remarks>
    internal class RankLayer : IEnumerable<LayoutVertex>
    {
        public int Rank { get; }
        public double HorizontalGap { get; }
        private readonly LayoutGraph _layoutGraph;
        private readonly List<LayoutVertex> _vertices;
        public double Top { get; set; }

        internal RankLayer(int rank, double horizontalGap, LayoutGraph layoutGraph)
        {
            Rank = rank;
            HorizontalGap = horizontalGap;
            _layoutGraph = layoutGraph;

            _vertices = new List<LayoutVertex>();
        }

        public int Count => _vertices.Count;
        public double Height => _vertices.Any() ? _vertices.Max(i => i.Height) : 0;
        public Rect2D Rect => _vertices.Select(i => i.Rect).Union();

        public LayoutVertex this[int i] => _vertices[i];
        public int IndexOf(LayoutVertex item) => _vertices.IndexOf(item);

        public IEnumerator<LayoutVertex> GetEnumerator() => _vertices.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddVertex(LayoutVertex vertex)
        {
            vertex.Rank = Rank;
            _vertices.Add(vertex);

            var centerX = Rect.Right + HorizontalGap + vertex.Width / 2;
            var centerY = Top + Math.Max(Height, vertex.Height) / 2;
            vertex.Center = new Point2D(centerX, centerY);
        }

        public void InsertVertex(int index, LayoutVertex vertex)
        {
            _vertices.Insert(index, vertex);
            vertex.Rank = Rank;
        }

        public void RemoveVertex(LayoutVertex vertex)
        {
            EnsureVertexBelongsToThisLayer(vertex);
            _vertices.Remove(vertex);
            vertex.Rank = null;
        }

        public void MoveVertexCenterXTo(LayoutVertex vertex, double centerX, 
            TranslateDirection? overlapResolutionDirection = null)
        {
            EnsureVertexBelongsToThisLayer(vertex);

            if (vertex.Center.X.IsEqualWithTolerance(centerX))
                return;

            Debug.WriteLine($"Vertex {vertex} moving to: {vertex.Center} ({overlapResolutionDirection})");
            vertex.Center = new Point2D(centerX, vertex.Center.Y);
            ResolveOverlaps(vertex, overlapResolutionDirection);
        }

        private void ResolveOverlaps(LayoutVertex fixedVertex, TranslateDirection? translateDirection)
        {
            foreach (var otherVertex in _vertices)
            {
                if (otherVertex == fixedVertex || 
                    otherVertex.IsNeighbourOf(fixedVertex, _layoutGraph))
                    continue;

                if (fixedVertex.OverlapsWith(otherVertex, HorizontalGap))
                {
                    Debug.WriteLine($"Resolving overlap of {fixedVertex} by moving {otherVertex}.");
                    ResolveOverlap(fixedVertex, otherVertex, translateDirection);
                }
            }
        }

        private void ResolveOverlap(LayoutVertex fixedVertex, LayoutVertex movedVertex,
            TranslateDirection? translateDirection)
        {
            if (translateDirection == null)
                translateDirection = fixedVertex.Center.X >= movedVertex.Center.X
                    ? TranslateDirection.Left
                    : TranslateDirection.Right;

            var translateVectorX = translateDirection == TranslateDirection.Left
                ? fixedVertex.Left - movedVertex.Right - HorizontalGap
                : fixedVertex.Right - movedVertex.Left + HorizontalGap;

            MoveVertexCenterXTo(movedVertex, movedVertex.Center.X + translateVectorX, translateDirection);
        }

        private void EnsureVertexBelongsToThisLayer(LayoutVertex vertex)
        {
            if (vertex.Rank != Rank)
                throw new InvalidOperationException($"Vertex belongs to rank {vertex.Rank} instead of {Rank}.");
        }

        public void Split(double splitPoint, double splitAmount)
        {
            foreach (var layoutVertex in _vertices)
            {
                if ((splitAmount < 0 && layoutVertex.Center.X <= splitPoint) ||
                    (splitAmount > 0 && layoutVertex.Center.X >= splitPoint))
                {
                    MoveVertexCenterXTo(layoutVertex, layoutVertex.Center.X + splitAmount);
                }
            }
        }
    }
}
