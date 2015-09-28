using System;
using System.Linq;
using System.Windows.Data;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts the given DiagramRect[] to WPF Rects and calculates their union.
    /// </summary>
    public class DiagramRectsToWpfUnionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rectUnion = values.OfType<DiagramRect>().Select(i => i.ToWpf()).Union();

            foreach (var wpfPoint in values.OfType<DiagramPoint[]>().SelectMany(i => i.Select(j => j.ToWpf())))
                rectUnion.Union(wpfPoint);

            return rectUnion;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
