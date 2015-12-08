using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls.MiniButtons
{
    /// <summary>
    /// Abstract base class for adorners that work as a button.
    /// </summary>
    internal abstract class MiniButtonBase : Adorner
    {
        private const double MiniButtonRadius = DefaultDiagramExtensionProvider.MiniButtonRadius;
        private const double ButtonFrameThickness = 1d;

        /// <summary>
        /// The adorned control is stored here to later acquire its colors. 
        /// The base class also stores the adorned element but as a UIElement.
        /// </summary>
        protected readonly Control AdornedControl;

        protected MiniButtonBase(Control adornedControl, Visibility initialVisibility = Visibility.Collapsed)
            : base(adornedControl)
        {
            AdornedControl = adornedControl;
            Visibility = initialVisibility;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var center = GetButtonCenter();
            DrawFrame(drawingContext, center);
            DrawPicture(drawingContext, center);
        }

        protected abstract Point GetButtonCenter();

        protected abstract void DrawPicture(DrawingContext drawingContext, Point center);

        private void DrawFrame(DrawingContext drawingContext, Point center)
        {
            var pen = new Pen(AdornedControl.Foreground, ButtonFrameThickness);
            var brush = AdornedControl.Background;

            drawingContext.DrawEllipse(brush, pen, center, MiniButtonRadius, MiniButtonRadius);
        }
    }
}