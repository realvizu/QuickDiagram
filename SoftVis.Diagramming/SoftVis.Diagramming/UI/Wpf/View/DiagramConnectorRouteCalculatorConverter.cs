using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Calculates diagram connector route points from the view model's route points, source node rect, target node rect and bounding rect.
    /// The first and last points are modified to always follow the source/target rect's movement.
    /// All points are translated to be relative to the bounding rect.
    /// </summary>
    public sealed class DiagramConnectorRouteCalculatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 ||
                !(values[0] is Point[]) ||
                !(values[1] is Rect) ||
                !(values[2] is Rect) ||
                !(values[3] is Rect))
                return null;

            var routePoints = (Point[])values[0];
            var sourceRect = (Rect)values[1];
            var targetRect = (Rect)values[2];
            var boundingRect = (Rect)values[3];

            if (boundingRect.IsEmpty || sourceRect.IsEmpty || targetRect.IsEmpty ||
                boundingRect.IsUndefined() || sourceRect.IsUndefined() || targetRect.IsUndefined() ||
                routePoints == null || routePoints.Length < 2)
                return null;

            var routePointCount = routePoints.Length;
            routePoints[0] = sourceRect.GetPerimeterPointTowardPoint(routePoints[1]);
            routePoints[routePointCount - 1] = targetRect.GetPerimeterPointTowardPoint(routePoints[routePointCount - 2]);

            return boundingRect.ToRelativePoints(routePoints);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
