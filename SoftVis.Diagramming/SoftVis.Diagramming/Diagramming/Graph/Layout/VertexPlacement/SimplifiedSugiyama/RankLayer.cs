using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.SimplifiedSugiyama
{
    /// <summary>
    /// A rank layer is an ordered list of LayoutVertices with the same rank.
    /// Each item is aware of which layer it belongs to (has a Rank property).
    /// The order of the items determines their horizontal order in the layout.
    /// </summary>
    internal class RankLayer : IEnumerable<LayoutVertex>
    {
        private readonly List<LayoutVertex> _items;
        public int Rank { get; }
        public double HorizontalGap { get; }

        public int Count => _items.Count;
        public LayoutVertex this[int i] => _items[i];

        public double Height { get; private set; }
        public double CenterY { get; private set; }

        internal RankLayer(int rank, double horizontalGap, IEnumerable<LayoutVertex> items = null)
        {
            _items = new List<LayoutVertex>();
            Rank = rank;
            HorizontalGap = horizontalGap;

            if (items != null)
                foreach (var layoutVertex in items)
                    AddItem(layoutVertex);
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
            item.Rank = Rank;
        }

        public void InsertItem(int index, LayoutVertex item)
        {
            _items.Insert(index, item);
            item.Rank = Rank;
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

        public void CalculateVertexPositions()
        {
            var x = 0d;
            foreach (var layoutVertex in _items)
            {
                layoutVertex.Center = new DiagramPoint(x + layoutVertex.Width / 2, CenterY);
                x += layoutVertex.Width + HorizontalGap;
            }
        }

        public void HorizontalTranslateAtLeastTo(LayoutVertex layoutVertex, double newHorizontalCenter)
        {
            Debug.WriteLine($"{((DiagramNode)layoutVertex.OriginalVertex)?.Name} from {layoutVertex.Center.X} to {newHorizontalCenter}");

            if (layoutVertex.Center.X >= newHorizontalCenter)
                return;

            layoutVertex.HorizontalTranslateTo(newHorizontalCenter);

            var vertexIndexInLayer = _items.IndexOf(layoutVertex);
            if (vertexIndexInLayer < _items.Count - 1)
            {
                var nextVertex = _items[vertexIndexInLayer + 1];
                var nextVertexNewHorizontalCenter = layoutVertex.Rect.Right + HorizontalGap + nextVertex.Width / 2;
                HorizontalTranslateAtLeastTo(nextVertex, nextVertexNewHorizontalCenter);
            }
        }
    }
}
