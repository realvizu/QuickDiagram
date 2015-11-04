using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Abstract base class for vertices used in a layout graph. 
    /// </summary>
    /// <remarks>
    /// <para>LayoutVertices can float which indicates that the edge does not take part in layout calculations.</para>
    /// </remarks>
    internal abstract class LayoutVertexBase : IRect
    {
        public bool IsFloating { get; set; }
        public Point2D Center { get; set; }

        protected LayoutVertexBase(bool isFloating)
        {
            IsFloating = isFloating;
            Center = Point2D.Empty;
        }

        public abstract bool IsDummy { get; }
        public abstract string Name { get; }
        public abstract int Priority { get; }

        public abstract Size2D Size { get; }
        public double Width => Size.Width;
        public double Height => Size.Height;

        public Rect2D Rect => new Rect2D(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);
        public double Left => Rect.Left;
        public double Right => Rect.Right;
        public double Top => Rect.Top;
        public double Bottom => Rect.Bottom;
    }
}
