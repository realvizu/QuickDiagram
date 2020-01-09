using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf.Converters
{
    public sealed class DictionaryItemSelectorConverter : ConverterMarkupExtensionBase, IValueConverter
    {
        [UsedImplicitly]
        public IDictionary Dictionary { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Dictionary?[value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}