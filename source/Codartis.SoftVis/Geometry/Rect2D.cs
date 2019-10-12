using System;
using System.Linq;

namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Represents a rectangle with a given position and size in 2D space.
    /// </summary>
    public struct Rect2D
    {
        public static readonly Rect2D Undefined = new Rect2D(double.NaN, double.NaN, double.NaN, double.NaN);
        public static readonly Rect2D Zero = new Rect2D(0, 0, 0, 0);

        public Point2D TopLeft { get; }
        public Size2D Size { get; }

        public Rect2D(Point2D topLeft, Size2D size)
        {
            TopLeft = topLeft;
            Size = size;
        }

        public Rect2D(Point2D topLeft, Point2D bottomRight)
            : this(topLeft, new Size2D(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y))
        {
        }

        public Rect2D(double left, double top, double right, double bottom)
            : this(new Point2D(left, top), new Point2D(right, bottom))
        {
        }

        public Rect2D(Size2D size)
            : this(new Point2D(0, 0), size)
        {
        }

        public Rect2D(double width, double height)
            : this(new Size2D(width, height))
        {
        }

        public double Top => TopLeft.Y;
        public double Bottom => TopLeft.Y + Size.Height;
        public double Left => TopLeft.X;
        public double Right => TopLeft.X + Size.Width;
        public double Width => Size.Width;
        public double Height => Size.Height;

        public Point2D BottomRight => new Point2D(Right, Bottom);
        public Point2D Center => new Point2D(Left + Size.Width / 2, Top + Size.Height / 2);
        public Point2D Position => TopLeft;
        public bool IsEmpty => Size == Size2D.Zero;

        public static Rect2D CreateFromCenterAndSize(Point2D center, Size2D size)
        {
            var halfWidth = size.Width / 2;
            var halfHeight = size.Height / 2;
            return new Rect2D(center.X - halfWidth, center.Y - halfHeight, center.X + halfWidth, center.Y + halfHeight);
        }

        public static Rect2D Union(Rect2D rect1, Rect2D rect2)
        {
            if (rect1.IsUndefined())
                return rect2;

            if (rect2.IsUndefined())
                return rect1;

            var left = Math.Min(rect1.Left, rect2.Left);
            var top = Math.Min(rect1.Top, rect2.Top);

            var right = Math.Max(rect1.Right, rect2.Right);
            var bottom = Math.Max(rect1.Bottom, rect2.Bottom);

            return new Rect2D(new Point2D(left, top), new Point2D(right, bottom));
        }

        public static Rect2D Intersect(Rect2D rect1, Rect2D rect2)
        {
            if (rect1.IsUndefined() || rect2.IsUndefined())
                return Undefined;

            var left = Math.Max(rect1.Left, rect2.Left);
            var top = Math.Max(rect1.Top, rect2.Top);

            var right = Math.Min(rect1.Right, rect2.Right);
            var bottom = Math.Min(rect1.Bottom, rect2.Bottom);

            return left >= right || top >= bottom
                ? Undefined
                : new Rect2D(new Point2D(left, top), new Point2D(right, bottom));
        }

        public Point2D GetAttachPointToward(Point2D targetPoint)
        {
            var center = Center;
            var sides = new[]
            {
                (Left - targetPoint.X) / (center.X - targetPoint.X),
                (Top - targetPoint.Y) / (center.Y - targetPoint.Y),
                (Right - targetPoint.X) / (center.X - targetPoint.X),
                (Bottom - targetPoint.Y) / (center.Y - targetPoint.Y)
            };

            var fi = Math.Max(0, sides.Where(i => i <= 1).Max());

            return targetPoint + fi * (center - targetPoint);
        }

        public bool Equals(Rect2D other)
        {
            return Size.Equals(other.Size) && Center.Equals(other.Center);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Rect2D && Equals((Rect2D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TopLeft.GetHashCode() * 397) ^ Size.GetHashCode();
            }
        }

        public static bool operator ==(Rect2D left, Rect2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rect2D left, Rect2D right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"[TopLeft:{TopLeft}|Size:{Size}]";
        }

        public Rect2D WithSize(Size2D newSize) => new Rect2D(TopLeft, newSize);

        public Rect2D WithCenter(Point2D newCenter) => CreateFromCenterAndSize(newCenter, Size);

        public Rect2D WithTopLeft(Point2D newTopLeft) => new Rect2D(newTopLeft, Size);

        public Rect2D WithMargin(double margin) => new Rect2D(Left - margin, Top - margin, Right + margin, Bottom + margin);
    }
}