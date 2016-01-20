using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Presents a collection of view model items. Creates a container for each view model 
    /// and that container creates the proper control corresponding to the view model.
    /// </summary>
    internal abstract class PositionedItemsControl : ItemsControl
    {
        protected PositionedItemsControl()
        {
            // To enable mouse hit.
            Background = Brushes.Transparent;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PositionedItemContainer();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PositionedItemContainer;
        }
    }
}
