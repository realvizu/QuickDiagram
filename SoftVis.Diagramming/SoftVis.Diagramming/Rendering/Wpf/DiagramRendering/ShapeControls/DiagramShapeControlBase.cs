using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    public abstract class DiagramShapeControlBase : Control
    {
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register("Rect", typeof(Rect), typeof(DiagramShapeControlBase),
                new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.AffectsParentArrange));

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
