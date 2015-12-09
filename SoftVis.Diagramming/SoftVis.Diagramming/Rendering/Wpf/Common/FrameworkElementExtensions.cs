using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public static class FrameworkElementExtensions
    {
        public static void AddResourceDictionary(this FrameworkElement frameworkElement, ResourceDictionary resourceDictionary)
        {
            frameworkElement.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
