using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Rendering.Wpf.Common.Geometry;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a Rect2D into a WPF Rect.
    /// </summary>
    public class RectToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Rect2D))
                throw new ArgumentException($"{typeof (RectToWpfConverter).Name} expects a {typeof(Rect2D).Name} value.");

            return ((Rect2D)value).ToWpf();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
