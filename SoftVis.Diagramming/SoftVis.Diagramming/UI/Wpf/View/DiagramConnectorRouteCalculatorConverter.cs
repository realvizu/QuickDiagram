using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Calculates diagram connector route points from the source node rect, interim points, target node rect and bounding rect.
    /// The first and last points are calculated on-the-fly to be on the source/target rect perimeter, toward the next point.
    /// All points are translated to be relative to the bounding rect.
    /// </summary>
    public sealed class DiagramConnectorRouteCalculatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 ||
                !(values[0] is Rect) ||
                !(values[1] is Point[]) ||
                !(values[2] is Rect) ||
                !(values[3] is Rect))
                return null;

            var sourceRect = (Rect) values[0];
            var interimRoutePoints = (Point[])values[1];
            var targetRect = (Rect)values[2];
            var boundingRect = (Rect)values[3];

            if (boundingRect.IsEmpty || sourceRect.IsEmpty || targetRect.IsEmpty ||
                boundingRect.IsUndefined() || sourceRect.IsUndefined() || targetRect.IsUndefined())
                return null;

            var secondPoint = interimRoutePoints.Any()
                ? interimRoutePoints.First()
                : targetRect.GetCenter();
            var penultimatePoint = interimRoutePoints.Any()
                ? interimRoutePoints.Last()
                : sourceRect.GetCenter();

            var firstPoint = sourceRect.GetPerimeterPointTowardOuterPoint(secondPoint);
            var lastPoint = targetRect.GetPerimeterPointTowardOuterPoint(penultimatePoint);

            var route = firstPoint.ToEnumerable()
                .Concat(interimRoutePoints)
                .Concat(lastPoint.ToEnumerable())
                .ToArray();

            return boundingRect.ToRelativePoints(route);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
