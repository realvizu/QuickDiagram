using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.UI.Wpf.Common.Converters
{
    /// <summary>
    /// Converts a bool value to Visibility.Visible / Visibility.Collapsed
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new ArgumentException($"{GetType().Name} value must be bool type.");

            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
