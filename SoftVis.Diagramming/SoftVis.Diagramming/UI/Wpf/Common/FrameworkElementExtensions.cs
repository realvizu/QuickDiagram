using System.Windows;
using System.Windows.Data;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    public static class FrameworkElementExtensions
    {
        public static void AddResourceDictionary(this FrameworkElement frameworkElement, ResourceDictionary resourceDictionary)
        {
            frameworkElement.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        public static void SetBinding(this FrameworkElement targetObject, DependencyProperty targetProperty,
            DependencyObject sourceObject, DependencyProperty sourceProperty)
        {
            var binding = new Binding
            {
                Source = sourceObject,
                Path = new PropertyPath(sourceProperty)
            };
            targetObject.SetBinding(targetProperty, binding);
        }

        public static void ClearBinding(this FrameworkElement dependencyObject, DependencyProperty dependencyProperty)
        {
            BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
        }
    }
}
