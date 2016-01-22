using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
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
                value.Length != 3 ||
                (value[0] != null && !(value[0] is IList<Point>)) ||
                !(value[1] is double) ||
                !(value[2] is double))
                throw new ArgumentException($"{typeof(PointsToPathConverterBase)} expects these parameters: IList<Point>, double, double.");

            if (value[0] == null)
                return null;

            var routePoints = (IList<Point>)value[0];
            var topLeftPoint = new Point((double) value[1], (double) value[2]);

            routePoints = MakePointsRelativeToPosition(routePoints, topLeftPoint);
            var pathFigures = CreatePathFigures(routePoints);
            return new PathGeometry(pathFigures);
        }

        protected abstract IEnumerable<PathFigure> CreatePathFigures(IList<Point> routePoints);

        private static IList<Point> MakePointsRelativeToPosition(IList<Point> routePoints, Point referencePoint)
        {
            var translate = -(Vector)referencePoint;
            return routePoints.Select(i => i + translate).ToArray();
        }

        protected static PathFigure CreatePathFigure(IList<Point> points, bool closed)
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
