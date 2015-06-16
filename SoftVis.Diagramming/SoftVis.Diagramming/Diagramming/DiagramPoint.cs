using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("( {X} , {Y} )")]
    public struct DiagramPoint
    {
        private readonly double _x;
        private readonly double _y;

        public static DiagramPoint Zero = new DiagramPoint(0, 0);

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }

        public DiagramPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public static bool Equals(DiagramPoint point1, DiagramPoint point2)
        {
            return point1._x.Equals(point2._x) &&
                   point1._y.Equals(point2._y);
        }

        public bool Equals(DiagramPoint other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if ((null == obj) || !(obj is DiagramPoint))
            {
                return false;
            }

            var value = (DiagramPoint)obj;
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _x.GetHashCode() ^ _y.GetHashCode();
            }
        }

        public static bool operator ==(DiagramPoint left, DiagramPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiagramPoint left, DiagramPoint right)
        {
            return !left.Equals(right);
        }
    }
}
