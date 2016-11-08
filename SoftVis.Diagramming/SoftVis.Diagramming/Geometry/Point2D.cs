using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Represents a point in 2D space.
    /// </summary>
    public struct Point2D
    {
        public double X { get; }
        public double Y { get; }

        public static Point2D Undefined = new Point2D(double.NaN, double.NaN);

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool IsDefined => !IsUndefined;
        public bool IsUndefined => double.IsNaN(X) || double.IsNaN(Y);

        public static bool Equals(Point2D point1, Point2D point2)
        {
            return point1.X.Equals(point2.X) &&
                   point1.Y.Equals(point2.Y);
        }

        public bool Equals(Point2D other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point2D)) return false;

            var value = (Point2D)obj;
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"({X:0.##};{Y:0.##})";
        }

        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point2D left, Point2D right)
        {
            return !left.Equals(right);
        }

        public static Point2D operator +(Point2D left, Point2D right)
        {
            return new Point2D(left.X + right.X, left.Y + right.Y);
        }

        public static Point2D operator -(Point2D left, Point2D right)
        {
            return new Point2D(left.X - right.X, left.Y - right.Y);
        }

        public static Point2D operator *(Point2D point, double multiplier)
        {
            return new Point2D(point.X * multiplier, point.Y * multiplier);
        }

        public static Point2D operator *(double multiplier, Point2D point)
        {
            return point * multiplier;
        }

        public bool IsEqualWithTolerance(Point2D point)
        {
            return X.IsEqualWithTolerance(point.X) && Y.IsEqualWithTolerance(point.Y);
        }
    }
}
