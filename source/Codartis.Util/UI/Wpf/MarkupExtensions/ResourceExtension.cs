using System;
using System.Windows;
using System.Windows.Markup;
using Codartis.Util.UI.Wpf.Resources;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf.MarkupExtensions
{
    public sealed class ResourceExtension : MarkupExtension
    {
        public string Filename { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Filename == null)
                throw new Exception($"{nameof(Filename)} must be specified");

            var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
            var baseUri = uriContext?.BaseUri;

            var uri = baseUri == null 
                ? new Uri(Filename) 
                : new PackUri(baseUri.ToString(), Filename);

            return Load(uri);
        }

        private static object Load([NotNull] Uri uri)
        {
            var streamResourceInfo = Application.GetResourceStream(uri);
            if (streamResourceInfo?.Stream == null)
                throw new Exception($"Resource not found: {uri}");

            return XamlReader.Load(streamResourceInfo.Stream);
        }
    }
}