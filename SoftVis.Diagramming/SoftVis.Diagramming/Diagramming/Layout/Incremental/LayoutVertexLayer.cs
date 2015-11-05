using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An ordered list of vertices that belong to the same horizontal layer.
    /// </summary>
    internal class LayoutVertexLayer : IReadOnlyLayoutVertexLayer
    {
        private readonly IReadOnlyLayoutGraph _layoutGraph;
        private readonly List<LayoutVertexBase> _items;
        public int LayerIndex { get; }
        public double Top { get; set; }

        internal LayoutVertexLayer(IReadOnlyLayoutGraph layoutGraph, int layerIndex)
        {
            _layoutGraph = layoutGraph;
            _items = new List<LayoutVertexBase>();

            LayerIndex = layerIndex;
            Top = double.MinValue;
        }

        public double Height => _items.Count == 0 ? 0 : _items.Max(i => i.Height);
        public double Bottom => Top + Height;
        public double CenterY => Top + Height / 2;
        public Rect2D Rect => _items.Where(i => !i.IsFloating).Select(i => i.Rect).Union();

        public LayoutVertexBase this[int i] => _items[i];
        public int Count => _items.Count;
        public int IndexOf(LayoutVertexBase vertex) => _items.IndexOf(vertex);
        public IEnumerator<LayoutVertexBase> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public void Add(LayoutVertexBase vertex, int index)
        {
            _items.Insert(index, vertex);
        }

        public void Remove(LayoutVertexBase vertex)
        {
            _items.Remove(vertex);
        }

        public LayoutVertexBase GetPrevious(LayoutVertexBase vertex)
        {
            var index = _items.IndexOf(vertex);
            return index == 0 ? null : _items[index - 1];
        }

        public LayoutVertexBase GetNext(LayoutVertexBase vertex)
        {
            var index = _items.IndexOf(vertex);
            return index == Count - 1 ? null : _items[index + 1];
        }

        public override string ToString()
        {
            var itemsAsString = string.Join(",", _items.Select(i => i.ToString()));
            return $"[{itemsAsString}]";
        }
    }
}
