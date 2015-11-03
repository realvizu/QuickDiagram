using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Tracks positioning vertex layering and calculates vertical positions.
    /// </summary>
    internal class PositioningVertexLayers : IEnumerable<PositioningVertexLayer>, IReadOnlyPositioningVertexLayers
    {
        private readonly double _verticalGap;
        private readonly List<PositioningVertexLayer> _layers;
        private readonly Map<PositioningVertexBase, int> _vertexToLayerIndexMap;

        public PositioningVertexLayers(double verticalGap)
        {
            _verticalGap = verticalGap;

            _layers = new List<PositioningVertexLayer>();
            _vertexToLayerIndexMap = new Map<PositioningVertexBase, int>();
        }

        public IEnumerator<PositioningVertexLayer> GetEnumerator() => _layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<IReadOnlyPositioningVertexLayer> IEnumerable<IReadOnlyPositioningVertexLayer>.GetEnumerator() => GetEnumerator();

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        public int GetLayerIndex(PositioningVertexBase vertex)
        {
            return _vertexToLayerIndexMap.Get(vertex);
        }

        public PositioningVertexLayer GetLayer(PositioningVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            return _layers[layerIndex];
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

        public void EnsureValidLayering(PositioningVertexBase vertex)
        {
            var minimumLayerIndex = vertex.GetParents().Select(i => i.GetLayerIndex()).DefaultIfEmpty(-1).Max() + 1;

            if (vertex.GetLayerIndex() < minimumLayerIndex)
                MoveVertex(vertex, minimumLayerIndex);
            else
                EnsureValidItemOrder(vertex);
        }

        private void MoveVertex(PositioningVertexBase vertex, int toLayerIndex)
        {
            RemoveVertexFromLayer(vertex, vertex.GetLayerIndex());
            AddVertexToLayer(vertex, toLayerIndex);
        }

        private void EnsureValidItemOrder(PositioningVertexBase vertex)
        {
            if (vertex.IsLayerItemIndexValid())
                return;

            var layer = GetLayer(vertex);
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
                _layers.Add(new PositioningVertexLayer(i));

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
