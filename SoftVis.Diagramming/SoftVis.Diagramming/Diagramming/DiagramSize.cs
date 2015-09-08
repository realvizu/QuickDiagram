using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Represents a width + height pair.
    /// </summary>
    [DebuggerDisplay("( {Width} x {Height} )")]
    public struct DiagramSize
    {
        public double Width { get; }
        public double Height { get; }

        public DiagramSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public static bool Equals(DiagramSize size1, DiagramSize size2)
        {
            return size1.Width.Equals(size2.Width) &&
                   size1.Height.Equals(size2.Height);
        }

        public bool Equals(DiagramSize other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DiagramSize)) return false;

            var value = (DiagramSize)obj;
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Width.GetHashCode() ^ Height.GetHashCode();
            }
        }

        public static bool operator ==(DiagramSize left, DiagramSize right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiagramSize left, DiagramSize right)
        {
            return !left.Equals(right);
        }
    }
}
