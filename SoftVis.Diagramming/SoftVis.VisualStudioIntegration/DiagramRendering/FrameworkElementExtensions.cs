using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.DiagramRendering
{
    public static class FrameworkElementExtensions
    {
        public static void AddResourceDictionary(this FrameworkElement frameworkElement, string resourceDictionaryFilename)
        {
            var uri = WpfHelpers.CreateUri(resourceDictionaryFilename);
            var dictionary = (ResourceDictionary)Application.LoadComponent(uri);
            frameworkElement.Resources.MergedDictionaries.Add(dictionary);
        }
    }
}
