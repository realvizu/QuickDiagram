using System.Collections;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Tracks vertex layering and calculates vertical positions.
    /// </summary>
    internal class LayoutVertexLayers : IEnumerable<LayoutVertexLayer>
    {
        private readonly double _verticalGap;
        private readonly List<LayoutVertexLayer> _layers;
        private readonly Dictionary<LayoutVertexBase, int> _vertexToLayerIndexMap;

        public LayoutVertexLayers(double verticalGap)
        {
            _verticalGap = verticalGap;

            _layers = new List<LayoutVertexLayer>();
            _vertexToLayerIndexMap = new Dictionary<LayoutVertexBase, int>();
        }

        public IEnumerator<LayoutVertexLayer> GetEnumerator() => _layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int GetLayerIndex(LayoutVertexBase layoutVertex)
        {
            return _vertexToLayerIndexMap[layoutVertex];
        }

        public LayoutVertexLayer GetLayer(LayoutVertexBase layoutVertex)
        {
            var layerIndex = GetLayerIndex(layoutVertex);
            return _layers[layerIndex];
        }

        public void AddVertex(LayoutVertexBase layoutVertex)
        {
            AddVertexToLayer(layoutVertex, 0);
        }

        public void RemoveVertex(LayoutVertexBase layoutVertex)
        {
            var layerIndex = _vertexToLayerIndexMap[layoutVertex];
            RemoveVertexFromLayer(layoutVertex, layerIndex);
        }

        public void EnsureValidLayering(LayoutVertexBase childVertex, LayoutVertexBase parentVertex)
        {
            var childVertexLayerIndex = childVertex.LayerIndex;
            var parentVertexLayerIndex = parentVertex.LayerIndex;

            if (childVertexLayerIndex > parentVertexLayerIndex)
                EnsureItemIndexIsValid(childVertex);
            else
                MoveVertexToLayer(childVertex, parentVertexLayerIndex + 1);
        }

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        private void AddVertexToLayer(LayoutVertexBase layoutVertex, int layerIndex)
        {
            var layer = EnsureLayerExists(layerIndex);
            _vertexToLayerIndexMap.Add(layoutVertex, layerIndex);
            layer.Add(layoutVertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private void RemoveVertexFromLayer(LayoutVertexBase layoutVertex, int layerIndex)
        {
            _layers[layerIndex].Remove(layoutVertex);
            _vertexToLayerIndexMap.Remove(layoutVertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private static void EnsureItemIndexIsValid(LayoutVertexBase layoutVertex)
        {
            if (!layoutVertex.IsLayerItemIndexValid)
                layoutVertex.RearrangeItemInLayer();
        }

        private void MoveVertexToLayer(LayoutVertexBase layoutVertex, int toLayerIndex)
        {
            RemoveVertexFromLayer(layoutVertex, layoutVertex.LayerIndex);
            AddVertexToLayer(layoutVertex, toLayerIndex);
        }

        private LayoutVertexLayer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
                _layers.Add(new LayoutVertexLayer(i));

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
