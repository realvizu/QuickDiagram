using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public class RectUnionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var resultRect = Rect.Empty;

            foreach (var inputRect in values.OfType<Rect>())
                resultRect.Union(inputRect);

            return resultRect;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
