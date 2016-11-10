using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Returns the union of Rects.
    /// </summary>
    public sealed class RectUnionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Any() ||
                !values.All(i => i is Rect))
                return null;

            return values.OfType<Rect>().Union();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
