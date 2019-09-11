using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// An ordered collection of LayoutVertexLayer items.
    /// </summary>
    internal class LayoutVertexLayers : IReadOnlyLayoutVertexLayers
    {
        private readonly List<LayoutVertexLayer> _layers;
        private readonly Map<LayoutVertexBase, int> _vertexToLayerIndexMap;

        public LayoutVertexLayers()
        {
            _layers = new List<LayoutVertexLayer>();
            _vertexToLayerIndexMap = new Map<LayoutVertexBase, int>();
        }

        private IEnumerable<IReadOnlyLayoutVertexLayer> Layers => _layers;

        public int Count => Layers.Count();
        public IEnumerator<IReadOnlyLayoutVertexLayer> GetEnumerator() => Layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
        }

        public void AddVertex(LayoutVertexBase vertex, RelativeLocation targetLocation)
        {
            var layer = EnsureLayerExists(targetLocation.LayerIndex);
            _vertexToLayerIndexMap.Set(vertex, targetLocation.LayerIndex);

            layer.Add(vertex, targetLocation.IndexInLayer);
        }

        public void RemoveVertex(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            _layers[layerIndex].Remove(vertex);
            _vertexToLayerIndexMap.Remove(vertex);
        }

        public int GetLayerIndex(LayoutVertexBase vertex)
        {
            return _vertexToLayerIndexMap.Get(vertex);
        }

        public int GetIndexInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).IndexOf(vertex);
        }

        public bool HasLocation(LayoutVertexBase vertex)
        {
            return _vertexToLayerIndexMap.Contains(vertex);
        }

        public RelativeLocation GetLocation(LayoutVertexBase vertex)
        {
            return new RelativeLocation(GetLayerIndex(vertex), GetIndexInLayer(vertex));
        }

        public IReadOnlyLayoutVertexLayer GetLayer(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            return Layers.ElementAt(layerIndex);
        }

        public IReadOnlyLayoutVertexLayer GetLayer(int index)
        {
            EnsureLayerExists(index);
            return Layers.ElementAt(index);
        }

        private LayoutVertexLayer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
            {
                var newLayer = new LayoutVertexLayer(i);
                _layers.Add(newLayer);
            }
            return _layers[layerIndex];
        }
    }
}
