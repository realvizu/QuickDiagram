using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Codartis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Makes diagram connector route points relative to a reference point.
    /// </summary>
    public sealed class DiagramConnectorRouteCalculatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 ||
                !(values[0] is Point[]) ||
                !(values[1] is Point))
                return null;

            var routePoints = (Point[])values[0];
            var referencePoint = (Point)values[1];

            if (referencePoint.IsUndefined() || routePoints == null || routePoints.Any(i => i.IsUndefined()))
                return null;

            return routePoints.MakeRelativeTo(referencePoint);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
