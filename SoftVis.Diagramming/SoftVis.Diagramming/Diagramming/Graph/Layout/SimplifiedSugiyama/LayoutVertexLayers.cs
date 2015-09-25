using System.Collections;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    /// <summary>
    /// An ordered list of layers.
    /// </summary>
    internal class LayoutVertexLayers : IEnumerable<LayoutVertexLayer>
    {
        private readonly List<LayoutVertexLayer> _layers;

        internal LayoutVertexLayers(IEnumerable<IEnumerable<LayoutVertex>> layoutVerticeClusters)
        {
            _layers = new List<LayoutVertexLayer>();

            foreach (var layoutVerticeCluster in layoutVerticeClusters)
                Add(layoutVerticeCluster);
        }

        public int Count => _layers.Count;
        public LayoutVertexLayer this[int index] => _layers[index];
        public LayoutVertexLayer this[LayoutVertex item] => _layers[item.LayerIndex];

        public IEnumerator<LayoutVertexLayer> GetEnumerator()
        {
            return _layers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Add(IEnumerable<LayoutVertex> newLayerItems)
        {
            var newLayer = new LayoutVertexLayer(newLayerItems, _layers.Count);
            _layers.Add(newLayer);
        }

        public void MoveItem(LayoutVertex item, int newLayerIndex)
        {
            _layers[item.LayerIndex].RemoveItem(item);
            _layers[newLayerIndex].AddItem(item);
        }
    }
}
