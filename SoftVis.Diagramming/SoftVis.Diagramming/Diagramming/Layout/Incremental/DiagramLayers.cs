using System.Collections;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates a layering from a layout graph that gives the basis for vertical position calculation.
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

        public void EnsureVertexIsUnderParentVertex(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            var childVertexLayerIndex = _vertexToLayerIndexMap[childVertex];
            var parentVertexLayerIndex = _vertexToLayerIndexMap[parentVertex];

            if (childVertexLayerIndex <= parentVertexLayerIndex)
                MoveVertexBetweenLayers(childVertex, childVertexLayerIndex, parentVertexLayerIndex + 1);
        }

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        //public void RemoveVertex(LayoutVertex layoutVertex)
        //{
        //    var layerIndex = _vertexToLayerIndexMap[layoutVertex];
        //    RemoveVertexFromLayer(layoutVertex, layerIndex);
        //}

        //public void RemoveEdge(LayoutEdge layoutEdge)
        //{
        //    var vertexToMove = layoutEdge.Source;
        //    var vertexLayerIndex = _vertexToLayerIndexMap[vertexToMove];
        //    var parentLayerIndex = GetMaxParentLayerIndex(vertexToMove) ?? -1;
        //    MoveVertexBetweenLayers(vertexToMove, vertexLayerIndex, parentLayerIndex + 1);
        //}

        //private int? GetMaxParentLayerIndex(LayoutVertex layoutVertex)
        //{
        //    var parentVertices = layoutVertex.GetParents().ToList();
        //    return parentVertices.Any()
        //        ? parentVertices.Where(i => _vertexToLayerIndexMap.ContainsKey(i)).Max(i => _vertexToLayerIndexMap[i])
        //        : (int?)null;
        //}

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

        private void MoveVertexBetweenLayers(LayoutVertex layoutVertex, int fromLayerIndex, int toLayerIndex)
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
