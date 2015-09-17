using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a Point[] to their bounding Rect.
    /// </summary>
    public class PointsToBoundingRectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Rect.Empty;

            if (!(value is Point[]))
                throw new ArgumentException($"{typeof(PointsToBoundingRectConverter).Name} expects a Point[] value.");

            var points = (Point[])value;
            return points.BoundingRect();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
