using System;
using System.Globalization;
using System.Windows.Data;

namespace Codartis.SoftVis.UI.Wpf.Common.Converters
{
    /// <summary>
    /// Multiplies the given double amount with the given parameter amount.
    /// </summary>
    public class DoubleMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double multiplierValue;
            if (!(value is double) || 
                !(parameter is string) || 
                !double.TryParse((string)parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out multiplierValue))
                return null;

            return (double)value * multiplierValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
