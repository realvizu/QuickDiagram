namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.SimplifiedSugiyama
{
    internal class LayoutVertex
    {
        public IExtent OriginalVertex { get; }
        public int Rank { get; set; }
        public DiagramPoint Center { get; set; }

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
        public double Width => OriginalVertex?.Width ?? 0d;
        public double Height => OriginalVertex?.Height ?? 0d;
        public DiagramRect Rect => new DiagramRect(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);

        public override string ToString()
        {
            return OriginalVertex?.ToString() ?? "(dummy)";
        }

        public void HorizontalTranslateTo(double newHorizontalCenter)
        {
            Center = new DiagramPoint(newHorizontalCenter, Center.Y);
        }
    }
}
