using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Summarizes the data structures that make up the relative layout and provides operations on them.
    /// </summary>
    internal sealed class RelativeLayout : IReadOnlyRelativeLayout
    {
        private readonly HighLevelLayoutGraph _highLevelLayoutGraph;
        private readonly LowLevelLayoutGraph _lowLevelLayoutGraph;
        private readonly LayoutVertexLayers _layers;

        public RelativeLayout(HighLevelLayoutGraph highLevelLayoutGraph, LowLevelLayoutGraph lowLevelLayoutGraph, 
            LayoutVertexLayers layers)
        {
            _highLevelLayoutGraph = highLevelLayoutGraph;
            _lowLevelLayoutGraph = lowLevelLayoutGraph;
            _layers = layers;
        }

        public IReadOnlyHighLevelLayoutGraph HighLevelLayoutGraph => _highLevelLayoutGraph;
        public IReadOnlyLowLevelLayoutGraph LowLevelLayoutGraph => _lowLevelLayoutGraph;
        public IReadOnlyLayoutVertexLayers LayoutVertexLayers => _layers;

        public IEnumerable<LayoutVertexBase> GetPrimarySiblingsInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            return _lowLevelLayoutGraph.GetPrimarySiblings(vertex).Where(i => _layers.GetLayerIndex(i) == layerIndex);
        }

        public IEnumerable<LayoutVertexBase> GetPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            var layerIndex = _layers.GetLayerIndexOrThrow(vertex);
            return GetPrimarySiblingsInLayer(vertex, layerIndex);
        }

        public IEnumerable<LayoutVertexBase> GetPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            return GetPrimarySiblingsInSameLayer(vertex).Where(i => !i.IsFloating);
        }

        public bool HasPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            return GetPlacedPrimarySiblingsInSameLayer(vertex).Any();
        }

        public LayoutVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex)
        {
            var previousVertex = _layers.GetPreviousInLayer(vertex);
            return _lowLevelLayoutGraph.IsPlacedPrimarySiblingOf(vertex, previousVertex) ? previousVertex : null;
        }

        public LayoutVertexBase GetNextPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex)
        {
            var nextVertex = _layers.GetNextInLayer(vertex);
            return _lowLevelLayoutGraph.IsPlacedPrimarySiblingOf(vertex, nextVertex) ? nextVertex : null;
        }
    }
}
