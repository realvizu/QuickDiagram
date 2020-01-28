using System.Collections;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf.DataTemplateSelectors
{
    /// <summary>
    /// Selects a DataTemplate from a given map, using a given key.
    /// </summary>
    public sealed class DataTemplateMapItemSelector : DataTemplateSelector
    {
        [UsedImplicitly] [CanBeNull] public IDictionary DataTemplateMap { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item != null && DataTemplateMap?.Contains(item) == true
                ? DataTemplateMap[item] as DataTemplate
                : null;
        }
    }
}