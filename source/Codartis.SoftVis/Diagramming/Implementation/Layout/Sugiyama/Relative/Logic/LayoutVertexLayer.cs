using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// An ordered list of vertices that belong to the same horizontal layer.
    /// </summary>
    internal class LayoutVertexLayer : IReadOnlyLayoutVertexLayer
    {
        private readonly List<LayoutVertexBase> _items;
        public int LayerIndex { get; }

        internal LayoutVertexLayer(int layerIndex)
        {
            _items = new List<LayoutVertexBase>();
            LayerIndex = layerIndex;
        }

        private IEnumerable<LayoutVertexBase> Items => _items;

        public int Count => Items.Count();
        public LayoutVertexBase this[int i] => Items.ElementAt(i);
        public int IndexOf(LayoutVertexBase vertex) => Items.IndexOf(vertex);
        public IEnumerator<LayoutVertexBase> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        public void Add(LayoutVertexBase vertex, int index)
        {
            _items.Insert(index, vertex);
        }

        public void Remove(LayoutVertexBase vertex)
        {
            _items.Remove(vertex);
        }

        public override string ToString()
        {
            var itemsAsString = string.Join(",", Items.Select(i => i.ToString()));
            return $"[{itemsAsString}]";
        }
    }
}
