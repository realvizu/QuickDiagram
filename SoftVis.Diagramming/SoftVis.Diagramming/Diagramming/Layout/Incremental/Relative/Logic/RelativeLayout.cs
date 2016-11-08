namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Summarizes the data structures that make up the relative layout and provides operations on them.
    /// </summary>
    internal sealed class RelativeLayout : IReadOnlyRelativeLayout
    {
        private readonly LayeredLayoutGraph _layeredLayoutGraph;
        private readonly LayoutVertexLayers _layers;

        public RelativeLayout(LayeredLayoutGraph layeredLayoutGraph, 
            LayoutVertexLayers layers)
        {
            _layeredLayoutGraph = layeredLayoutGraph;
            _layers = layers;
        }

        public IReadOnlyLayeredLayoutGraph LayeredLayoutGraph => _layeredLayoutGraph;
        public IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph => _layeredLayoutGraph.ProperGraph;
        public IReadOnlyLayoutVertexLayers LayoutVertexLayers => _layers;
    }
}
