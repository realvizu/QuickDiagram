using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf
{
    public static class BindingHelper
    {
        public static void Bind(
            [NotNull] this DependencyProperty property,
            [NotNull] DependencyObject source,
            [NotNull] DependencyObject target)
        {
            var binding = new Binding
            {
                Source = source,
                Path = new PropertyPath(property)
            };
            BindingOperations.SetBinding(target, property, binding);
        }
    }
}