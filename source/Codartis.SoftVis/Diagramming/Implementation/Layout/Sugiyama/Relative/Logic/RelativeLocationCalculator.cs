using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// Calculates the relative location of a vertex.
    /// Ensures that primary edges never cross.
    /// </summary>
    /// <remarks>
    /// No primary edge crossing is achieved by:
    /// <para>proper graph ensures that parent and children are always on adjacent layers,</para>
    /// <para>primary siblings on a layer are always placed together to form a block,</para>
    /// <para>without primary siblings but having a primary parent the order is based on parent ordering.</para>
    /// </remarks>
    internal sealed class RelativeLocationCalculator
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
            return GetTargetLocation(vertex, _layoutVertexLayers);
        }

        private RelativeLocation GetTargetLocation(LayoutVertexBase vertex, IReadOnlyLayoutVertexLayers layers)
        {
            if (layers.HasLocation(vertex))
                throw new InvalidOperationException($"Vertex {vertex} already has a relative location.");

            var toLayerIndex = _properLayoutGraph.GetLayerIndex(vertex);
            var toIndexInLayer = DetermineIndexInLayer(vertex, toLayerIndex, layers);

            return new RelativeLocation(toLayerIndex, toIndexInLayer);
        }

        private int DetermineIndexInLayer(LayoutVertexBase vertex, int layerIndex, IReadOnlyLayoutVertexLayers layers)
        {
            var layer = layers.GetLayer(layerIndex);

            var parentVertex = _properLayoutGraph.GetPrimaryParent(vertex);
            if (parentVertex == null)
                return layer.Count;

            var siblingsInLayer = GetPrimarySiblingsInLayer(vertex, layerIndex, layers)
                .OrderBy(layer.IndexOf).ToArray();
            if (siblingsInLayer.Any())
                return CalculateInsertionIndexBasedOnSiblings(vertex, siblingsInLayer, layers);

            return CalculateInsertionIndexBasedOnParent(layerIndex, parentVertex, layers);
        }

        private int CalculateInsertionIndexBasedOnSiblings(LayoutVertexBase vertex,
            LayoutVertexBase[] siblingsInLayer, IReadOnlyLayoutVertexLayers layers)
        {
            var followingSiblingInLayer = siblingsInLayer.FirstOrDefault(i => Precedes(vertex, i));
            return followingSiblingInLayer != null
                ? layers.GetIndexInLayer(followingSiblingInLayer)
                : layers.GetIndexInLayer(siblingsInLayer.Last()) + 1;
        }

        private int CalculateInsertionIndexBasedOnParent(int targetLayer, LayoutVertexBase parentVertex,
            IReadOnlyLayoutVertexLayers layers)
        {
            var parentLayer = layers.GetLayer(parentVertex);
            var parentIndexInLayer = layers.GetIndexInLayer(parentVertex);

            var followingParent = GetFollowingParent(parentLayer, parentIndexInLayer, layers);
            if (followingParent == null)
                return layers.GetLayer(targetLayer).Count;

            var firstChildOfFollowingParent = _properLayoutGraph.GetPrimaryChildren(followingParent)
                .Where(layers.HasLocation).OrderBy(layers.GetIndexInLayer).First();

            return layers.GetIndexInLayer(firstChildOfFollowingParent);
        }

        private LayoutVertexBase GetFollowingParent(IReadOnlyLayoutVertexLayer layer, int index,
            IReadOnlyLayoutVertexLayers layers)
        {
            return layer.FirstOrDefault(i => layers.GetIndexInLayer(i) > index && 
                _properLayoutGraph.GetPrimaryChildren(i).Any(layers.HasLocation));
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _siblingsComparer.Compare(vertex1, vertex2) < 0;
        }

        private IEnumerable<LayoutVertexBase> GetPrimarySiblingsInLayer(
            LayoutVertexBase vertex, int layerIndex, IReadOnlyLayoutVertexLayers layers)
        {
            return _properLayoutGraph.GetPrimarySiblings(vertex)
                .Where(i => layers.HasLocation(i) && layers.GetLayerIndex(i) == layerIndex);
        }
    }
}
