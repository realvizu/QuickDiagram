using System;
using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Represents a rectangle with a given position and size.
    /// </summary>
    [DebuggerDisplay("( {TopLeft.X}, {TopLeft.Y} )  ( {Size.Width}, {Size.Height} )")]
    public struct DiagramRect
    {
        public DiagramPoint TopLeft { get; }
        public DiagramSize Size { get; }

        public DiagramRect(DiagramPoint topLeft, DiagramSize size)
        {
            TopLeft = topLeft;
            Size = size;
        }

        public DiagramRect(DiagramPoint topLeft, DiagramPoint bottomRight)
            : this(topLeft, new DiagramSize(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y))
        {
        }

        public DiagramRect(double left, double top, double right, double bottom)
            : this(new DiagramPoint(left, top), new DiagramPoint(right, bottom))
        {
        }

        public double Top => TopLeft.Y;
        public double Bottom => TopLeft.Y + Size.Height;
        public double Left => TopLeft.X;
        public double Right => TopLeft.X + Size.Width;
        public double Width => Size.Width;
        public double Height => Size.Height;

        public DiagramPoint BottomRight => new DiagramPoint(Right, Bottom);
        public DiagramPoint Center => new DiagramPoint(Left + Size.Width / 2, Top + Size.Height / 2);

        public static DiagramRect Union(DiagramRect rect1, DiagramRect rect2)
        {
            var left = Math.Min(rect1.Left, rect2.Left);
            var top = Math.Min(rect1.Top, rect2.Top);

            var right = Math.Max(rect1.Right, rect2.Right);
            var bottom = Math.Max(rect1.Bottom, rect2.Bottom);

            return new DiagramRect(new DiagramPoint(left, top), new DiagramPoint(right, bottom));
        }
    }
}
