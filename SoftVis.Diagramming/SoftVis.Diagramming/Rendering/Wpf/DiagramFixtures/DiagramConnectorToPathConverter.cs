using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public class DiagramConnectorToPathConverter : IMultiValueConverter
    {
        private const double ArrowHeadSize = 10;
        private const double ArrowHeadWidthPerLength = 0.5;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(values != null && values.Length == 4, "DiagramConnectorToPathConverter should have 4 parameters: Rect, SourceRect, TargetRect, RoutePoints.");
            Debug.Assert(values[0] != DependencyProperty.UnsetValue, "Rect parameter is mandatory.");
            Debug.Assert(values[1] != DependencyProperty.UnsetValue, "SourceRect parameter is mandatory.");
            Debug.Assert(values[2] != DependencyProperty.UnsetValue, "TargetRect parameter is mandatory.");

            var rect = (Rect)values[0];
            var sourceRect = (Rect)values[1];
            var targetRect = (Rect)values[2];
            var routeInformation = (Point[])values[3];

            if (routeInformation == null || routeInformation.Length < 2)
                routeInformation = CreateRouteInformationFromSourceRectAndTargetRect(sourceRect, targetRect);

            routeInformation = MakePositionsRelativeToBoundingRect(routeInformation, rect);
            routeInformation = AlignEndpointsToNodeSides(routeInformation, sourceRect.Size, targetRect.Size);

            var pathFigureCollection = new PathFigureCollection();

            if (routeInformation.Length > 2)
                pathFigureCollection.Add(CreateConnectorWithoutLastSegment(routeInformation));

            pathFigureCollection.Add(CreateGeneralizationArrow(routeInformation.TakeLast(2)));

            return pathFigureCollection;
        }

        private static Point[] CreateRouteInformationFromSourceRectAndTargetRect(Rect sourceRect, Rect targetRect)
        {
            return new[] { sourceRect.GetCenter(), targetRect.GetCenter() };
        }

        private static Point[] MakePositionsRelativeToBoundingRect(Point[] routeInformation, Rect boundingRect)
        {
            var translate = -(Vector)boundingRect.TopLeft;
            return routeInformation.Select(i => i + translate).ToArray();
        }

        private static Point[] AlignEndpointsToNodeSides(Point[] routeInformation, Size sourceNodeSize, Size targetNodeSize)
        {
            var lastIndex = routeInformation.Length - 1;
            routeInformation[0] = CalculateAttachPoint(routeInformation[0], routeInformation[1], sourceNodeSize);
            routeInformation[lastIndex] = CalculateAttachPoint(routeInformation[lastIndex], routeInformation[lastIndex - 1], targetNodeSize);
            return routeInformation;
        }

        private static Point CalculateAttachPoint(Point sourcePoint, Point targetPoint, Size sourceNodeSize)
        {
            double[] sides = new double[4];
            sides[0] = (sourcePoint.X - sourceNodeSize.Width / 2.0 - targetPoint.X) / (sourcePoint.X - targetPoint.X);
            sides[1] = (sourcePoint.Y - sourceNodeSize.Height / 2.0 - targetPoint.Y) / (sourcePoint.Y - targetPoint.Y);
            sides[2] = (sourcePoint.X + sourceNodeSize.Width / 2.0 - targetPoint.X) / (sourcePoint.X - targetPoint.X);
            sides[3] = (sourcePoint.Y + sourceNodeSize.Height / 2.0 - targetPoint.Y) / (sourcePoint.Y - targetPoint.Y);

            double fi = 0;
            for (int i = 0; i < 4; i++)
            {
                if (sides[i] <= 1)
                    fi = Math.Max(fi, sides[i]);
            }

            return targetPoint + fi * (sourcePoint - targetPoint);
        }

        private static PathFigure CreatePathFigure(IEnumerable<Point> points, bool closed)
        {
            var segments = points.Skip(1).Select(i => new LineSegment(i, true));
            return new PathFigure(points.First(), segments, closed);
        }

        private static PathFigure CreateConnectorWithoutLastSegment(IEnumerable<Point> points)
        {
            return CreatePathFigure(points.TakeButLast(), false);
        }

        private static IEnumerable<PathFigure> CreateGeneralizationArrow(IEnumerable<Point> points)
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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
