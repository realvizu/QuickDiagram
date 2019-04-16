using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Codartis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Calculates diagram connector route points from the view model's route points, source node rect, target node rect.
    /// The first and last points are modified to always follow the source/target rect's movement.
    /// All points are translated to be relative to the given point.
    /// </summary>
    public sealed class DiagramConnectorRouteCalculatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 ||
                !(values[0] is Point[]) ||
                !(values[1] is Rect) ||
                !(values[2] is Rect) ||
                !(values[3] is Point))
                return null;

            var routePoints = (Point[])values[0];
            var sourceRect = (Rect)values[1];
            var targetRect = (Rect)values[2];
            var referencePoint = (Point)values[3];

            if (sourceRect.IsEmpty || targetRect.IsEmpty ||
                referencePoint.IsUndefined() || sourceRect.IsUndefined() || targetRect.IsUndefined() ||
                routePoints == null || routePoints.Length < 2 || routePoints.Any(i => i.IsUndefined()))
                return null;

            var routePointCount = routePoints.Length;
            routePoints[0] = sourceRect.GetPerimeterPointTowardPoint(routePoints[1]);
            routePoints[routePointCount - 1] = targetRect.GetPerimeterPointTowardPoint(routePoints[routePointCount - 2]);

            return routePoints.MakeRelativeTo(referencePoint);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
