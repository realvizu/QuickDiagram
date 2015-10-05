using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.SimplifiedSugiyama
{
    internal class LayoutVertex
    {
        public ISized OriginalVertex { get; }
        public int Rank { get; set; }
        public Point2D Center { get; set; }

        private LayoutVertex(ISized originalVertex)
        {
            OriginalVertex = originalVertex;
        }

        public static LayoutVertex Create(ISized originalVertex)
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
        public Rect2D Rect => new Rect2D(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);
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
            Center = new Point2D(newHorizontalCenter, Center.Y);
        }

        public void HorizontalTranslateCenterBy(double translateAmount)
        {
            Center = new Point2D(Center.X + translateAmount, Center.Y);
        }
    }
}
