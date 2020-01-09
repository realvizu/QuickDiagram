using System;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Abstract base class for converters that are also markup extensions.
    /// </summary>
    public abstract class ConverterMarkupExtensionBase : MarkupExtension
    {
        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}