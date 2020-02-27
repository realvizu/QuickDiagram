using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Codartis.Util.UI.Wpf.Converters
{
    public sealed class AncestorFinderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var dependencyObject = value as DependencyObject;
            if (dependencyObject == null)
                throw new Exception("AncestorFinderConverter's value must be a DependencyObject.");

            var ancestorType = parameter as Type;
            if (ancestorType == null)
                throw new Exception("AncestorFinderConverter's parameter must be the ancestor's type.");

            return FindAncestor(dependencyObject, ancestorType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static DependencyObject FindAncestor(DependencyObject dependencyObject, Type ancestorType)
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent == null || parent.GetType() == ancestorType)
                return parent;

            return FindAncestor(parent, ancestorType);
        }
    }
}