using System;
using System.Collections;
using System.Collections.Generic;

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

            foreach (var inEdge in _layoutGraph.InEdges(vertex))
                ArrangeVerticesOf(inEdge);

            AdjustLayerVerticalPositions(rank);
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
                ArrangeSourceVertexOf(edge);
            else
                ArrangeTargetVertexOf(edge);
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

        private void MoveVertexCenterXTo(LayoutVertex vertex, double centerX)
        {
            GetLayer(vertex).MoveVertexCenterXTo(vertex, centerX);
        }

        private void ArrangeSourceVertexOf(LayoutEdge edge)
        {
            MoveVertexCenterXTo(edge.Source, edge.Target.Center.X);
        }

        private void ArrangeTargetVertexOf(LayoutEdge edge)
        {
            MoveVertexCenterXTo(edge.Target, edge.Source.Center.X);
        }
    }
}
