using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    internal class DiagramItemsControl : ItemsControl
    {
        public DiagramItemsControl()
        {
            // To enable mouse hit.
            Background = Brushes.Transparent;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramItemContainer();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiagramItemContainer;
        }
    }
}
