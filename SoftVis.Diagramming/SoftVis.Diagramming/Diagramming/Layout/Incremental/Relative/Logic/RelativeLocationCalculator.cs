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
        private readonly IReadOnlyQuasiProperLayoutGraph _properLayoutGraph;
        private readonly IReadOnlyLayoutVertexLayers _layoutVertexLayers;
        private readonly IComparer<LayoutVertexBase> _siblingsComparer;

        public RelativeLocationCalculator(IReadOnlyQuasiProperLayoutGraph properLayoutGraph,
            IReadOnlyLayoutVertexLayers layoutVertexLayers)
        {
            _properLayoutGraph = properLayoutGraph;
            _layoutVertexLayers = layoutVertexLayers;
            _siblingsComparer = new SiblingsComparer(properLayoutGraph);
        }

        public RelativeLocation GetTargetLocation(LayoutVertexBase vertex)
        {
            var relativeLocation = _layoutVertexLayers.GetLocation(vertex);
            if (relativeLocation != null)
                throw new InvalidOperationException($"Vertex {vertex} already has a relative location: {relativeLocation}.");

            var toLayerIndex = _properLayoutGraph.GetLayerIndex(vertex);
            var toIndexInLayer = DetermineIndexInLayer(vertex, toLayerIndex);

            return new RelativeLocation(toLayerIndex, toIndexInLayer);
        }

        private int DetermineIndexInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            var layer = _layoutVertexLayers.GetLayer(layerIndex);

            var parentVertex = _properLayoutGraph.GetPrimaryParent(vertex);
            if (parentVertex == null)
                return layer.Count;

            var siblingsInLayer = GetPrimarySiblingsInLayer(vertex, layerIndex)
                .OrderBy(layer.IndexOf).ToArray();
            if (siblingsInLayer.Any())
                return CalculateInsertionIndexBasedOnSiblings(vertex, siblingsInLayer);

            return CalculateInsertionIndexBasedOnParent(layerIndex, parentVertex);
        }

        private int CalculateInsertionIndexBasedOnSiblings(LayoutVertexBase vertex, LayoutVertexBase[] siblingsInLayer)
        {
            var followingSiblingInLayer = siblingsInLayer.FirstOrDefault(i => Precedes(vertex, i));
            return followingSiblingInLayer != null
                ? _layoutVertexLayers.GetIndexInLayer(followingSiblingInLayer)
                : _layoutVertexLayers.GetIndexInLayer(siblingsInLayer.Last()) + 1;
        }

        private int CalculateInsertionIndexBasedOnParent(int targetLayer, LayoutVertexBase parentVertex)
        {
            var parentLayer = _layoutVertexLayers.GetLayer(parentVertex);
            var parentIndexInLayer = _layoutVertexLayers.GetIndexInLayer(parentVertex);

            var followingParent = GetFollowingVerticesWithPrimaryChildren(parentLayer, parentIndexInLayer).FirstOrDefault();
            if (followingParent == null)
                return _layoutVertexLayers.GetLayer(targetLayer).Count;

            var firstChildOfFollowingParent = _properLayoutGraph.GetPrimaryChildren(followingParent)
                .OrderBy(_layoutVertexLayers.GetIndexInLayer).First();

            return _layoutVertexLayers.GetIndexInLayer(firstChildOfFollowingParent);
        }

        private IEnumerable<LayoutVertexBase> GetFollowingVerticesWithPrimaryChildren(
            IReadOnlyLayoutVertexLayer layer, int index)
        {
            return layer.OrderBy(_layoutVertexLayers.GetIndexInLayer)
                .Where(i => _layoutVertexLayers.GetIndexInLayer(i) > index && _properLayoutGraph.HasPrimaryChildren(i));
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _siblingsComparer.Compare(vertex1, vertex2) < 0;
        }

        private IEnumerable<LayoutVertexBase> GetPrimarySiblingsInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            return _properLayoutGraph.GetPrimarySiblings(vertex).Where(i => _layoutVertexLayers.GetLayerIndex(i) == layerIndex);
        }
    }
}
