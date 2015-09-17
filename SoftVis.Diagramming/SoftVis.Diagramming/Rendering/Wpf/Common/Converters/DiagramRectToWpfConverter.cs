using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a DiagramRect into a WPF Rect.
    /// </summary>
    public class DiagramRectToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DiagramRect))
                throw new ArgumentException($"{typeof (DiagramRectToWpfConverter).Name} expects a DiagramRect value.");

            return ((DiagramRect)value).ToWpf();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
