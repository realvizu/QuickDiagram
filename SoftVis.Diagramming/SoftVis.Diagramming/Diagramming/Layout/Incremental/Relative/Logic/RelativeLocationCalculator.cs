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

        public RelativeLocation GetTargetLocation(LayoutVertexBase vertex)
        {
            var isDiagramNodeLayoutVertex = vertex is DiagramNodeLayoutVertex;
            var currentLocation = Layers.GetLocation(vertex);

            var minimumLayerIndex = isDiagramNodeLayoutVertex
                ? CalculateRankInHighLevelLayoutGraph(vertex)
                : CalculateRankInLowLevelLayoutGraph(vertex);

            var toLayerIndex = Math.Max(currentLocation.LayerIndex, minimumLayerIndex);
            var toIndexInLayer = DetermineIndexInLayer(vertex, minimumLayerIndex);
            return new RelativeLocation(toLayerIndex, toIndexInLayer);
        }

        private int CalculateRankInLowLevelLayoutGraph(LayoutVertexBase vertex)
        {
            return LowLevelLayoutGraph.GetRank(vertex, i => Layers.GetLayerIndex(i));
        }

        private int CalculateRankInHighLevelLayoutGraph(LayoutVertexBase vertex)
        {
            return HighLevelLayoutGraph.GetRank((DiagramNodeLayoutVertex)vertex, i => Layers.GetLayerIndex(i));
        }

        private int DetermineIndexInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            var layer = Layers.GetLayer(layerIndex);

            var parentVertex = LowLevelLayoutGraph.GetPrimaryParent(vertex);
            if (parentVertex == null)
                return layer.Count;

            var siblingsInLayer = _relativeLayout.GetPrimarySiblingsInSameLayer(vertex).OrderBy(layer.IndexOf).ToArray();
            if (siblingsInLayer.Any())
                return CalculateInsertionIndexBasedOnSiblings(vertex, siblingsInLayer);

            return CalculateInsertionIndexBasedOnParents(vertex, parentVertex);
        }

        private int CalculateInsertionIndexBasedOnSiblings(LayoutVertexBase vertex, LayoutVertexBase[] siblingsInLayer)
        {
            var followingSiblingInLayer = siblingsInLayer.FirstOrDefault(i => Precedes(vertex, i));
            return followingSiblingInLayer != null
                ? Layers.GetIndexInLayer(followingSiblingInLayer)
                : Layers.GetIndexInLayer(siblingsInLayer.Last()) + 1;
        }

        private int CalculateInsertionIndexBasedOnParents(LayoutVertexBase vertex, LayoutVertexBase parentVertex)
        {
            CheckThatParentIsAtOneLayerHigherThanVertex(vertex, parentVertex);

            var parentLayer = Layers.GetLayer(parentVertex);
            var parentIndexInLayer = Layers.GetIndexInLayer(parentVertex);

            var followingParent = GetFollowingVerticesWithPrimaryChildren(parentLayer, parentIndexInLayer).FirstOrDefault();
            if (followingParent == null)
                return Layers.GetLayer(vertex).Count;

            var firstChildOfFollowingParent = LowLevelLayoutGraph.GetPrimaryChildren(followingParent)
                .OrderBy(Layers.GetIndexInLayer).First();
            CheckThatVerticesAreOnTheSameLayer(vertex, firstChildOfFollowingParent);
            return Layers.GetIndexInLayer(firstChildOfFollowingParent);
        }

        private IEnumerable<LayoutVertexBase> GetFollowingVerticesWithPrimaryChildren(
            IReadOnlyLayoutVertexLayer layer, int index)
        {
            return layer.OrderBy(Layers.GetIndexInLayer)
                .Where(i => Layers.GetIndexInLayer(i) > index && LowLevelLayoutGraph.HasPrimaryChildren(i));
        }

        private void CheckThatParentIsAtOneLayerHigherThanVertex(LayoutVertexBase vertex, LayoutVertexBase parentVertex)
        {
            var layerIndex = Layers.GetLayerIndex(vertex);
            var parentLayerIndex = Layers.GetLayerIndex(parentVertex);
            if (layerIndex != parentLayerIndex + 1)
                throw new Exception($"Child was expected to be 1 layer lower than parent, but vertex {vertex} is on layer {layerIndex} and parent {parentVertex} is on layer {parentLayerIndex}.");
        }

        private void CheckThatVerticesAreOnTheSameLayer(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            var layerIndex1 = Layers.GetLayerIndex(vertex1);
            var layerIndex2 = Layers.GetLayerIndex(vertex2);
            if (layerIndex1 != layerIndex2)
                throw new Exception($"Vertices were expected to be on the same layer, but vertex {vertex1} is on layer {layerIndex1} and vertex {vertex2} is on layer {layerIndex2}.");
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _vertexComparer.Compare(vertex1, vertex2) < 0;
        }
    }
}
