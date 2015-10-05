using System.Diagnostics;

namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Represents a width + height pair.
    /// </summary>
    [DebuggerDisplay("( {Width} x {Height} )")]
    public struct Size2D
    {
        public static readonly Size2D Empty = new Size2D(0, 0);

        public double Width { get; }
        public double Height { get; }

        public Size2D(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public static bool Equals(Size2D size1, Size2D size2)
        {
            return size1.Width.Equals(size2.Width) &&
                   size1.Height.Equals(size2.Height);
        }

        public bool Equals(Size2D other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size2D)) return false;

            var value = (Size2D)obj;
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Width.GetHashCode() ^ Height.GetHashCode();
            }
        }

        public static bool operator ==(Size2D left, Size2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Size2D left, Size2D right)
        {
            return !left.Equals(right);
        }
    }
}
