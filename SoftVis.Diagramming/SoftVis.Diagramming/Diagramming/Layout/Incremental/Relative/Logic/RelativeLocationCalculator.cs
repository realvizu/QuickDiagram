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
        private readonly IComparer<LayoutVertexBase> _siblingsComparer;

        public RelativeLocationCalculator(IReadOnlyRelativeLayout relativeLayout)
        {
            _relativeLayout = relativeLayout;
            _siblingsComparer = new SiblingsComparer(_relativeLayout.ProperLayeredLayoutGraph);
        }

        private IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph => _relativeLayout.ProperLayeredLayoutGraph;
        private IReadOnlyLayoutVertexLayers Layers => _relativeLayout.LayoutVertexLayers;
        private IReadOnlyLayoutVertexLayers LayersWithoutFloatingItems => Layers.GetViewWithoutFloatingItems();

        public RelativeLocation GetTargetLocation(LayoutVertexBase vertex)
        {
            vertex.IsFloating = true;

            var toLayerIndex = ProperLayeredLayoutGraph.GetLayerIndex(vertex);
            var toIndexInLayer = DetermineIndexInLayer(vertex, toLayerIndex);

            vertex.IsFloating = false;

            return new RelativeLocation(toLayerIndex, toIndexInLayer);
        }

        private int DetermineIndexInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            var layer = LayersWithoutFloatingItems.GetLayer(layerIndex);

            var parentVertex = ProperLayeredLayoutGraph.GetPrimaryParent(vertex);
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

            var firstChildOfFollowingParent = ProperLayeredLayoutGraph.GetPrimaryChildren(followingParent)
                .OrderBy(LayersWithoutFloatingItems.GetIndexInLayer).First();

            return LayersWithoutFloatingItems.GetIndexInLayer(firstChildOfFollowingParent);
        }

        private IEnumerable<LayoutVertexBase> GetFollowingVerticesWithPrimaryChildren(
            IReadOnlyLayoutVertexLayer layer, int index)
        {
            return layer.OrderBy(LayersWithoutFloatingItems.GetIndexInLayer)
                .Where(i => LayersWithoutFloatingItems.GetIndexInLayer(i) > index && ProperLayeredLayoutGraph.HasPrimaryChildren(i));
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _siblingsComparer.Compare(vertex1, vertex2) < 0;
        }
    }
}
