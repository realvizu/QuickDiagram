using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.Common;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes
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
        private static readonly Duration ShapeAppearAnimationDuration = new Duration(TimeSpan.FromMilliseconds(250));
        private static readonly Duration ShapeDisappearAnimationDuration = ShapeAppearAnimationDuration;
        protected static readonly Duration ShapeMoveAnimationDuration = ShapeAppearAnimationDuration;

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

        public abstract void RefreshBinding();

        public void Remove(Action<DiagramShape, DiagramShapeControlBase> completedAction)
        {
            var animation = new DoubleAnimation(1, 0, ShapeDisappearAnimationDuration);
            animation.Completed += (s, a) => completedAction(DiagramShape, this);
            BeginAnimation(ScaleProperty, animation);
        }

        protected abstract DiagramShape DiagramShape { get; }

        protected override Size MeasureOverride(Size constraint)
        {
            return Size;
        }

        protected void Appear()
        {
            if (Position.IsExtreme())
                AnimateAppear();
        }

        protected void MoveTo(Point newPosition)
        {
            if (Position.IsExtreme())
                Position = newPosition;
            else
                AnimateMove(newPosition);
        }

        protected void SizeTo(Size newSize)
        {
            if (Size == Size.Empty)
                Size = newSize;
            else
                AnimateSize(newSize);
        }

        private void AnimateAppear()
        {
            var animation = new DoubleAnimation(0, 1, ShapeAppearAnimationDuration);
            BeginAnimation(ScaleProperty, animation);
        }

        private void AnimateMove(Point toPosition)
        {
            var animation = new PointAnimation(toPosition, ShapeMoveAnimationDuration);
            BeginAnimation(PositionProperty, animation);
        }

        private void AnimateSize(Size toSize)
        {
            var animation = new SizeAnimation(toSize, ShapeMoveAnimationDuration);
            BeginAnimation(SizeProperty, animation);
        }
    }
}
