using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Codartis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Creates a Thickness value from 4 double values (left, top, right, bottom).
    /// </summary>
    public class ThicknessCreatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Thickness))
                throw new Exception($"{nameof(ThicknessCreatorConverter)} must be bound to a Thickness typed property.");

            if (values.Length != 4 || values.Any(i => !(i is double)))
                return new Thickness();

            return new Thickness(
                (double)values[0],
                (double)values[1],
                (double)values[2],
                (double)values[3]
            );
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}