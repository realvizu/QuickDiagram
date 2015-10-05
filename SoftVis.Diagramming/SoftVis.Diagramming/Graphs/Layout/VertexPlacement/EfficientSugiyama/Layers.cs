using System.Collections;
using System.Collections.Generic;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    /// <summary>
    /// A collection of layers, where each layer is aware of its index.
    /// </summary>
    internal class Layers : IEnumerable<Layer>
    {
        private readonly List<Layer> _layers;

        internal Layers(IEnumerable<IEnumerable<SugiVertex>> sugiVertexClusters)
        {
            _layers = new List<Layer>();

            foreach (var sugiVertexCluster in sugiVertexClusters)
                Add(sugiVertexCluster);
        }

        public int Count => _layers.Count;
        public Layer this[int index] => _layers[index];
        public Layer this[SugiVertex item] => _layers[item.LayerIndex];

        public IEnumerator<Layer> GetEnumerator()
        {
            return _layers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Add(IEnumerable<SugiVertex> newLayerItems)
        {
            var newLayer = new Layer(newLayerItems, _layers.Count);
            _layers.Add(newLayer);
        }

        public void MoveItem(SugiVertex item, int newLayerIndex)
        {
            _layers[item.LayerIndex].RemoveItem(item);
            _layers[newLayerIndex].AddItem(item);
        }
    }
}
