using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Rendering.Wpf.Common.Geometry;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a Point2D into a WPF Point.
    /// </summary>
    public class PointToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Point2D))
                throw new ArgumentException($"{typeof (PointToWpfConverter).Name} expects a {typeof(Point2D).Name} value.");

            return ((Point2D)value).ToWpf();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
