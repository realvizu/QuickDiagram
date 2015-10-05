using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a Route into a WPF Point[].
    /// </summary>
    public class RouteToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!(value is Route))
                throw new ArgumentException($"{typeof(RouteToWpfConverter).Name} expects a {typeof(Route).Name}[] value.");

            return ((Route)value).Select(i => i.ToWpf()).ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
