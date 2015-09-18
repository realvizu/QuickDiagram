using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    /// <summary>
    /// Base class for WPF controls that visualize diagram shapes.
    /// All shape controls has a Size and a Position that drives the placement of the shape on the canvas.
    /// The placement (arrangement) of the shape control is done by their parent canvas/panel.
    /// The Position and Size together form a Rect value.
    /// If the Rect changes then the parent canvas must rearrange its shape children.
    /// </summary>
    public abstract class DiagramShapeControlBase : Control
    {
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register("Rect", typeof(Rect), typeof(DiagramShapeControlBase),
                new FrameworkPropertyMetadata(Rect.Empty, 
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        public Point Position => Rect.IsEmpty ? new Point(0,0) : new Point(Rect.X, Rect.Y);
        public Size Size => Rect.IsEmpty ? Size.Empty : new Size(Rect.Width, Rect.Height);

        protected override Size MeasureOverride(Size constraint)
        {
            return Size;
        }
    }
}
