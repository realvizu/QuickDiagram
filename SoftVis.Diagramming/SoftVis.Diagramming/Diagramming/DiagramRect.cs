using System;
using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("( {TopLeft.X}, {TopLeft.Y} )  ( {Size.Width}, {Size.Height} )")]
    public struct DiagramRect
    {
        public DiagramPoint TopLeft { get; private set; }
        public DiagramSize Size { get; private set; }

        public DiagramRect(DiagramPoint topLeft, DiagramSize size)
        {
            TopLeft = topLeft;
            Size = size;
        }

        public DiagramRect(DiagramPoint topLeft, DiagramPoint bottomRight)
        {
            TopLeft = topLeft;
            Size = new DiagramSize(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }

        public DiagramRect(double left, double top, double right, double bottom)
            : this(new DiagramPoint(left, top), new DiagramPoint(right, bottom))
        {
        }

        public double Top
        {
            get { return TopLeft.Y; }
        }

        public double Bottom
        {
            get { return TopLeft.Y + Size.Height; }
        }

        public double Left
        {
            get { return TopLeft.X; }
        }

        public double Right
        {
            get { return TopLeft.X + Size.Width; }
        }

        public DiagramPoint BottomRight
        {
            get { return new DiagramPoint(Right, Bottom); }
        }

        public DiagramPoint Center
        {
            get { return new DiagramPoint(Left + Size.Width / 2, Top + Size.Height / 2); }
        }

        public double Width
        {
            get { return Size.Width; }
        }

        public double Height
        {
            get { return Size.Height; }
        }

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
