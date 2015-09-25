namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    internal class LayoutVertex
    {
        private IExtent OriginalVertex { get; }
        public int LayerIndex { get; set; }

        public LayoutVertex(IExtent originalVertex)
        {
            OriginalVertex = originalVertex;
        }

        public double Height => OriginalVertex?.Height ?? 0d;

        public override string ToString()
        {
            return OriginalVertex.ToString();
        }
    }
}
