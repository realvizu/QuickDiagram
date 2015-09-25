namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    internal class LayoutVertex
    {
        private IExtent OriginalVertex { get; }
        public int Rank { get; set; }

        private LayoutVertex(IExtent originalVertex)
        {
            OriginalVertex = originalVertex;
        }

        public static LayoutVertex Create(IExtent originalVertex)
        {
            return new LayoutVertex(originalVertex);
        }

        public static LayoutVertex CreateDummy()
        {
            return new LayoutVertex(null);
        }

        public bool IsDummy => OriginalVertex == null;
        public double Height => OriginalVertex?.Height ?? 0d;

        public override string ToString()
        {
            return OriginalVertex?.ToString() ?? "(dummy)";
        }
    }
}
