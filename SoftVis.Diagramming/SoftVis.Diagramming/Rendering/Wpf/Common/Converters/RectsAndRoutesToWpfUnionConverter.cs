using System;
using System.Linq;
using System.Windows.Data;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Rendering.Wpf.Common.Converters
{
    /// <summary>
    /// Converts the given Rect2D and/or Route object to the union of their WPF Rects.
    /// </summary>
    public class RectsAndRoutesToWpfUnionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rectUnion = values.OfType<Rect2D>().Select(i => i.ToWpf()).Union();

            foreach (var wpfPoint in values.OfType<Route>().SelectMany(i => i.Select(j => j.ToWpf())))
                rectUnion.Union(wpfPoint);

            return rectUnion;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
