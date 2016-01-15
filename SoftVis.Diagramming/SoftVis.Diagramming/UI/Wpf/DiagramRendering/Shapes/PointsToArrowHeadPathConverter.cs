using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes
{
    /// <summary>
    /// Creates a path that represents an arrow head (without the shaft).
    /// </summary>
    public sealed class PointsToArrowHeadPathConverter : PointsToPathConverterBase
    {
        protected override IEnumerable<PathFigure> CreatePathFigures(IList<Point> points)
        {
            var startPoint = points[points.Count - 2];
            var endPoint = points[points.Count - 1];

            var arrowVector = endPoint - startPoint;
            var arrowHeadVector = arrowVector / arrowVector.Length * ArrowHeadSize;

            yield return CreateGeneralizationArrowHead(endPoint, arrowHeadVector);
        }

        private static PathFigure CreateGeneralizationArrowHead(Point arrowEndPoint, Vector arrowHeadVector)
        {
            var arrowHeadWidthVector = new Vector(arrowHeadVector.Y, -arrowHeadVector.X) * ArrowHeadWidthPerLength;

            var arrowHeadPoints = new[]
            {
                arrowEndPoint - arrowHeadVector - arrowHeadWidthVector,
                arrowEndPoint,
                arrowEndPoint - arrowHeadVector + arrowHeadWidthVector
            };

            return CreatePathFigure(arrowHeadPoints, true);
        }
    }
}
