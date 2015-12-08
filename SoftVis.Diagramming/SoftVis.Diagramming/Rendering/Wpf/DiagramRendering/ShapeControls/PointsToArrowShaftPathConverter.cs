using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Common;
using MoreLinq;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    /// <summary>
    /// Creates a path that represents an arrow shaft (without the head).
    /// </summary>
    public sealed class PointsToArrowShaftPathConverter : PointsToPathConverterBase
    {
        protected override IEnumerable<PathFigure> CreatePathFigures(Point[] routePoints)
        {
            if (routePoints.Length > 2)
                yield return CreatePathFigureWithoutLastSegment(routePoints);

            var lastSegment = routePoints.TakeLast(2).ToArray();
            yield return CreateGeneralizationArrowShaft(lastSegment);
        }

        private static PathFigure CreatePathFigureWithoutLastSegment(Point[] points)
        {
            return CreatePathFigure(points.TakeButLast().ToArray(), false);
        }

        private static PathFigure CreateGeneralizationArrowShaft(Point[] points)
        {
            var startPoint = points[points.Length - 2];
            var endPoint = points[points.Length - 1];

            var arrowVector = endPoint - startPoint;
            var arrowHeadVector = arrowVector / arrowVector.Length * ArrowHeadSize;

            var shaftPoints = new[]
            {
                startPoint,
                endPoint - arrowHeadVector,
            };

            return CreatePathFigure(shaftPoints, false);
        }
    }
}
