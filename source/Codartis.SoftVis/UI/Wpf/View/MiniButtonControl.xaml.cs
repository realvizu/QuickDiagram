using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;
using Codartis.Util.UI.Wpf.Commands;
using Codartis.Util.UI.Wpf.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A special button used on diagrams. 
    /// Can serve as a BubbleListBox owner.
    /// Inherits its Placement property from its items control container.
    /// </summary>
    public partial class MiniButtonControl : UserControl, IBubbleListBoxOwner
    {
        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(MiniButtonControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(MiniButtonControl));

        public static readonly DependencyProperty PlacementProperty =
            RelativePlacementPanel.PlacementProperty.AddOwner(typeof(MiniButtonControl));

        public static readonly DependencyProperty MouseClickCommandProperty =
            DependencyProperty.Register("MouseClickCommand", typeof(DelegateCommand), typeof(MiniButtonControl));

        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.Register("MouseDoubleClickCommand", typeof(DelegateCommand), typeof(MiniButtonControl));

        public MiniButtonControl()
        {
            InitializeComponent();
        }

        public Brush DiagramFill
        {
            get { return (Brush)GetValue(DiagramFillProperty); }
            set { SetValue(DiagramFillProperty, value); }
        }

        public Brush DiagramStroke
        {
            get { return (Brush)GetValue(DiagramStrokeProperty); }
            set { SetValue(DiagramStrokeProperty, value); }
        }

        public RectRelativePlacement Placement
        {
            get { return (RectRelativePlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public DelegateCommand MouseClickCommand
        {
            get { return (DelegateCommand)GetValue(MouseClickCommandProperty); }
            set { SetValue(MouseClickCommandProperty, value); }
        }

        public DelegateCommand MouseDoubleClickCommand
        {
            get { return (DelegateCommand)GetValue(MouseDoubleClickCommandProperty); }
            set { SetValue(MouseDoubleClickCommandProperty, value); }
        }

        public HandleOrientation GetHandleOrientation()
        {
            var rectRelativePlacement = RelativePlacementPanel.GetPlacement(this);
            if (rectRelativePlacement == null)
                return HandleOrientation.None;

            return rectRelativePlacement.Value.Vertical == VerticalAlignment.Bottom
                ? HandleOrientation.Top
                : HandleOrientation.Bottom;
        }

        public Point GetAttachPoint()
        {
            var vectorX = Width / 2;
            var vectorY = GetHandleOrientation() == HandleOrientation.Top ? Height : 0;
            return new Point(vectorX, vectorY);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Handling the mouse move event stops it from bubbling up
            // so the diagram shape can retains focus while hovering on its buttons
            e.Handled = true;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            MouseClickCommand?.Execute();
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MouseDoubleClickCommand?.Execute();
            e.Handled = true;
        }
    }
}
