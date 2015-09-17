using System;
using System.Globalization;
using System.Windows.Data;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a DiagramSize into a WPF Size.
    /// </summary>
    public class DiagramSizeToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DiagramSize))
                throw new ArgumentException($"{typeof (DiagramSizeToWpfConverter).Name} expects a DiagramSize value.");

            return ((DiagramSize)value).ToWpf();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
