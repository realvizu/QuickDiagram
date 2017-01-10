using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    public static class PointExtensions
    {
        public static Point Undefined = new Point(double.NaN, double.NaN);

        public static bool IsUndefined(this Point point)
        {
            return double.IsNaN(point.X) || double.IsNaN(point.Y)
                || double.IsInfinity(point.X) || double.IsInfinity(point.Y);
        }

        public static PathFigure ToPathFigure(this IEnumerable<Point> points, bool closed)
        {
            var enumerable = points as Point[] ?? points.ToArray();
            var segments = enumerable.Skip(1).Select(i => new LineSegment(i, true));
            return new PathFigure(enumerable.First(), segments, closed);
        }

        public static Rect ToRect(this Point point) => new Rect(point, new Size(0, 0));

        public static Rect BoundingRect(this IEnumerable<Point> points)
        {
            if (points == null)
                return Rect.Empty;

            var pointArray = points as Point[] ?? points.ToArray();

            if (pointArray.Length == 0)
                return Rect.Empty;

            var left = pointArray.Select(i => i.X).Min();
            var top = pointArray.Select(i => i.Y).Min();
            var right = pointArray.Select(i => i.X).Max();
            var bottom = pointArray.Select(i => i.Y).Max();

            return new Rect(new Point(left, top), new Point(right, bottom));
        }

        public static Point[] MakeRelativeTo(this IEnumerable<Point> points, Point refrencePoint)
        {
            var translate = -(Vector)refrencePoint;
            return points.Select(i => i + translate).ToArray();
        }
    }
}
