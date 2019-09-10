using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.Util;
using Codartis.Util.UI.Wpf;
using MoreLinq;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Creates a path that represents an arrow shaft (without the head).
    /// </summary>
    public sealed class PointsToArrowShaftPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3 ||
                !(values[0] is IList<Point>) ||
                !(values[1] is ArrowHeadType) ||
                !(values[2] is double))
                return null;

            var routePoints = (IList<Point>) values[0];
            var arrowHeadType = (ArrowHeadType) values[1];
            var arrowHeadSize = (double) values[2];

            if (routePoints.Count < 1)
                return null;

            var pathFigures = CreatePathFigures(routePoints, arrowHeadType, arrowHeadSize);
            return new PathGeometry(pathFigures);
        }

        private static IEnumerable<PathFigure> CreatePathFigures(IList<Point> routePoints, ArrowHeadType arrowHeadType, double arrowHeadSize)
        {
            if (routePoints.Count > 2)
                yield return CreatePathFigureWithoutLastSegment(routePoints);

            var lastSegment = routePoints.TakeLast(2).ToArray();

            switch (arrowHeadType)
            {
                case ArrowHeadType.Hollow:
                    yield return CreateArrowShaftForHollowHead(lastSegment, arrowHeadSize);
                    yield break;
                case ArrowHeadType.Simple:
                    yield return CreateArrowShaftForSimpleHead(lastSegment);
                    yield break;
            }
        }

        private static PathFigure CreatePathFigureWithoutLastSegment(IList<Point> points)
        {
            return points.TakeButLast().ToPathFigure(closed: false);
        }

        private static PathFigure CreateArrowShaftForHollowHead(IList<Point> points, double arrowHeadSize)
        {
            var startPoint = points[points.Count - 2];
            var endPoint = points[points.Count - 1];

            var arrowVector = endPoint - startPoint;
            var arrowHeadVector = arrowVector / arrowVector.Length * arrowHeadSize;

            var shaftPoints = new[]
            {
                startPoint,
                endPoint - arrowHeadVector
            };

            return shaftPoints.ToPathFigure(closed: false);
        }

        private static PathFigure CreateArrowShaftForSimpleHead(IList<Point> points)
        {
            var startPoint = points[points.Count - 2];
            var endPoint = points[points.Count - 1];

            var shaftPoints = new[]
            {
                startPoint,
                endPoint
            };

            return shaftPoints.ToPathFigure(closed: false);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}