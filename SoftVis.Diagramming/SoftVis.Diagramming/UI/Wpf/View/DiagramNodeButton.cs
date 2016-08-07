using System.Windows;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A special button used on diagram nodes. 
    /// Can serve as a BubbleListBox owner.
    /// Inherits its Placement property from its items control container.
    /// </summary>
    public class DiagramNodeButton : DiagramButton, IBubbleListBoxOwner
    {
        static DiagramNodeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramNodeButton),
                new FrameworkPropertyMetadata(typeof(DiagramNodeButton)));
        }

        public static readonly DependencyProperty PlacementProperty =
            DecoratorPanel.PlacementProperty.AddOwner(typeof(DiagramNodeButton));

        public RectRelativePlacement Placement
        {
            get { return (RectRelativePlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public HandleOrientation GetHandleOrientation()
        {
            var rectRelativePlacement = DecoratorPanel.GetPlacement(this);
            if (rectRelativePlacement == RectRelativePlacement.Undefined)
                return HandleOrientation.None;

            return rectRelativePlacement.Vertical == VerticalAlignment.Bottom
                ? HandleOrientation.Top
                : HandleOrientation.Bottom;
        }

        public Point GetAttachPoint()
        {
            var vectorX = Width / 2;
            var vectorY = GetHandleOrientation() == HandleOrientation.Top ? Height : 0;
            return new Point(vectorX, vectorY);
        }
    }
}
