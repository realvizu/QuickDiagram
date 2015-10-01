using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// An ordered list of rank layers.
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
            vertex.CenterChanged += OnVertexCenterChanged;

            foreach (var inEdge in _layoutGraph.InEdges(vertex))
                ArrangeVerticesOf(inEdge);

            AdjustLayerVerticalPositions(rank);
        }

        private void OnVertexCenterChanged(object sender, DiagramPoint diagramPoint)
        {
        }

        private void EnsureLayerExists(int rank)
        {
            for (var i = Count; i <= rank; i++)
                _layers.Add(new RankLayer(i, HorizontalGap));

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

        public void RemoveVertex(LayoutVertex vertex)
        {
            GetLayer(vertex).RemoveVertex(vertex);
            vertex.CenterChanged -= OnVertexCenterChanged;
        }

        public void MoveVertex(LayoutVertex vertex, int newRank)
        {
            RemoveVertex(vertex);
            AddVertex(newRank, vertex);
        }

        public void ArrangeVerticesOf(LayoutEdge edge)
        {
            EnsureTargetVertexIsAboveSourceVertex(edge.Source, edge.Target);
            ArrangeSourceOrTargetVertexOf(edge);
        }

        private void EnsureTargetVertexIsAboveSourceVertex(LayoutVertex source, LayoutVertex target)
        {
            if (source.Rank == null) throw new InvalidOperationException($"{nameof(source)} has no rank assigned.");
            if (target.Rank == null) throw new InvalidOperationException($"{nameof(target)} has no rank assigned.");

            while (source.Rank <= target.Rank)
                MoveVertex(source, source.Rank.Value + 1);
        }

        private void ArrangeSourceOrTargetVertexOf(LayoutEdge edge)
        {
            var vertexToMove = ChooseVertexToMove(edge.Source, edge.Target);
            if (vertexToMove == edge.Source)
            {
                ArrangeVertexToOutNeighbours(edge.Source);
                AdjustOtherLayers(edge.Source);
            }
            else
            {
                ArrangeVertexToInNeighbours(edge.Target);
                AdjustOtherLayers(edge.Target);
            }
        }

        private LayoutVertex ChooseVertexToMove(LayoutVertex vertex1, LayoutVertex vertex2)
        {
            if (vertex1.Rank == null) throw new InvalidOperationException($"{nameof(vertex1)} has no rank assigned.");
            if (vertex2.Rank == null) throw new InvalidOperationException($"{nameof(vertex2)} has no rank assigned.");

            var vertex1EdgeCount = _layoutGraph.Degree(vertex1);
            var vertex2EdgeCount = _layoutGraph.Degree(vertex2);

            if (vertex1EdgeCount < vertex2EdgeCount)
                return vertex1;
            if (vertex1EdgeCount > vertex2EdgeCount)
                return vertex2;
            if (vertex1.Rank > vertex2.Rank)
                return vertex1;
            if (vertex1.Rank < vertex2.Rank)
                return vertex2;

            return GetLayer(vertex1).IndexOf(vertex1) > GetLayer(vertex2).IndexOf(vertex2)
                ? vertex1
                : vertex2;
        }

        private void MoveVertexCenterXTo(LayoutVertex layoutVertex, double centerX)
        {
            if (!layoutVertex.Center.X.IsEqualWithTolerance(centerX))
                GetLayer(layoutVertex).MoveVertexCenterXTo(layoutVertex, centerX);
        }

        private void ArrangeVertexToOutNeighbours(LayoutVertex layoutVertex)
        {
            var outNeighboursRect = _layoutGraph.GetOutNeighbours(layoutVertex).Select(i => i.Rect).Union();
            if (outNeighboursRect != DiagramRect.Empty)
                MoveVertexCenterXTo(layoutVertex, outNeighboursRect.Center.X);
        }

        private void ArrangeVertexToInNeighbours(LayoutVertex layoutVertex)
        {
            var inNeighboursRect = _layoutGraph.GetInNeighbours(layoutVertex).Select(i => i.Rect).Union();
            if (inNeighboursRect != DiagramRect.Empty)
                MoveVertexCenterXTo(layoutVertex, inNeighboursRect.Center.X);
        }

        private void AdjustOtherLayers(LayoutVertex layoutVertex)
        {
            if (layoutVertex.Rank == null) throw new InvalidOperationException($"{nameof(layoutVertex)} has no rank assigned.");

            for (var i = layoutVertex.Rank.Value - 1; i >= 0; i--)
                AdjustLayerToInNeighbours(_layers[i]);

            for (var i = layoutVertex.Rank.Value + 1; i < Count; i++)
                AdjustLayerToOutNeighbours(_layers[i]);
        }

        private void AdjustLayerToInNeighbours(RankLayer rankLayer)
        {
            foreach (var layoutVertex in rankLayer)
                ArrangeVertexToInNeighbours(layoutVertex);
        }

        private void AdjustLayerToOutNeighbours(RankLayer rankLayer)
        {
            foreach (var layoutVertex in rankLayer)
                ArrangeVertexToOutNeighbours(layoutVertex);
        }
    }
}
