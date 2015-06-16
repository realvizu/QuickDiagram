using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("( {Width} x {Height} )")]
    public struct DiagramSize
    {
        private readonly double _width;
        private readonly double _height;

        public double Width
        {
            get { return _width; }
        }

        public double Height
        {
            get { return _height; }
        }

        public DiagramSize(double x, double y)
        {
            _width = x;
            _height = y;
        }

        public DiagramSize(DiagramPoint point)
        {
            _width = point.X;
            _height = point.Y;
        }

        public static bool Equals(DiagramSize size1, DiagramSize size2)
        {
            return size1._width.Equals(size2._width) &&
                   size1._height.Equals(size2._height);
        }

        public bool Equals(DiagramSize other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if ((null == obj) || !(obj is DiagramSize))
            {
                return false;
            }

            var value = (DiagramSize)obj;
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _width.GetHashCode() ^ _height.GetHashCode();
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
