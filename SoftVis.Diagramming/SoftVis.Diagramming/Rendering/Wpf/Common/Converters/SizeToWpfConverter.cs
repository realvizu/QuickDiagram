using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Rendering.Wpf.Common.Geometry;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a Size2D into a WPF Size.
    /// </summary>
    public class SizeToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Size2D))
                throw new ArgumentException($"{typeof (SizeToWpfConverter).Name} expects a {typeof(Size2D).Name} value.");

            return ((Size2D)value).ToWpf();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
