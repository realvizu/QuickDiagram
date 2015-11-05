using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Arranges vertices into layers so that all edges point "upward" (to a layer with lower index),
    /// and orders vertices in layers so that primary edges never cross.
    /// </summary>
    /// <remarks>
    /// No primary edge crossing is achieved by:
    /// <para>dummy vertices ensure that parent and children are always on adjacent layers,</para>
    /// <para>if a vertex has primary siblings than it is placed next to them so primary siblings always form a block,</para>
    /// <para>if a vertex has no primary siblings but has primary parent then it is ordered based on parent ordering.</para>
    /// </remarks>
    internal class LayoutVertexLayers : IReadOnlyLayoutVertexLayers
    {
        private readonly IReadOnlyLayoutGraph _layoutGraph;
        private readonly IComparer<LayoutVertexBase> _vertexComparer;
        private readonly List<LayoutVertexLayer> _layers;
        private readonly Map<LayoutVertexBase, int> _vertexToLayerIndexMap;

        public LayoutVertexLayers(IReadOnlyLayoutGraph layoutGraph)
        {
            _layoutGraph = layoutGraph;
            _vertexComparer = new VerticesInLayerComparer(layoutGraph);
            _layers = new List<LayoutVertexLayer>();
            _vertexToLayerIndexMap = new Map<LayoutVertexBase, int>();
        }

        public IEnumerator<IReadOnlyLayoutVertexLayer> GetEnumerator() => _layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        public void AddVertex(LayoutVertexBase vertex)
        {
            AddVertexToLayer(vertex, 0);
        }

        public void RemoveVertex(LayoutVertexBase vertex)
        {
            RemoveVertexFromLayer(vertex);
        }

        public void AddEdge(LayoutEdge edge)
        {
            _layoutGraph.ExecuteOnDescendantVertices(edge.Source, EnsureValidLayering);
        }

        public void RemoveEdge(LayoutEdge edge)
        {
        }

        public void UpdateLayerVerticalPositions(double verticalGap)
        {
            for (var i = 0; i < _layers.Count; i++)
            {
                _layers[i].Top = (i == 0)
                    ? 0
                    : _layers[i - 1].Bottom + verticalGap;
            }
        }

        public IReadOnlyLayoutVertexLayer GetLayer(LayoutVertexBase vertex)
        {
            return GetMutableLayer(vertex);
        }

        public int GetLayerIndex(LayoutVertexBase vertex)
        {
            return _vertexToLayerIndexMap.Get(vertex);
        }

        public int GetIndexInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).IndexOf(vertex);
        }

        public LayoutVertexBase GetPreviousInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).GetPrevious(vertex);
        }

        public LayoutVertexBase GetNextInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).GetNext(vertex);
        }

        public IEnumerable<LayoutVertexBase> GetOtherPlacedVerticesInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).Where(i => i != vertex && !i.IsFloating);
        }

        public bool HasPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            return GetPlacedPrimarySiblingsInSameLayer(vertex).Any();
        }

        public IEnumerable<LayoutVertexBase> GetPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            return GetPrimarySiblingsInSameLayer(vertex).Where(i => !i.IsFloating);
        }

        public LayoutVertexBase GetNextPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex)
        {
            var nextVertex = GetNextInLayer(vertex);
            return _layoutGraph.IsPlacedPrimarySiblingOf(vertex, nextVertex) ? nextVertex : null;
        }

        public LayoutVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex)
        {
            var previousVertex = GetPreviousInLayer(vertex);
            return _layoutGraph.IsPlacedPrimarySiblingOf(vertex, previousVertex) ? previousVertex : null;
        }

        public IEnumerable<LayoutVertexBase> GetPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            return _layoutGraph.GetPrimarySiblings(vertex).Where(i => GetLayerIndex(i) == layerIndex);
        }

        private LayoutVertexLayer GetMutableLayer(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            return _layers[layerIndex];
        }

        private void EnsureValidLayering(LayoutVertexBase vertex)
        {
            var minimumLayerIndex = _layoutGraph.GetParents(vertex)
                .Select(GetLayerIndex).DefaultIfEmpty(-1).Max() + 1;

            if (GetLayerIndex(vertex) < minimumLayerIndex)
                MoveVertex(vertex, minimumLayerIndex);
            else
                EnsureValidItemOrder(vertex);
        }

        private void MoveVertex(LayoutVertexBase vertex, int toLayerIndex)
        {
            RemoveVertexFromLayer(vertex);
            AddVertexToLayer(vertex, toLayerIndex);
        }

        private void EnsureValidItemOrder(LayoutVertexBase vertex)
        {
            var indexInLayer = DetermineIndexInLayer(vertex);
            if (indexInLayer == GetIndexInLayer(vertex))
                return;

            var layerIndex = GetLayerIndex(vertex);
            RemoveVertexFromLayer(vertex);
            AddVertexToLayer(vertex, layerIndex);
        }

        private void AddVertexToLayer(LayoutVertexBase vertex, int layerIndex)
        {
            var layer = EnsureLayerExists(layerIndex);
            _vertexToLayerIndexMap.Set(vertex, layerIndex);

            var indexInLayer = DetermineIndexInLayer(vertex);
            layer.Add(vertex, indexInLayer);
        }

        private void RemoveVertexFromLayer(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);

            _layers[layerIndex].Remove(vertex);
            _vertexToLayerIndexMap.Remove(vertex);
        }

        private LayoutVertexLayer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
                _layers.Add(new LayoutVertexLayer(_layoutGraph, i));

            return _layers[layerIndex];
        }

        private int DetermineIndexInLayer(LayoutVertexBase vertex)
        {
            var layer = GetMutableLayer(vertex);

            var parentVertex = _layoutGraph.GetPrimaryParent(vertex);
            if (parentVertex == null)
                return layer.Count;

            var siblingsInLayer = GetPrimarySiblingsInSameLayer(vertex).OrderBy(layer.IndexOf).ToArray();
            if (siblingsInLayer.Any())
                return CalculateInsertionIndexBasedOnSiblings(vertex, siblingsInLayer);

            return CalculateInsertionIndexBasedOnParents(vertex, parentVertex);
        }

        private int CalculateInsertionIndexBasedOnSiblings(LayoutVertexBase vertex, LayoutVertexBase[] siblingsInLayer)
        {
            var followingSiblingInLayer = siblingsInLayer.FirstOrDefault(i => Precedes(vertex, i));
            return followingSiblingInLayer != null
                ? GetIndexInLayer(followingSiblingInLayer)
                : GetIndexInLayer(siblingsInLayer.Last()) + 1;
        }

        private int CalculateInsertionIndexBasedOnParents(LayoutVertexBase vertex, LayoutVertexBase parentVertex)
        {
            CheckThatParentIsAtOneLayerHigherThanVertex(vertex, parentVertex);

            var parentLayer = GetLayer(parentVertex);
            var parentIndexInLayer = GetIndexInLayer(parentVertex);

            var followingParent = GetFollowingVerticesWithPrimaryChildren(parentLayer, parentIndexInLayer).FirstOrDefault();
            if (followingParent == null)
                return GetLayer(vertex).Count;

            var firstChildOfFollowingParent = _layoutGraph.GetPrimaryChildren(followingParent).OrderBy(GetIndexInLayer).First();
            CheckThatVerticesAreOnTheSameLayer(vertex, firstChildOfFollowingParent);
            return GetIndexInLayer(firstChildOfFollowingParent);
        }

        private IEnumerable<LayoutVertexBase> GetFollowingVerticesWithPrimaryChildren(IReadOnlyLayoutVertexLayer layer, 
            int index)
        {
            return layer.OrderBy(GetIndexInLayer)
                .Where(i => GetIndexInLayer(i) > index && _layoutGraph.HasPrimaryChildren(i));
        }

        private void CheckThatParentIsAtOneLayerHigherThanVertex(LayoutVertexBase vertex, LayoutVertexBase parentVertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            var parentLayerIndex = GetLayerIndex(parentVertex);
            if (layerIndex != parentLayerIndex + 1)
                throw new Exception($"Child was expected to be 1 layer lower than parent, but vertex {vertex} is on layer {layerIndex} and parent {parentVertex} is on layer {parentLayerIndex}.");
        }

        private void CheckThatVerticesAreOnTheSameLayer(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            var layerIndex1 = GetLayerIndex(vertex1);
            var layerIndex2 = GetLayerIndex(vertex2);
            if (layerIndex1 != layerIndex2)
                throw new Exception($"Vertices were expected to be on the same layer, but vertex {vertex1} is on layer {layerIndex1} and vertex {vertex2} is on layer {layerIndex2}.");
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _vertexComparer.Compare(vertex1, vertex2) < 0;
        }
    }
}
