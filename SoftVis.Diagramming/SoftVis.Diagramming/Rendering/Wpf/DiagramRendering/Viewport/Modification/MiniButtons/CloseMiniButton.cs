using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Rendering.Extensibility;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Modification.MiniButtons
{
    /// <summary>
    /// Close button for diagram nodes.
    /// </summary>
    internal class CloseMiniButton : MiniButtonBase
    {
        private const double XMarkWidthHalf = 3.5d;
        private const double XMarkThickness = 1.5;
        private static readonly Color XMarkColor = Colors.Red;

        public CloseMiniButton(Control adornedControl, Visibility initialVisibility = Visibility.Visible)
            : base(adornedControl, initialVisibility)
        {
        }

        protected override Point GetButtonCenter()
        {
            var overlap = DefaultDiagramBehaviourProvider.MiniButtonOverlapParentBy;
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);
            return adornedElementRect.TopRight + new Vector(-overlap, overlap);
        }

        protected override void DrawPicture(DrawingContext drawingContext, Point center)
        {
            var xPen = new Pen(new SolidColorBrush(XMarkColor), XMarkThickness);
            var nw = center + new Vector(-XMarkWidthHalf, -XMarkWidthHalf);
            var ne = center + new Vector(XMarkWidthHalf, -XMarkWidthHalf);
            var se = center + new Vector(XMarkWidthHalf, XMarkWidthHalf);
            var sw = center + new Vector(-XMarkWidthHalf, XMarkWidthHalf);
            drawingContext.DrawLine(xPen, nw, se);
            drawingContext.DrawLine(xPen, ne, sw);
        }
    }
}
