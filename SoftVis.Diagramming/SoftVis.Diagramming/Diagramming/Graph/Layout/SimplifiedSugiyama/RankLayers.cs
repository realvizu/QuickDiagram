using System.Collections;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    /// <summary>
    /// An ordered list of rank layers.
    /// </summary>
    internal class RankLayers : IEnumerable<RankLayer>
    {
        private readonly List<RankLayer> _layers;

        internal RankLayers(IEnumerable<IEnumerable<LayoutVertex>> layoutVerticeClusters)
        {
            _layers = new List<RankLayer>();

            foreach (var layoutVerticeCluster in layoutVerticeClusters)
                Add(layoutVerticeCluster);
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

        public void CalculatePositions(double horizontalGap, double verticalGap)
        {
            double? previousLayerPosition = null;
            foreach (var layer in _layers)
            {
                layer.CalculateHeight();
                layer.CalculateVerticalPosition(previousLayerPosition, verticalGap);
                layer.CalculateVertexPositions(horizontalGap);
                previousLayerPosition = layer.CenterY;
            }
        }

        private void Add(IEnumerable<LayoutVertex> newLayerItems)
        {
            var newLayer = new RankLayer(newLayerItems, _layers.Count);
            _layers.Add(newLayer);
        }
    }
}
