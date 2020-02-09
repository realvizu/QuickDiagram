using System.Windows;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI.Wpf
{
    public static class Point2DExtensions
    {
        public static Point ToWpf(this Point2D point2D)
        {
            return new Point(point2D.X, point2D.Y);
        }

        public static Point2D FromWpf(this Point size)
        {
            return new Point2D(size.X, size.Y);
        }

        public static Vector ToVector(this Point2D point2D)
        {
            return new Vector(point2D.X, point2D.Y);
        }
    }
}