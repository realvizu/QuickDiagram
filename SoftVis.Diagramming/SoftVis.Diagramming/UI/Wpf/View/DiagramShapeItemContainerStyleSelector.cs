using System;
using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Selectes different style for DiagramNode and DiagramConnector view model items.
    /// </summary>
    public class DiagramShapeItemContainerStyleSelector : StyleSelector
    {
        public Style DiagramNodeStyle { get; set; }
        public Style DiagramConnectorStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is DiagramNodeViewModel)
                return DiagramNodeStyle;
            if (item is DiagramConnectorViewModel)
                return DiagramConnectorStyle;

            throw new Exception($"Unexpected item type: {item?.GetType()}");
        }
    }
}
