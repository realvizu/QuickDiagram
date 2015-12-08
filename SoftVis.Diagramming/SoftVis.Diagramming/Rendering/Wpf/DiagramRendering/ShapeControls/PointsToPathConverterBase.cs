using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    /// <summary>
    /// Creates a path from the supplied Point[] that is relative to the supplied Point.
    /// </summary>
    public abstract class PointsToPathConverterBase : IMultiValueConverter
    {
        protected const double ArrowHeadSize = 10;
        protected const double ArrowHeadWidthPerLength = 0.5;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value.Length != 2 ||
                !(value[0] is Point[]) ||
                !(value[1] is Point))
                throw new ArgumentException($"{typeof(PointsToPathConverterBase)} expects these parameters: Point[], Point.");

            var routePoints = (Point[])value[0];
            var topLeftPoint = (Point)value[1];

            routePoints = MakePointsRelativeToPosition(routePoints, topLeftPoint);
            var pathFigures = CreatePathFigures(routePoints);
            return new PathGeometry(pathFigures);
        }

        protected abstract IEnumerable<PathFigure> CreatePathFigures(Point[] routePoints);

        private static Point[] MakePointsRelativeToPosition(Point[] routePoints, Point referencePoint)
        {
            var translate = -(Vector)referencePoint;
            return routePoints.Select(i => i + translate).ToArray();
        }

        protected static PathFigure CreatePathFigure(Point[] points, bool closed)
        {
            var segments = points.Skip(1).Select(i => new LineSegment(i, true));
            return new PathFigure(points.First(), segments, closed);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
