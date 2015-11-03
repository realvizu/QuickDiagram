using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An ordered list of positioning vertices that belong to the same horizontal layer.
    /// </summary>
    /// <remarks>
    /// Ordering is based on the vertices' CompareTo implementation (node name).
    /// </remarks>
    internal class PositioningVertexLayer : IReadOnlyPositioningVertexLayer
    {
        private readonly List<PositioningVertexBase> _items;
        public int LayerIndex { get; }
        public double Top { get; set; }

        internal PositioningVertexLayer(int layerIndex)
        {
            LayerIndex = layerIndex;
            _items = new List<PositioningVertexBase>();
        }

        public PositioningVertexBase this[int i] => _items[i];
        public int Count => _items.Count;
        public int IndexOf(PositioningVertexBase vertex) => _items.IndexOf(vertex);
        public IEnumerator<PositioningVertexBase> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public double Height => _items.Count == 0 ? 0 : _items.Max(i => i.Height);
        public double Bottom => Top + Height;
        public double CenterY => Top + Height / 2;
        public Rect2D Rect => _items.Where(i => !i.IsFloating).Select(i => i.Rect).Union();

        public void Add(PositioningVertexBase vertex)
        {
            var itemIndex = DetermineItemIndex(vertex);
            _items.Insert(itemIndex, vertex);
        }

        public void Remove(PositioningVertexBase vertex)
        {
            _items.Remove(vertex);
        }

        public PositioningVertexBase GetPrevious(PositioningVertexBase vertex)
        {
            var index = _items.IndexOf(vertex);
            return index == 0 ? null : _items[index - 1];
        }

        public PositioningVertexBase GetNext(PositioningVertexBase vertex)
        {
            var index = _items.IndexOf(vertex);
            return index == Count - 1 ? null : _items[index + 1];
        }

        public bool IsItemIndexValid(PositioningVertexBase vertex)
        {
            return IndexOf(vertex) == DetermineItemIndex(vertex);
        }

        private int DetermineItemIndex(PositioningVertexBase vertex)
        {
            // TODO: handle the case of different parents

            var vertexParent = vertex.GetPrimaryParent();

            if (vertexParent == null)
                return _items.Count;

            var siblingsInLayer = vertex.GetPrimarySiblingsInSameLayer().OrderBy(IndexOf).ToArray();
            if (!siblingsInLayer.Any())
                return _items.Count;

            var nextSiblingInLayer = siblingsInLayer.FirstOrDefault(vertex.Precedes);
            return nextSiblingInLayer != null
                ? _items.IndexOf(nextSiblingInLayer)
                : _items.IndexOf(siblingsInLayer.Last()) + 1;
        }
    }
}
