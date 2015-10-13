using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An ordered list of layout vertices that belong to the same horizontal layer.
    /// </summary>
    /// <remarks>
    /// Ordering is based on LayoutVertex' CompareTo implementation (node name).
    /// </remarks>
    internal class DiagramLayer : IEnumerable<LayoutVertex>
    {
        public int LayerIndex { get; }
        public double Top { get; set; }
        private readonly List<LayoutVertex> _items;

        internal DiagramLayer(int layerIndex)
        {
            LayerIndex = layerIndex;
            _items = new List<LayoutVertex>();
        }

        public int Count => _items.Count;
        public IEnumerator<LayoutVertex> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public double Height => _items.Count == 0 ? 0 : _items.Max(i => i.Height);
        public double Bottom => Top + Height;
        public double CenterY => Top + Height / 2;
        public Rect2D Rect => _items.Where(i => !i.IsFloating).Select(i => i.Rect).Union();

        public void Add(LayoutVertex layoutVertex)
        {
            var itemIndex = DetermineItemIndex(layoutVertex);
            _items.Insert(itemIndex, layoutVertex);
        }

        public void Remove(LayoutVertex layoutVertex)
        {
            _items.Remove(layoutVertex);
        }

        public LayoutVertex GetPrevious(LayoutVertex layoutVertex)
        {
            var index = _items.IndexOf(layoutVertex);
            return index == 0 ? null : _items[index - 1];
        }

        public LayoutVertex GetNext(LayoutVertex layoutVertex)
        {
            var index = _items.IndexOf(layoutVertex);
            return index == Count - 1 ? null : _items[index + 1];
        }

        private int DetermineItemIndex(LayoutVertex insertedVertex)
        {
            var insertedVertexParent = insertedVertex.GetParent();

            var index = 0;
            foreach (var existingVertex in _items)
            {
                var existingVertexParent = existingVertex.GetParent();

                var noParentOrSameParent = insertedVertexParent == null || existingVertexParent == insertedVertexParent;
                if (noParentOrSameParent && insertedVertex.Precedes(existingVertex))
                    break;

                var differentParents = insertedVertexParent != null && existingVertexParent != insertedVertexParent;
                if (differentParents && insertedVertexParent.Precedes(existingVertexParent))
                    break;

                index++;
            }

            return index;
        }
    }
}
