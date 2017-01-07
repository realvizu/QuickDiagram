using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Codartis.SoftVis.Util.UI.Wpf.Resources
{
    public static class ResourceHelpers
    {
        /// <summary>
        /// Converts a filename that is relative to the project root into an Uri that contains the assembly name too.
        /// </summary>
        /// <param name="filenameRelativeToProjectRoot">A filename relative to the project root.</param>
        /// <param name="assembly">The assembly of the Uri.</param>
        /// <returns>The Uri of the given filename that contains the assembly name too.</returns>
        /// <remarks>
        /// Unfortunately the simple relative Uri format don't work. May be related to hosting in Visual Studio, who knows.
        /// Anyway, it works by specifying the assembly.
        /// </remarks>
        public static Uri CreateUri(string filenameRelativeToProjectRoot, Assembly assembly)
        {
            var assemblyFileName = assembly.ManifestModule.Name;
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyFileName);
            var uri = new Uri($"/{assemblyName};component/{filenameRelativeToProjectRoot}", UriKind.Relative);
            return uri;
        }

        public static ResourceDictionary GetResourceDictionary(string resourceDictionaryFilename, Assembly assembly)
        {
            var uri = CreateUri(resourceDictionaryFilename, assembly);
            return GetResourceDictionary(uri);
        }

        public static ResourceDictionary GetResourceDictionary(Uri uri)
        {
            return (ResourceDictionary)Application.LoadComponent(uri);
        }
    }
}