using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    /// <summary>
    /// A rank layer is an ordered list of LayoutVertices with the same rank.
    /// Each item is aware of which layer it belongs to (has a Rank property).
    /// The order of the items determines their horizontal order in the layout.
    /// </summary>
    internal class RankLayer : IEnumerable<LayoutVertex>
    {
        private readonly List<LayoutVertex> _items;
        private readonly int _layerIndex;

        public int Count => _items.Count;
        public LayoutVertex this[int i] => _items[i];

        public double Height { get; private set; }
        public double CenterY { get; private set; }

        internal RankLayer(IEnumerable<LayoutVertex> items, int layerIndex)
        {
            _layerIndex = layerIndex;
            _items = items as List<LayoutVertex> ?? items.ToList();
            _items.ForEach(i => i.Rank = layerIndex);
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
            item.Rank = _layerIndex;
        }

        public void RemoveItem(LayoutVertex item)
        {
            _items.Remove(item);
        }

        public void CalculateHeight()
        {
            Height = _items.Max(i => i.Height);
        }

        public void CalculateVerticalPosition(double? previousLayerPosition, double verticalGap)
        {
            CenterY = previousLayerPosition + Height + verticalGap ?? 0;
        }

        public void CalculateVertexPositions(double horizontalGap)
        {
            var x = 0d;
            foreach (var layoutVertex in _items)
            {
                var vertexWidth = layoutVertex.OriginalVertex?.Width ?? 0;

                layoutVertex.Center = new DiagramPoint( x + vertexWidth / 2, CenterY);

                x += vertexWidth + horizontalGap;
            }
        }
    }
}
