using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.UI.Wpf.Converters
{
    /// <summary>
    /// Multiplies the given thickness with the given parameter amount.
    /// </summary>
    public class ThicknessMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double multiplier;
            if (!(value is Thickness) ||
                !(parameter is string) ||
                !double.TryParse((string)parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out multiplier))
                return null;

            var thickness = (Thickness)value;

            return new Thickness(
                thickness.Left * multiplier, 
                thickness.Top * multiplier,
                thickness.Right * multiplier, 
                thickness.Bottom * multiplier);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
