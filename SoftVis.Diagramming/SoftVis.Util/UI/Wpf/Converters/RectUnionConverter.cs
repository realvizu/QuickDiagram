using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Returns the union of rects and point collections.
    /// </summary>
    public sealed class RectUnionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Any() ||
                !values.All(i => i is Rect || i is IEnumerable<Point>))
                return null;

            return values.OfType<IEnumerable<Point>>().Select(i => i.BoundingRect())
                .Concat(values.OfType<Rect>())
                .Union();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
