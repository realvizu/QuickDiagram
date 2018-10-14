using System;
using System.Globalization;
using System.Windows.Data;

namespace Codartis.SoftVis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// If a double value is not a number (NaN or infinity) then substitutes it with the given parameter value.
    /// </summary>
    public class DoubleEnsureNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double substitutionValue;
            if (!(value is double) || 
                !(parameter is string) || 
                !double.TryParse((string)parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out substitutionValue))
                return null;

            var doubleValue = (double) value;

            return double.IsInfinity(doubleValue) || double.IsNaN(doubleValue) ? substitutionValue : doubleValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
