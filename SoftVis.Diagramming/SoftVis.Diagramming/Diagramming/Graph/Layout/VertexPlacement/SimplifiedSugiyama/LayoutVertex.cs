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
        public double Left => Rect.Left;
        public double Right => Rect.Right;
        public double Top => Rect.Top;
        public double Bottom => Rect.Bottom;

        public override string ToString()
        {
            return OriginalVertex?.ToString() ?? "(dummy)";
        }

        public void HorizontalTranslateCenterTo(double newHorizontalCenter)
        {
            Center = new DiagramPoint(newHorizontalCenter, Center.Y);
        }

        public void HorizontalTranslateCenterBy(double translateAmount)
        {
            Center = new DiagramPoint(Center.X + translateAmount, Center.Y);
        }
    }
}
