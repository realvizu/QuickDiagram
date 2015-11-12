using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Calculates the relative location of a vertex.
    /// </summary>
    internal sealed class RelativeLocationCalculator : RelativeLayoutActionEventSource
    {
        private readonly IReadOnlyRelativeLayout _relativeLayout;
        private readonly IComparer<LayoutVertexBase> _vertexComparer;

        public RelativeLocationCalculator(IReadOnlyRelativeLayout relativeLayout)
        {
            _relativeLayout = relativeLayout;
            _vertexComparer = new VerticesInLayerComparer(_relativeLayout.LowLevelLayoutGraph);
        }

        private IReadOnlyHighLevelLayoutGraph HighLevelLayoutGraph => _relativeLayout.HighLevelLayoutGraph;
        private IReadOnlyLowLevelLayoutGraph LowLevelLayoutGraph => _relativeLayout.LowLevelLayoutGraph;
        private IReadOnlyLayoutVertexLayers Layers => _relativeLayout.LayoutVertexLayers;
        private IReadOnlyLayoutVertexLayers LayersWithoutFloatingItems => Layers.GetViewWithoutFloatingItems();

        public RelativeLocation GetTargetLocation(LayoutVertexBase vertex)
        {
            vertex.IsFloating = true;

            var currentLayerIndex = Layers.GetLayerIndex(vertex) ?? -1;

            var minimumLayerIndex = vertex is DiagramNodeLayoutVertex
                ? CalculateRankInHighLevelLayoutGraph((DiagramNodeLayoutVertex) vertex)
                : CalculateRankInLowLevelLayoutGraph(vertex);

            var toLayerIndex = Math.Max(currentLayerIndex, minimumLayerIndex);
            var toIndexInLayer = DetermineIndexInLayer(vertex, minimumLayerIndex);

            vertex.IsFloating = false;

            return new RelativeLocation(toLayerIndex, toIndexInLayer);
        }

        private int CalculateRankInLowLevelLayoutGraph(LayoutVertexBase vertex)
        {
            return LowLevelLayoutGraph.GetRank(vertex, i => Layers.GetLayerIndexOrThrow(i));
        }

        private int CalculateRankInHighLevelLayoutGraph(DiagramNodeLayoutVertex vertex)
        {
            return HighLevelLayoutGraph.GetRank(vertex, i => Layers.GetLayerIndexOrThrow(i));
        }

        private int DetermineIndexInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            var layer = LayersWithoutFloatingItems.GetLayer(layerIndex);

            var parentVertex = LowLevelLayoutGraph.GetPrimaryParent(vertex);
            if (parentVertex == null)
                return layer.Count;

            var siblingsInLayer = _relativeLayout.GetPlacedPrimarySiblingsInLayer(vertex, layerIndex)
                .OrderBy(layer.IndexOf).ToArray();
            if (siblingsInLayer.Any())
                return CalculateInsertionIndexBasedOnSiblings(vertex, siblingsInLayer);

            return CalculateInsertionIndexBasedOnParent(layerIndex, parentVertex);
        }

        private int CalculateInsertionIndexBasedOnSiblings(LayoutVertexBase vertex, LayoutVertexBase[] siblingsInLayer)
        {
            var followingSiblingInLayer = siblingsInLayer.FirstOrDefault(i => Precedes(vertex, i));
            return followingSiblingInLayer != null
                ? LayersWithoutFloatingItems.GetIndexInLayer(followingSiblingInLayer)
                : LayersWithoutFloatingItems.GetIndexInLayer(siblingsInLayer.Last()) + 1;
        }

        private int CalculateInsertionIndexBasedOnParent(int targetLayer, LayoutVertexBase parentVertex)
        {
            var parentLayer = LayersWithoutFloatingItems.GetLayer(parentVertex);
            var parentIndexInLayer = LayersWithoutFloatingItems.GetIndexInLayer(parentVertex);

            var followingParent = GetFollowingVerticesWithPrimaryChildren(parentLayer, parentIndexInLayer).FirstOrDefault();
            if (followingParent == null)
                return LayersWithoutFloatingItems.GetLayer(targetLayer).Count;

            var firstChildOfFollowingParent = LowLevelLayoutGraph.GetPrimaryChildren(followingParent)
                .OrderBy(LayersWithoutFloatingItems.GetIndexInLayer).First();

            return LayersWithoutFloatingItems.GetIndexInLayer(firstChildOfFollowingParent);
        }

        private IEnumerable<LayoutVertexBase> GetFollowingVerticesWithPrimaryChildren(
            IReadOnlyLayoutVertexLayer layer, int index)
        {
            return layer.OrderBy(LayersWithoutFloatingItems.GetIndexInLayer)
                .Where(i => LayersWithoutFloatingItems.GetIndexInLayer(i) > index && LowLevelLayoutGraph.HasPrimaryChildren(i));
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _vertexComparer.Compare(vertex1, vertex2) < 0;
        }
    }
}
