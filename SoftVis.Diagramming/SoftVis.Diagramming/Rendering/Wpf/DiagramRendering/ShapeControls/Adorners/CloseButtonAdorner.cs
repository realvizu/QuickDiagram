using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls.Adorners
{
    /// <summary>
    /// A close button for diagram nodes, implemented as an adorner.
    /// </summary>
    internal class CloseButtonAdorner : Adorner
    {
        private const double ButtonRadius = 8d;
        private const double ButtonFrameThickness = 1d;
        private const double XMarkWidthHalf = 3.5d;
        private const double XMarkThickness = 1.5;
        private static readonly Color XMarkColor = Colors.Red;
        private const double OverlapParentBy = 3d;

        private readonly Control _adornedControl;

        public CloseButtonAdorner(Control adornedControl, Visibility initialVisibility = Visibility.Collapsed)
            : base(adornedControl)
        {
            _adornedControl = adornedControl;
            Visibility = initialVisibility;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);
            var center = adornedElementRect.TopRight + new Vector(-OverlapParentBy, OverlapParentBy);
            DrawFrame(drawingContext, center);
            DrawX(drawingContext, center);
        }

        private void DrawFrame(DrawingContext drawingContext, Point center)
        {
            var pen = new Pen(_adornedControl.Foreground, ButtonFrameThickness);
            var brush = _adornedControl.Background;

            drawingContext.DrawEllipse(brush, pen, center, ButtonRadius, ButtonRadius);
        }

        private static void DrawX(DrawingContext drawingContext, Point center)
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
