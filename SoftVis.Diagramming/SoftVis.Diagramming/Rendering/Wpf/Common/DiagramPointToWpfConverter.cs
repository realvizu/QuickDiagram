using Codartis.SoftVis.Diagramming;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public class DiagramPointToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DiagramPoint))
                throw new ArgumentException($"{typeof (DiagramPointToWpfConverter).Name} expects a DiagramPoint value.");

            return ((DiagramPoint)value).ToWpf();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
