using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    public static class FrameworkElementExtensions
    {
        public static void AddResourceDictionary(this FrameworkElement frameworkElement, string resourceDictionaryFilename)
        {
            var assemblyFileName = Assembly.GetExecutingAssembly().ManifestModule.Name;
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyFileName);
            var uri = new Uri($"/{assemblyName};component/{resourceDictionaryFilename}", UriKind.Relative);
            var dictionary = (ResourceDictionary)Application.LoadComponent(uri);
            frameworkElement.Resources.MergedDictionaries.Add(dictionary);
        }
    }
}
