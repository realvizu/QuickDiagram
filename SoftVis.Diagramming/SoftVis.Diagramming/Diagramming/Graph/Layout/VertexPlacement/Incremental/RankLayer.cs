using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// A rank layer is an ordered list of LayoutVertex items with the same rank.
    /// Each vertex is aware of which layer it belongs to (has a Rank property).
    /// The order of the items determines their horizontal order in the layout.
    /// </summary>
    internal class RankLayer : IEnumerable<LayoutVertex>
    {
        public int Rank { get; }
        public double HorizontalGap { get; }
        private readonly List<LayoutVertex> _vertices;
        public double Top { get; set; }

        internal RankLayer(int rank, double horizontalGap)
        {
            Rank = rank;
            HorizontalGap = horizontalGap;
            _vertices = new List<LayoutVertex>();
        }

        public int Count => _vertices.Count;
        public double Height => _vertices.Any() ? _vertices.Max(i => i.Height) : 0;
        public DiagramRect Rect => _vertices.Select(i => i.Rect).Union();

        public LayoutVertex this[int i] => _vertices[i];
        public int IndexOf(LayoutVertex item) => _vertices.IndexOf(item);

        public IEnumerator<LayoutVertex> GetEnumerator() => _vertices.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddVertex(LayoutVertex vertex)
        {
            vertex.Rank = Rank;
            _vertices.Add(vertex);

            var centerX = Rect.Right + HorizontalGap + vertex.Width / 2;
            vertex.Center = new DiagramPoint(centerX, Top + Math.Max(Height, vertex.Height) / 2);
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

        public void MoveVertexCenterXTo(LayoutVertex vertex, double centerX)
        {
            EnsureVertexBelongsToThisLayer(vertex);
            vertex.Center = new DiagramPoint(centerX, vertex.Center.Y);
            ResolveOverlaps(vertex);
        }

        private void ResolveOverlaps(LayoutVertex fixedVertex)
        {
            foreach (var otherVertex in _vertices)
            {
                if (otherVertex == fixedVertex) continue;

                if (fixedVertex.OverlapsWith(otherVertex, HorizontalGap))
                    ResolveOverlap(fixedVertex, otherVertex);
            }
        }

        private void ResolveOverlap(LayoutVertex fixedVertex, LayoutVertex movedVertex)
        {
            var moveXAmount = fixedVertex.Center.X >= movedVertex.Center.X
                ? fixedVertex.Left - movedVertex.Right - HorizontalGap
                : fixedVertex.Right - movedVertex.Left + HorizontalGap;

            MoveVertexCenterXTo(movedVertex, movedVertex.Center.X + moveXAmount);
        }

        private void EnsureVertexBelongsToThisLayer(LayoutVertex vertex)
        {
            if (vertex.Rank != Rank)
                throw new InvalidOperationException($"Vertex belongs to rank {vertex.Rank} instead of {Rank}.");
        }
    }
}
