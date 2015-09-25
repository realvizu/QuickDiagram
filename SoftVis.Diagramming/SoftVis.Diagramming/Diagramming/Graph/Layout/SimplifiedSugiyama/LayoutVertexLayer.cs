using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    /// <summary>
    /// A layer has an ordered list of LayoutVertices and some other properties like height and position.
    /// Each item is aware of which layer it belongs to (has a LayerIndex property).
    /// </summary>
    internal class LayoutVertexLayer : IEnumerable<LayoutVertex>
    {
        private readonly List<LayoutVertex> _items;
        private readonly int _layerIndex;

        public int Count => _items.Count;
        public LayoutVertex this[int i] => _items[i];

        public double Height { get; private set; }
        public double Position { get; private set; }

        internal LayoutVertexLayer(IEnumerable<LayoutVertex> items, int layerIndex)
        {
            _layerIndex = layerIndex;
            _items = items as List<LayoutVertex> ?? items.ToList();
            _items.ForEach(i => i.LayerIndex = layerIndex);
        }

        public IEnumerator<LayoutVertex> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddItem(LayoutVertex item)
        {
            _items.Add(item);
            item.LayerIndex = _layerIndex;
        }

        public void RemoveItem(LayoutVertex item)
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
