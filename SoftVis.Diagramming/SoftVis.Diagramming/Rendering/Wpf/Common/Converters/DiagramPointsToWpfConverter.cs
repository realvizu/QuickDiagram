using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a DiagramPoint[] into a WPF Point[].
    /// </summary>
    public class DiagramPointsToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!(value is DiagramPoint[]))
                throw new ArgumentException($"{typeof(DiagramPointsToWpfConverter).Name} expects a DiagramPoint[] value.");

            return ((DiagramPoint[])value).Select(i => i.ToWpf()).ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
