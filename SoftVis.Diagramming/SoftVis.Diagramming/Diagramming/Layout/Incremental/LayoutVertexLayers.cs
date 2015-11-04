using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Tracks vertex layering and calculates vertical positions.
    /// </summary>
    internal class LayoutVertexLayers :  IReadOnlyLayoutVertexLayers
    {
        private readonly IReadOnlyLayoutGraph _layoutGraph;
        private readonly double _verticalGap;
        private readonly List<LayoutVertexLayer> _layers;
        private readonly Map<LayoutVertexBase, int> _vertexToLayerIndexMap;

        public LayoutVertexLayers(IReadOnlyLayoutGraph layoutGraph, double verticalGap)
        {
            _layoutGraph = layoutGraph;
            _verticalGap = verticalGap;
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
            var layerIndex = GetLayerIndex(vertex);
            RemoveVertexFromLayer(vertex, layerIndex);
        }

        public void AddEdge(LayoutEdge edge)
        {
            EnsureValidLayering(edge.Target);
            EnsureValidLayering(edge.Source);
        }

        public void RemoveEdge(LayoutEdge edge)
        {
            throw new NotImplementedException();
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

        private void MoveVertex(LayoutVertexBase vertex, int toLayerIndex)
        {
            RemoveVertexFromLayer(vertex, GetLayerIndex(vertex));
            AddVertexToLayer(vertex, toLayerIndex);
        }

        private void EnsureValidItemOrder(LayoutVertexBase vertex)
        {
            var layer = GetMutableLayer(vertex);
            if (layer.IsItemIndexValid(vertex))
                return;

            layer.Remove(vertex);
            layer.Add(vertex);
        }

        private void AddVertexToLayer(LayoutVertexBase vertex, int layerIndex)
        {
            var layer = EnsureLayerExists(layerIndex);
            _vertexToLayerIndexMap.Set(vertex, layerIndex);
            layer.Add(vertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private void RemoveVertexFromLayer(LayoutVertexBase vertex, int layerIndex)
        {
            _layers[layerIndex].Remove(vertex);
            _vertexToLayerIndexMap.Remove(vertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private LayoutVertexLayer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
                _layers.Add(new LayoutVertexLayer(_layoutGraph, i));

            UpdateLayerVerticalPositions(layerIndex);

            return _layers[layerIndex];
        }

        private void UpdateLayerVerticalPositions(int fromLayerIndex)
        {
            for (var i = fromLayerIndex; i < _layers.Count; i++)
            {
                _layers[i].Top = (i == 0)
                    ? 0
                    : _layers[i - 1].Bottom + _verticalGap;
            }
        }
    }
}
