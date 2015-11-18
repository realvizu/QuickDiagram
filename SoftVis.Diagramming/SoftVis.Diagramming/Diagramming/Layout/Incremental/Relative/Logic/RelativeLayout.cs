using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Summarizes the data structures that make up the relative layout and provides operations on them.
    /// </summary>
    internal sealed class RelativeLayout : IReadOnlyRelativeLayout
    {
        private readonly LayeredLayoutGraph _layeredLayoutGraph;
        private readonly LayoutVertexLayers _layers;

        public RelativeLayout(LayeredLayoutGraph layeredLayoutGraph, 
            LayoutVertexLayers layers)
        {
            _layeredLayoutGraph = layeredLayoutGraph;
            _layers = layers;
        }

        public IReadOnlyLayeredLayoutGraph LayeredLayoutGraph => _layeredLayoutGraph;
        public IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph => _layeredLayoutGraph.ProperGraph;
        public IReadOnlyLayoutVertexLayers LayoutVertexLayers => _layers;

        public IEnumerable<LayoutVertexBase> GetPrimarySiblingsInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            return ProperLayeredLayoutGraph.GetPrimarySiblings(vertex).Where(i => _layers.GetLayerIndex(i) == layerIndex);
        }

        public IEnumerable<LayoutVertexBase> GetPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            var layerIndex = _layers.GetLayerIndexOrThrow(vertex);
            return GetPrimarySiblingsInLayer(vertex, layerIndex);
        }

        public IEnumerable<LayoutVertexBase> GetPlacedPrimarySiblingsInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            return GetPrimarySiblingsInLayer(vertex, layerIndex).Where(i => !i.IsFloating); ;
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
            return ProperLayeredLayoutGraph.IsPlacedPrimarySiblingOf(vertex, previousVertex) ? previousVertex : null;
        }

        public LayoutVertexBase GetNextPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex)
        {
            var nextVertex = _layers.GetNextInLayer(vertex);
            return ProperLayeredLayoutGraph.IsPlacedPrimarySiblingOf(vertex, nextVertex) ? nextVertex : null;
        }
    }
}
