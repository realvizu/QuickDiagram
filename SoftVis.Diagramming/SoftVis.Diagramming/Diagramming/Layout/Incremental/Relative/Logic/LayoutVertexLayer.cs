using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// An ordered list of vertices that belong to the same horizontal layer.
    /// TODO: move height calculation logic to VerticalPositionLogic class?
    /// </summary>
    internal class LayoutVertexLayer : IReadOnlyLayoutVertexLayer
    {
        private readonly List<LayoutVertexBase> _items;
        public int LayerIndex { get; }
        public double Top { get; set; }

        internal LayoutVertexLayer(int layerIndex)
        {
            _items = new List<LayoutVertexBase>();
            LayerIndex = layerIndex;
            Top = double.MinValue;
        }

        private IEnumerable<LayoutVertexBase> Items => _items;

        public double Height => Items.Any() ? Items.Max(i => i.Height) : 0;
        public double Bottom => Top + Height;
        public double CenterY => Top + Height / 2;

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

        public LayoutVertexBase GetPrevious(LayoutVertexBase vertex)
        {
            var index = Items.IndexOf(vertex);
            return index == 0 ? null : Items.ElementAt(index - 1);
        }

        public LayoutVertexBase GetNext(LayoutVertexBase vertex)
        {
            var index = Items.IndexOf(vertex);
            return index == Count - 1 ? null : Items.ElementAt(index + 1);
        }

        public override string ToString()
        {
            var itemsAsString = string.Join(",", Items.Select(i => i.ToString()));
            return $"[{itemsAsString}]";
        }
    }
}
