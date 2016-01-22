using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes;
using MoreLinq;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Creates a path that represents an arrow shaft (without the head).
    /// </summary>
    public sealed class PointsToArrowShaftPathConverter : PointsToPathConverterBase
    {
        protected override IEnumerable<PathFigure> CreatePathFigures(IList<Point> routePoints)
        {
            if (routePoints.Count > 2)
                yield return CreatePathFigureWithoutLastSegment(routePoints);

            var lastSegment = routePoints.TakeLast(2).ToArray();
            yield return CreateGeneralizationArrowShaft(lastSegment);
        }

        private static PathFigure CreatePathFigureWithoutLastSegment(IList<Point> points)
        {
            return CreatePathFigure(points.TakeButLast().ToArray(), false);
        }

        private static PathFigure CreateGeneralizationArrowShaft(IList<Point> points)
        {
            var startPoint = points[points.Count - 2];
            var endPoint = points[points.Count - 1];

            var arrowVector = endPoint - startPoint;
            var arrowHeadVector = arrowVector / arrowVector.Length * ArrowHeadSize;

            var shaftPoints = new[]
            {
                startPoint,
                endPoint - arrowHeadVector
            };

            return CreatePathFigure(shaftPoints, false);
        }
    }
}
