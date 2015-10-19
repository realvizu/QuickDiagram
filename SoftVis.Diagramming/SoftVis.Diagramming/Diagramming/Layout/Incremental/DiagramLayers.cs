using System.Collections;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Tracks vertex layering and calculates vertical positions.
    /// </summary>
    internal class DiagramLayers : IEnumerable<DiagramLayer>
    {
        private readonly double _verticalGap;
        private readonly List<DiagramLayer> _layers;
        private readonly Dictionary<LayoutVertex, int> _vertexToLayerIndexMap;

        public DiagramLayers(double verticalGap)
        {
            _verticalGap = verticalGap;

            _layers = new List<DiagramLayer>();
            _vertexToLayerIndexMap = new Dictionary<LayoutVertex, int>();
        }

        public IEnumerator<DiagramLayer> GetEnumerator() => _layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int GetLayerIndex(LayoutVertex layoutVertex)
        {
            return _vertexToLayerIndexMap[layoutVertex];
        }

        public DiagramLayer GetLayer(LayoutVertex layoutVertex)
        {
            var layerIndex = GetLayerIndex(layoutVertex);
            return _layers[layerIndex];
        }

        public void AddVertex(LayoutVertex layoutVertex)
        {
            AddVertexToLayer(layoutVertex, 0);
        }

        public void RemoveVertex(LayoutVertex layoutVertex)
        {
            var layerIndex = _vertexToLayerIndexMap[layoutVertex];
            RemoveVertexFromLayer(layoutVertex, layerIndex);
        }

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        private void AddVertexToLayer(LayoutVertex layoutVertex, int layerIndex)
        {
            var layer = EnsureLayerExists(layerIndex);
            _vertexToLayerIndexMap.Add(layoutVertex, layerIndex);
            layer.Add(layoutVertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private void RemoveVertexFromLayer(LayoutVertex layoutVertex, int layerIndex)
        {
            _layers[layerIndex].Remove(layoutVertex);
            _vertexToLayerIndexMap.Remove(layoutVertex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        public void MoveVertexBetweenLayers(LayoutVertex layoutVertex, int fromLayerIndex, int toLayerIndex)
        {
            layoutVertex.IsFloating = true;
            RemoveVertexFromLayer(layoutVertex, fromLayerIndex);
            AddVertexToLayer(layoutVertex, toLayerIndex);
        }

        private DiagramLayer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
                _layers.Add(new DiagramLayer(i));

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
