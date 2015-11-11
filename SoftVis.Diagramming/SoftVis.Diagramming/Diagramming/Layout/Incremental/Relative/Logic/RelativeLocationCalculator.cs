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
                ? CalculateRankInHighLevelLayoutGraph(vertex)
                : CalculateRankInLowLevelLayoutGraph(vertex);

            var toLayerIndex = Math.Max(currentLayerIndex, minimumLayerIndex);
            var toIndexInLayer = DetermineIndexInLayer(vertex, minimumLayerIndex);

            return new RelativeLocation(toLayerIndex, toIndexInLayer);
        }

        private int CalculateRankInLowLevelLayoutGraph(LayoutVertexBase vertex)
        {
            return LowLevelLayoutGraph.GetRank(vertex, i => LayersWithoutFloatingItems.GetLayerIndexOrThrow(i));
        }

        private int CalculateRankInHighLevelLayoutGraph(LayoutVertexBase vertex)
        {
            return HighLevelLayoutGraph.GetRank((DiagramNodeLayoutVertex)vertex, 
                i => LayersWithoutFloatingItems.GetLayerIndexOrThrow(i));
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
            CheckThatParentIsAtOneLayerHigherThanVertex(targetLayer, parentVertex);

            var parentLayer = LayersWithoutFloatingItems.GetLayer(parentVertex);
            var parentIndexInLayer = LayersWithoutFloatingItems.GetIndexInLayer(parentVertex);

            var followingParent = GetFollowingVerticesWithPrimaryChildren(parentLayer, parentIndexInLayer).FirstOrDefault();
            if (followingParent == null)
                return LayersWithoutFloatingItems.GetLayer(targetLayer).Count;

            var firstChildOfFollowingParent = LowLevelLayoutGraph.GetPrimaryChildren(followingParent)
                .OrderBy(LayersWithoutFloatingItems.GetIndexInLayer).First();

            CheckThatVerticesAreOnTheSameLayer(targetLayer, firstChildOfFollowingParent);

            return LayersWithoutFloatingItems.GetIndexInLayer(firstChildOfFollowingParent);
        }

        private IEnumerable<LayoutVertexBase> GetFollowingVerticesWithPrimaryChildren(
            IReadOnlyLayoutVertexLayer layer, int index)
        {
            return layer.OrderBy(LayersWithoutFloatingItems.GetIndexInLayer)
                .Where(i => LayersWithoutFloatingItems.GetIndexInLayer(i) > index && LowLevelLayoutGraph.HasPrimaryChildren(i));
        }

        private void CheckThatParentIsAtOneLayerHigherThanVertex(int layerIndex, LayoutVertexBase parentVertex)
        {
            var parentLayerIndex = LayersWithoutFloatingItems.GetLayerIndex(parentVertex);
            if (parentLayerIndex != layerIndex - 1)
                throw new Exception($"Parent {parentVertex} was expected to be on layer {layerIndex - 1} but was on layer {parentLayerIndex}.");
        }

        private void CheckThatVerticesAreOnTheSameLayer(int layerIndex, LayoutVertexBase otherVertex)
        {
            var otherLayerIndex = LayersWithoutFloatingItems.GetLayerIndex(otherVertex);
            if (otherLayerIndex != layerIndex)
                throw new Exception($"Vertex {otherVertex} was expected to be on layer {layerIndex} but was on layer {otherLayerIndex}.");
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _vertexComparer.Compare(vertex1, vertex2) < 0;
        }
    }
}
