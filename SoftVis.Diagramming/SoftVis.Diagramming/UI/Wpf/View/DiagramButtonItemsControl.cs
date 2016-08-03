using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A control that presents diagram buttons that are visually attached to other diagram visuals.
    /// </summary>
    internal class DiagramButtonItemsControl : AdornerlikeItemsControl
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramAdornerlikeContentPresenter();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiagramAdornerlikeContentPresenter;
        }
    }
}
