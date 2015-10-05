using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a Point2D[] into a WPF Point[].
    /// </summary>
    public class PointsToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!(value is Point2D[]))
                throw new ArgumentException($"{typeof(PointsToWpfConverter).Name} expects a {typeof(Point2D).Name}[] value.");

            return ((Point2D[])value).Select(i => i.ToWpf()).ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
