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

        public static Vector ToVector(this Point2D point2D)
        {
            return new Vector(point2D.X, point2D.Y);
        }
    }
}
