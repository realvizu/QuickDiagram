using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.Rendering.Wpf.Common;

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
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(DiagramShapeControlBase),
                new FrameworkPropertyMetadata(1d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(DiagramShapeControlBase),
                new FrameworkPropertyMetadata(WpfConstants.ExtremePoint,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(DiagramShapeControlBase),
                new FrameworkPropertyMetadata(Size.Empty,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public Size Size
        {
            get { return (Size)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public Rect Rect => new Rect(Position, Size);

        protected override Size MeasureOverride(Size constraint)
        {
            return Size;
        }
    }
}
