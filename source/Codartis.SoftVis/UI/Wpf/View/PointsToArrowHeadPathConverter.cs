using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Creates a path that represents an arrow head (without the shaft).
    /// </summary>
    public sealed class PointsToArrowHeadPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 ||
                !(values[0] is IList<Point>) ||
                !(values[1] is ArrowHeadType) ||
                !(values[2] is double) ||
                !(values[3] is double))
                return null;

            var routePoints = (IList<Point>) values[0];
            var arrowHeadType = (ArrowHeadType) values[1];
            var arrowHeadSize = (double) values[2];
            var arrowHeadLengthPerWidth = (double) values[3];

            if (routePoints.Count < 1)
                return null;

            var pathFigures = CreatePathFigures(routePoints, arrowHeadType, arrowHeadSize, arrowHeadLengthPerWidth);
            return new PathGeometry(pathFigures);
        }

        private static IEnumerable<PathFigure> CreatePathFigures(IList<Point> points,
            ArrowHeadType arrowHeadType, double arrowHeadSize, double arrowHeadLengthPerWidth)
        {
            var startPoint = points[points.Count - 2];
            var endPoint = points[points.Count - 1];

            var arrowVector = endPoint - startPoint;
            var arrowHeadVector = arrowVector / arrowVector.Length * arrowHeadSize;

            switch (arrowHeadType)
            {
                case ArrowHeadType.Hollow:
                    yield return CreateArrowHead(endPoint, arrowHeadVector, arrowHeadLengthPerWidth, isClosed: true);
                    yield break;
                case ArrowHeadType.Simple:
                    yield return CreateArrowHead(endPoint, arrowHeadVector, arrowHeadLengthPerWidth, isClosed: false);
                    yield break;
            }
        }

        private static PathFigure CreateArrowHead(Point arrowEndPoint,
            Vector arrowHeadVector, double arrowHeadLengthPerWidth, bool isClosed)
        {
            var arrowHeadWidthVector = new Vector(arrowHeadVector.Y, -arrowHeadVector.X) / arrowHeadLengthPerWidth;

            var arrowHeadPoints = new[]
            {
                arrowEndPoint - arrowHeadVector - arrowHeadWidthVector,
                arrowEndPoint,
                arrowEndPoint - arrowHeadVector + arrowHeadWidthVector
            };

            return arrowHeadPoints.ToPathFigure(isClosed);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}