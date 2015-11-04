using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Tracks positioning vertex layering and calculates vertical positions.
    /// </summary>
    internal class PositioningVertexLayers :  IReadOnlyPositioningVertexLayers
    {
        private readonly IReadOnlyPositioningGraph _positioningGraph;
        private readonly double _verticalGap;
        private readonly List<PositioningVertexLayer> _layers;
        private readonly Map<PositioningVertexBase, int> _vertexToLayerIndexMap;

        public PositioningVertexLayers(IReadOnlyPositioningGraph positioningGraph, double verticalGap)
        {
            _positioningGraph = positioningGraph;
            _verticalGap = verticalGap;
            _layers = new List<PositioningVertexLayer>();
            _vertexToLayerIndexMap = new Map<PositioningVertexBase, int>();
        }

        public IEnumerator<IReadOnlyPositioningVertexLayer> GetEnumerator() => _layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        public void AddVertex(PositioningVertexBase vertex)
        {
            AddVertexToLayer(vertex, 0);
        }

        public void RemoveVertex(PositioningVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            RemoveVertexFromLayer(vertex, layerIndex);
        }

        public void AddEdge(PositioningEdge edge)
        {
            EnsureValidLayering(edge.Target);
            EnsureValidLayering(edge.Source);
        }

        public void RemoveEdge(PositioningEdge edge)
        {
            throw new NotImplementedException();
        }

        private void EnsureValidLayering(PositioningVertexBase vertex)
        {
            var minimumLayerIndex = _positioningGraph.GetParents(vertex)
                .Select(GetLayerIndex).DefaultIfEmpty(-1).Max() + 1;

            if (GetLayerIndex(vertex) < minimumLayerIndex)
                MoveVertex(vertex, minimumLayerIndex);
            else
                EnsureValidItemOrder(vertex);
        }

        public IReadOnlyPositioningVertexLayer GetLayer(PositioningVertexBase vertex)
        {
            return GetMutableLayer(vertex);
        }

        public int GetLayerIndex(PositioningVertexBase vertex)
        {
            return _vertexToLayerIndexMap.Get(vertex);
        }

        public int GetIndexInLayer(PositioningVertexBase vertex)
        {
            return GetLayer(vertex).IndexOf(vertex);
        }

        public PositioningVertexBase GetPreviousInLayer(PositioningVertexBase vertex)
        {
            return GetLayer(vertex).GetPrevious(vertex);
        }

        public PositioningVertexBase GetNextInLayer(PositioningVertexBase vertex)
        {
            return GetLayer(vertex).GetNext(vertex);
        }

        public IEnumerable<PositioningVertexBase> GetOtherPlacedVerticesInLayer(PositioningVertexBase vertex)
        {
            return GetLayer(vertex).Where(i => i != vertex && !i.IsFloating);
        }

        public bool HasPlacedPrimarySiblingsInSameLayer(PositioningVertexBase vertex)
        {
            return GetPlacedPrimarySiblingsInSameLayer(vertex).Any();
        }

        public IEnumerable<PositioningVertexBase> GetPlacedPrimarySiblingsInSameLayer(PositioningVertexBase vertex)
        {
            return GetPrimarySiblingsInSameLayer(vertex).Where(i => !i.IsFloating);
        }

        public PositioningVertexBase GetNextPlacedPrimarySiblingInSameLayer(PositioningVertexBase vertex)
        {
            var nextVertex = GetNextInLayer(vertex);
            return _positioningGraph.IsPlacedPrimarySiblingOf(vertex, nextVertex) ? nextVertex : null;
        }

        public PositioningVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(PositioningVertexBase vertex)
        {
            var previousVertex = GetPreviousInLayer(vertex);
            return _positioningGraph.IsPlacedPrimarySiblingOf(vertex, previousVertex) ? previousVertex : null;
        }

        public IEnumerable<PositioningVertexBase> GetPrimarySiblingsInSameLayer(PositioningVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            return _positioningGraph.GetPrimarySiblings(vertex).Where(i => GetLayerIndex(i) == layerIndex);
        }

        private PositioningVertexLayer GetMutableLayer(PositioningVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            return _layers[layerIndex];
        }

        private void MoveVertex(PositioningVertexBase vertex, int toLayerIndex)
        {
            RemoveVertexFromLayer(vertex, GetLayerIndex(vertex));
            AddVertexToLayer(vertex, toLayerIndex);
        }

        private void EnsureValidItemOrder(PositioningVertexBase vertex)
        {
            var layer = GetMutableLayer(vertex);
            if (layer.IsItemIndexValid(vertex))
                return;

            layer.Remove(vertex);
            layer.Add(vertex);
        }

        private void AddVertexToLayer(PositioningVertexBase vertex, int layerIndex)
        {
            var layer = EnsureLayerExists(layerIndex);
            _vertexToLayerIndexMap.Set(vertex, layerIndex);
            layer.Add(vertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private void RemoveVertexFromLayer(PositioningVertexBase vertex, int layerIndex)
        {
            _layers[layerIndex].Remove(vertex);
            _vertexToLayerIndexMap.Remove(vertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private PositioningVertexLayer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
                _layers.Add(new PositioningVertexLayer(_positioningGraph, i));

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
