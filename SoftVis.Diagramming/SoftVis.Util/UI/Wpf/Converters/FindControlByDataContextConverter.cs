using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Returns the first framework element child of a given search root that has the given data context.
    /// </summary>
    public sealed class FindControlByDataContextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 ||
                !(values[0] is FrameworkElement) ||
                values[1] == null)
                return null;

            var searchRootElement = (FrameworkElement)values[0];
            var dataContext = values[1];

            return searchRootElement.FindDescendantByDataContext<FrameworkElement>(dataContext);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
