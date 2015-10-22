using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// An ordered list of layout vertices that belong to the same horizontal layer.
    /// </summary>
    /// <remarks>
    /// Ordering is based on LayoutVertex' CompareTo implementation (node name).
    /// </remarks>
    internal class LayoutVertexLayer : IEnumerable<LayoutVertexBase>
    {
        public int LayerIndex { get; }
        public double Top { get; set; }
        private readonly List<LayoutVertexBase> _items;

        internal LayoutVertexLayer(int layerIndex)
        {
            LayerIndex = layerIndex;
            _items = new List<LayoutVertexBase>();
        }

        public LayoutVertexBase this[int i] => _items[i];
        public int Count => _items.Count;
        public int IndexOf(LayoutVertexBase layoutVertex) => _items.IndexOf(layoutVertex);
        public IEnumerator<LayoutVertexBase> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public double Height => _items.Count == 0 ? 0 : _items.Max(i => i.Height);
        public double Bottom => Top + Height;
        public double CenterY => Top + Height / 2;
        public Rect2D Rect => _items.Where(i => !i.IsFloating).Select(i => i.Rect).Union();

        public void Add(LayoutVertexBase layoutVertex)
        {
            var itemIndex = DetermineItemIndex(layoutVertex);
            _items.Insert(itemIndex, layoutVertex);
        }

        public void Remove(LayoutVertexBase layoutVertex)
        {
            _items.Remove(layoutVertex);
        }

        public LayoutVertexBase GetPrevious(LayoutVertexBase layoutVertex)
        {
            var index = _items.IndexOf(layoutVertex);
            return index == 0 ? null : _items[index - 1];
        }

        public LayoutVertexBase GetNext(LayoutVertexBase layoutVertex)
        {
            var index = _items.IndexOf(layoutVertex);
            return index == Count - 1 ? null : _items[index + 1];
        }

        public bool IsItemIndexValid(LayoutVertexBase layoutVertex)
        {
            return IndexOf(layoutVertex) == DetermineItemIndex(layoutVertex);
        }

        private int DetermineItemIndex(LayoutVertexBase insertedVertex)
        {
            var insertedVertexParent = insertedVertex.GetPrimaryParent();
            var insertedVertexParentIndexInLayer = insertedVertexParent?.GetIndexInLayer();

            var index = 0;
            foreach (var existingVertex in _items.Except(insertedVertex.ToEnumerable()))
            {
                var existingVertexParent = existingVertex.GetPrimaryParent();

                var noParentOrSameParent = insertedVertexParent == null || existingVertexParent == insertedVertexParent;
                if (noParentOrSameParent && insertedVertex.Precedes(existingVertex))
                    break;

                var differentParents = insertedVertexParent != null && existingVertexParent != insertedVertexParent;
                if (differentParents && existingVertexParent.GetIndexInLayer() > insertedVertexParentIndexInLayer.Value)
                    break;

                index++;
            }

            return index;
        }
    }
}
