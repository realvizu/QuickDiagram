using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// Calculates a layering from a layout graph that gives the basis for vertical position calculation.
    /// </summary>
    internal class Layers
    {
        private readonly LayoutGraph _layoutGraph;
        private readonly double _verticalGap;
        private readonly List<Layer> _layers;
        private readonly Dictionary<LayoutVertex, int> _vertexToLayerIndexMap;

        public Layers(LayoutGraph layoutGraph, double verticalGap)
        {
            _layoutGraph = layoutGraph;
            _layoutGraph.VertexAdded += OnVertexAdded;
            _layoutGraph.VertexRemoved += OnVertexRemoved;
            _layoutGraph.EdgeAdded += OnEdgeAdded;
            _layoutGraph.EdgeRemoved += OnEdgeRemoved;
            _layoutGraph.Cleared += OnCleared;

            _verticalGap = verticalGap;

            _layers = new List<Layer>();
            _vertexToLayerIndexMap = new Dictionary<LayoutVertex, int>();
        }

        public Layer First()
        {
            EnsureLayerExists(0);
            return _layers[0];
        }

        public int GetLayerIndex(LayoutVertex layoutVertex)
        {
            return _vertexToLayerIndexMap[layoutVertex];
        }

        public Layer GetLayer(LayoutVertex layoutVertex)
        {
            var layerIndex = GetLayerIndex(layoutVertex);
            return _layers[layerIndex];
        }

        public IEnumerable<LayoutVertex> GetOtherNonFloatingVerticesInLayer(LayoutVertex layoutVertex)
        {
            return GetLayer(layoutVertex).Where(i => i != layoutVertex && !i.IsFloating);
        }

        public double GetCenterY(LayoutVertex layoutVertex)
        {
            return GetLayer(layoutVertex).CenterY;
        }

        private void OnVertexAdded(LayoutVertex layoutVertex)
        {
            AddVertexToLayer(layoutVertex, 0);
        }

        private void OnVertexRemoved(LayoutVertex layoutVertex)
        {
            var index = _vertexToLayerIndexMap[layoutVertex];
            RemoveVertexFromLayer(layoutVertex, index);
        }

        private void OnEdgeAdded(LayoutEdge layoutEdge)
        {
            _layoutGraph.ExecuteOnTree(layoutEdge.Source, layoutEdge.Target, EdgeDirection.In, EnsureVertexIsUnderParentVertex);
        }

        private void OnEdgeRemoved(LayoutEdge layoutEdge)
        {
            var vertexToMove = layoutEdge.Source;
            var vertexLayerIndex = _vertexToLayerIndexMap[vertexToMove];
            var parentLayerIndex = GetMaxParentLayerIndex(vertexToMove) ?? -1;
            MoveVertexBetweenLayers(vertexToMove, vertexLayerIndex, parentLayerIndex + 1);
        }

        private void OnCleared(object sender, EventArgs e)
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        private int? GetMaxParentLayerIndex(LayoutVertex layoutVertex)
        {
            var parentVertices = _layoutGraph.GetOutNeighbours(layoutVertex).ToList();
            return parentVertices.Any()
                ? parentVertices.Where(i => _vertexToLayerIndexMap.ContainsKey(i)).Max(i => _vertexToLayerIndexMap[i])
                : (int?)null;
        }

        private void EnsureVertexIsUnderParentVertex(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            var childVertexLayerIndex = _vertexToLayerIndexMap[childVertex];
            var parentVertexLayerIndex = _vertexToLayerIndexMap[parentVertex];

            if (childVertexLayerIndex <= parentVertexLayerIndex)
                MoveVertexBetweenLayers(childVertex, childVertexLayerIndex, parentVertexLayerIndex + 1);
        }

        private void AddVertexToLayer(LayoutVertex layoutVertex, int layerIndex)
        {
            var layer = EnsureLayerExists(layerIndex);
            layer.Add(layoutVertex);
            _vertexToLayerIndexMap.Add(layoutVertex, layerIndex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private void RemoveVertexFromLayer(LayoutVertex layoutVertex, int layerIndex)
        {
            _layers[layerIndex].Remove(layoutVertex);
            _vertexToLayerIndexMap.Remove(layoutVertex);

            if (_layers[layerIndex].Count == 0)
                _layers.RemoveAt(layerIndex);

            UpdateLayerVerticalPositions(layerIndex);
        }

        private void MoveVertexBetweenLayers(LayoutVertex layoutVertex, int fromLayerIndex, int toLayerIndex)
        {
            RemoveVertexFromLayer(layoutVertex, fromLayerIndex);
            AddVertexToLayer(layoutVertex, toLayerIndex);
        }

        private Layer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
                _layers.Add(new Layer(i));

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
