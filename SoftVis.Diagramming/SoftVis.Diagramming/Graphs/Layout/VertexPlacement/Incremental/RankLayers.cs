using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// An ordered list of rank layers.
    /// Responsible for adjusting the vertical position of the layers,
    /// and for moving vertices between layers.
    /// </summary>
    internal class RankLayers : IEnumerable<RankLayer>
    {
        public double HorizontalGap { get; }
        public double VerticalGap { get; }
        private readonly LayoutGraph _layoutGraph;
        private readonly List<RankLayer> _layers;

        internal RankLayers(double horizontalGap, double verticalGap, LayoutGraph layoutGraph)
        {
            HorizontalGap = horizontalGap;
            VerticalGap = verticalGap;
            _layoutGraph = layoutGraph;

            _layers = new List<RankLayer>();
        }

        public int Count => _layers.Count;
        public RankLayer this[int index] => index < Count ? _layers[index] : null;

        public IEnumerator<RankLayer> GetEnumerator() => _layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public RankLayer GetLayer(LayoutVertex vertex)
        {
            if (vertex.Rank == null) throw new InvalidOperationException($"{nameof(vertex)} has no rank assigned.");

            return this[vertex.Rank.Value];
        }

        public void AddVertex(int rank, LayoutVertex vertex)
        {
            EnsureLayerExists(rank);

            _layers[rank].AddVertex(vertex);

            //foreach (var inEdge in _layoutGraph.InEdges(vertex))
            //    ArrangeVerticesOf(inEdge);

            AdjustLayerVerticalPositions(rank);
        }

        public void RemoveVertex(LayoutVertex vertex)
        {
            GetLayer(vertex).RemoveVertex(vertex);
        }

        public void MoveVertex(LayoutVertex vertex, int newRank)
        {
            RemoveVertex(vertex);
            AddVertex(newRank, vertex);
        }

        public void EnsureVertexIsUnderParentVertex(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            if (childVertex.Rank == null) throw new InvalidOperationException($"{nameof(childVertex)} has no rank assigned.");
            if (parentVertex.Rank == null) throw new InvalidOperationException($"{nameof(parentVertex)} has no rank assigned.");

            while (childVertex.Rank <= parentVertex.Rank)
                MoveVertex(childVertex, childVertex.Rank.Value + 1);
        }

        public void MoveVertexCenterXBy(LayoutVertex layoutVertex, double translateVectorX)
        {
            var direction = translateVectorX < 0 ? TranslateDirection.Left : TranslateDirection.Right;
            MoveVertexCenterXTo(layoutVertex, layoutVertex.Center.X + translateVectorX, direction);
        }

        public void MoveVertexCenterXTo(LayoutVertex layoutVertex, double centerX, 
            TranslateDirection? overlapResolutionDirection = null)
        {
            GetLayer(layoutVertex).MoveVertexCenterXTo(layoutVertex, centerX, overlapResolutionDirection);
        }

        private void EnsureLayerExists(int rank)
        {
            for (var i = Count; i <= rank; i++)
                _layers.Add(new RankLayer(i, HorizontalGap, _layoutGraph));

            AdjustLayerVerticalPositions(rank);
        }

        private void AdjustLayerVerticalPositions(int fromRank)
        {
            for (var i = fromRank; i < Count; i++)
            {
                _layers[i].Top = (i == 0)
                    ? 0
                    : _layers[i - 1].Rect.Bottom + VerticalGap;
            }
        }

        private void MakeRoomForVertex(LayoutVertex layoutVertex, double centerX)
        {
            var overlapRegions = GetOverlapRegions(layoutVertex, centerX);
            foreach (var overlapRect in overlapRegions)
                RemoveOverlap(layoutVertex, overlapRect);
        }

        private void RemoveOverlap(LayoutVertex newVertex, Rect2D overlapRect)
        {
            double splitPoint;
            double splitAmount;
            if (newVertex.Center.X >= overlapRect.Center.X)
            {
                splitPoint = overlapRect.Right;
                splitAmount = -overlapRect.Width - HorizontalGap;
            }
            else
            {
                splitPoint = overlapRect.Left;
                splitAmount = overlapRect.Width + HorizontalGap;
            }

            Split(splitPoint, splitAmount);
        }

        private void Split(double splitPoint, double splitAmount)
        {
            foreach (var layer in _layers)
                layer.Split(splitPoint, splitAmount);
        }

        private IEnumerable<Rect2D> GetOverlapRegions(LayoutVertex newVertex, double centerX)
        {
            var rankLayer = GetLayer(newVertex);

            var toBeCenterY = rankLayer.Top + Math.Max(rankLayer.Height, newVertex.Height) / 2;
            var toBeCenter = new Point2D(centerX, toBeCenterY);
            var toBeRect = Rect2D.CreateFromCenterAndSize(toBeCenter, newVertex.Size);

            foreach (var existingVertex in rankLayer)
            {
                var overlapRect = toBeRect.Intersect(existingVertex.Rect);
                if (overlapRect != Rect2D.Empty)
                    yield return overlapRect;
            }
        }
    }
}
