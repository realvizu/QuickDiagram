using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Rendering.Wpf.Common;
using MoreLinq;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    /// <summary>
    /// Creates a path from the supplied Point[] that is relative to the supplied Rect (given with size and position).
    /// </summary>
    public class PointsToPathConverter : IMultiValueConverter
    {
        private const double ArrowHeadSize = 10;
        private const double ArrowHeadWidthPerLength = 0.5;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value.Length != 3 ||
                !(value[0] is Point[]) ||
                !(value[1] is Size) ||
                !(value[2] is Point))
                throw new ArgumentException($"{typeof(PointsToPathConverter)} expects these parameters: Point[], Size, Point.");

            var routePoints = (Point[])value[0];
            var boundingRect = new Rect((Point) value[2], (Size) value[1]);

            routePoints = MakePointsRelativeToBoundingRect(routePoints, boundingRect);

            var pathFigureCollection = new PathFigureCollection();

            if (routePoints.Length > 2)
                pathFigureCollection.Add(CreatePathFigureWithoutLastSegment(routePoints));

            pathFigureCollection.Add(CreateGeneralizationArrow(routePoints.TakeLast(2).ToArray()));

            return new PathGeometry(pathFigureCollection);
        }

        private static Point[] MakePointsRelativeToBoundingRect(Point[] routePoints, Rect boundingRect)
        {
            var translate = -(Vector)boundingRect.TopLeft;
            return routePoints.Select(i => i + translate).ToArray();
        }

        private static PathFigure CreatePathFigure(Point[] points, bool closed)
        {
            var segments = points.Skip(1).Select(i => new LineSegment(i, true));
            return new PathFigure(points.First(), segments, closed);
        }

        private static PathFigure CreatePathFigureWithoutLastSegment(Point[] points)
        {
            return CreatePathFigure(points.TakeButLast().ToArray(), false);
        }

        private static IEnumerable<PathFigure> CreateGeneralizationArrow(Point[] points)
        {
            var startPoint = points.First();
            var endPoint = points.Last();

            var arrowVector = endPoint - startPoint;
            var arrowHeadVector = (arrowVector / arrowVector.Length) * ArrowHeadSize;

            var shaftPoints = new[]
            {
                startPoint,
                endPoint - arrowHeadVector,
            };

            yield return CreatePathFigure(shaftPoints, false);
            yield return CreateGeneralizationArrowHead(endPoint, arrowHeadVector);
        }

        private static PathFigure CreateGeneralizationArrowHead(Point arrowEndPoint, Vector arrowHeadVector)
        {
            var arrowHeadWidthVector = new Vector(arrowHeadVector.Y, -arrowHeadVector.X) * ArrowHeadWidthPerLength;

            var arrowHeadPoints = new[]
            {
                arrowEndPoint - arrowHeadVector - arrowHeadWidthVector,
                arrowEndPoint,
                arrowEndPoint - arrowHeadVector + arrowHeadWidthVector,
            };

            return CreatePathFigure(arrowHeadPoints, true);
        }
                
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
