using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Creates a Rect from X, Y, Width, Height properties.
    /// </summary>
    public sealed class RectCreatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 ||
                !(values[0] is double) ||
                !(values[1] is double) ||
                !(values[2] is double) ||
                !(values[3] is double))
                return null;

            return new Rect(new Point((double)values[0], (double)values[1]), new Size((double)values[2], (double)values[3]));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
