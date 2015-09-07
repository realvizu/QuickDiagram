using Codartis.SoftVis.Diagramming;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
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
