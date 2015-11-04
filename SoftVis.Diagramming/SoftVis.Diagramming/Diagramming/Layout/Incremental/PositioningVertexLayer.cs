using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An ordered list of positioning vertices that belong to the same horizontal layer.
    /// </summary>
    internal class PositioningVertexLayer : IReadOnlyPositioningVertexLayer
    {
        private readonly IReadOnlyPositioningGraph _positioningGraph;
        private readonly IComparer<PositioningVertexBase> _vertexComparer;
        private readonly List<PositioningVertexBase> _items;
        public int LayerIndex { get; }
        public double Top { get; set; }

        internal PositioningVertexLayer(IReadOnlyPositioningGraph positioningGraph, int layerIndex)
        {
            _positioningGraph = positioningGraph;
            _vertexComparer = new VerticesInLayerComparer(positioningGraph);
            _items = new List<PositioningVertexBase>();

            LayerIndex = layerIndex;
            Top = double.MinValue;
        }

        public double Height => _items.Count == 0 ? 0 : _items.Max(i => i.Height);
        public double Bottom => Top + Height;
        public double CenterY => Top + Height / 2;
        public Rect2D Rect => _items.Where(i => !i.IsFloating).Select(i => i.Rect).Union();

        public PositioningVertexBase this[int i] => _items[i];
        public int Count => _items.Count;
        public int IndexOf(PositioningVertexBase vertex) => _items.IndexOf(vertex);
        public IEnumerator<PositioningVertexBase> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

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

            var vertexParent = _positioningGraph.GetPrimaryParent(vertex);

            if (vertexParent == null)
                return _items.Count;

            var siblingsInLayer = GetPrimarySiblingsInSameLayer(vertex).OrderBy(IndexOf).ToArray();
            if (!siblingsInLayer.Any())
                return _items.Count;

            var nextSiblingInLayer = siblingsInLayer.FirstOrDefault(i => Precedes(vertex, i));
            return nextSiblingInLayer != null
                ? _items.IndexOf(nextSiblingInLayer)
                : _items.IndexOf(siblingsInLayer.Last()) + 1;
        }

        private IEnumerable<PositioningVertexBase> GetPrimarySiblingsInSameLayer(PositioningVertexBase vertex)
        {
            return _positioningGraph.GetPrimarySiblings(vertex).Where(_items.Contains);
        }

        private bool Precedes(PositioningVertexBase vertex1, PositioningVertexBase vertex2)
        {
            return _vertexComparer.Compare(vertex1, vertex2) < 0;
        }
    }
}
