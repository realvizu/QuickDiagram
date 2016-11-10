using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    // TODO: refactor: separate connection point calculation, rect-relative calculation, general geometry.
    public sealed class PointsToRectRelativeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //if (values.Length != 4 || 
            //    !(values[0] is IList<Point>) ||
            //    !(values[1] is ArrowHeadType) ||
            //    !(values[2] is double) ||
            //    !(values[3] is double))
            //    return null;

            var points = (Point[])values[0];
            var boundingRect = (Rect)values[1];
            var sourceRect = (Rect) values[2];
            var targetRect = (Rect)values[3];

            if (boundingRect.IsEmpty || sourceRect.IsEmpty || targetRect.IsEmpty 
                || IsUndefined(boundingRect) || IsUndefined(sourceRect) || IsUndefined(targetRect))
                return null;

            var point1 = GetAttachPointOfRectTowardPoint(sourceRect, targetRect.GetCenter());
            var point2 = GetAttachPointOfRectTowardPoint(targetRect, sourceRect.GetCenter());

            var result = MakePointsRelativeToRect(new []{point1, point2}, boundingRect);

            return result;
        }

        private static bool IsUndefined(Rect rect)
        {
            return double.IsNaN(rect.Left) || double.IsNaN(rect.Top) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height);
        }

        private static Point[] MakePointsRelativeToRect(Point[] points, Rect boundingRect)
        {
            var rectRelativeTranslate = -(Vector) boundingRect.TopLeft;
            var routePoints = points?.Select(i => i + rectRelativeTranslate).ToArray();
            var result = routePoints == null || routePoints.Any(i => i.IsUndefined()) ? null : routePoints;
            return result;
        }

        private static Point GetAttachPointOfRectTowardPoint(Rect rect, Point targetPoint)
        {
            var center = rect.GetCenter();
            var sides = new[]
            {
                (rect.Left - targetPoint.X)/(center.X - targetPoint.X),
                (rect.Top - targetPoint.Y)/(center.Y - targetPoint.Y),
                (rect.Right - targetPoint.X)/(center.X - targetPoint.X),
                (rect.Bottom - targetPoint.Y)/(center.Y - targetPoint.Y)
            };

            var fi = Math.Max(0, sides.Where(i => i <= 1).Max());

            return targetPoint + fi * (center - targetPoint);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
