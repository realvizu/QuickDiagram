using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    /// <summary>
    /// A layer has an ordered list of items and some other properties like height and position.
    /// Each item is aware of which layer it belongs to (has a LayerIndex property).
    /// </summary>
    internal class Layer : IEnumerable<SugiVertex>
    {
        private readonly List<SugiVertex> _items;
        private readonly int _layerIndex;

        public int Count => _items.Count;
        public SugiVertex this[int i] => _items[i];

        public double Height { get; private set; }
        public double Position { get; private set; }

        internal Layer(IEnumerable<SugiVertex> items, int layerIndex)
        {
            _layerIndex = layerIndex;
            _items = items as List<SugiVertex> ?? items.ToList();
            _items.ForEach(i => i.LayerIndex = layerIndex);
        }

        public IEnumerator<SugiVertex> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddItem(SugiVertex item)
        {
            _items.Add(item);
            item.LayerIndex = _layerIndex;
        }

        public void RemoveItem(SugiVertex item)
        {
            _items.Remove(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void UpdateHight()
        {
            Height = _items.Max(i => i.Height);
        }

        public void UpdatePosition(double? previousLayerPosition, double layerDistance)
        {
            Position = previousLayerPosition + Height + layerDistance ?? 0;
        }
    }
}
