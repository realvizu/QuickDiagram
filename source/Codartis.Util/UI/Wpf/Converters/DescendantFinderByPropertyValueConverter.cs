using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Codartis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Finds a descendant of on object whose given property equals the given value.
    /// The parameter is the searched property.
    /// Values[0] is the search root object.
    /// Values[1] is the searched property value.
    /// </summary>
    public sealed class DescendantFinderByPropertyValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is DependencyProperty searchedProperty))
                throw new ArgumentNullException($"Parameter must be a {nameof(DependencyProperty)}.");

            if (!(values[0] is DependencyObject searchRoot))
                throw new ArgumentNullException($"Values[0] must be a {nameof(DependencyObject)}.");

            var searchedValue = values[1];

            var result = searchRoot.FindFirstDescendant<DependencyObject>(i => i.GetType() == targetType && i.GetValue(searchedProperty) == searchedValue);

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}