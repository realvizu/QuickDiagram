using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Presents a collection of view model items with adorner-like containers that align to a certain UI element.
    /// </summary>
    internal class AdornerlikeItemsControl : ItemsControl
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AdornerlikeContentPresenter();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AdornerlikeContentPresenter;
        }
    }
}
