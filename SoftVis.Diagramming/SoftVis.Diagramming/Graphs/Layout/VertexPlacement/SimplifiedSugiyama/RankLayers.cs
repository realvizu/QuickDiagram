using System.Collections;
using System.Collections.Generic;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.SimplifiedSugiyama
{
    /// <summary>
    /// An ordered list of rank layers.
    /// </summary>
    internal class RankLayers : IEnumerable<RankLayer>
    {
        private readonly List<RankLayer> _layers;
        public double HorizontalGap { get; }
        public double VerticalGap { get; }

        internal RankLayers(double horizontalGap, double verticalGap, IEnumerable<IEnumerable<LayoutVertex>> itemsByLayers = null)
        {
            _layers = new List<RankLayer>();
            HorizontalGap = horizontalGap;
            VerticalGap = verticalGap;

            if (itemsByLayers != null)
                foreach (var itemsByLayer in itemsByLayers)
                    AddLayer(itemsByLayer);
        }

        public int Count => _layers.Count;
        public RankLayer this[int index] => _layers[index];
        public RankLayer this[LayoutVertex item] => _layers[item.Rank];

        public IEnumerator<RankLayer> GetEnumerator()
        {
            return _layers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public RankLayer AddLayer(IEnumerable<LayoutVertex> newLayerItems = null)
        {
            var newLayer = new RankLayer(_layers.Count, HorizontalGap, newLayerItems);
            _layers.Add(newLayer);
            return newLayer;
        }

        public void CalculatePositions()
        {
            double? previousLayerPosition = null;
            foreach (var layer in _layers)
            {
                layer.CalculateHeight();
                layer.CalculateVerticalPosition(previousLayerPosition, VerticalGap);
                layer.CalculateVertexPositions();
                previousLayerPosition = layer.CenterY;
            }
        }
    }
}
