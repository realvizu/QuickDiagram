using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf
{
    public static class BindingHelper
    {
        public static void SetBinding(
            [NotNull] this DependencyObject targetObject,
            [NotNull] DependencyProperty targetProperty,
            [NotNull] DependencyObject sourceObject,
            [NotNull] DependencyProperty sourceProperty)
        {
            var binding = new Binding
            {
                Source = sourceObject,
                Path = new PropertyPath(sourceProperty)
            };
            BindingOperations.SetBinding(targetObject, targetProperty, binding);
        }

        public static void ClearBinding(
            [NotNull] this DependencyObject dependencyObject,
            [NotNull] DependencyProperty dependencyProperty)
        {
            BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
        }
    }
}