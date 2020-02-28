using System;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf.Converters
{
    /// <summary>
    /// Abstract base class for converters that are also markup extensions.
    /// </summary>
    /// <remarks>
    /// Useful if you want to specify more converter parameters and/or you want type checking on the parameter.
    /// </remarks>
    public abstract class ConverterMarkupExtensionBase : MarkupExtension
    {
        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}