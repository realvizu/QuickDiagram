using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A canvas with an additional Transform applied to the children after positioning them with (Canvas.Left, Canvas.Top).
    /// </summary>
    internal class TransformCanvas : Canvas
    {
        static TransformCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransformCanvas),
                new FrameworkPropertyMetadata(typeof(TransformCanvas)));
        }

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform), typeof(TransformCanvas),
                new FrameworkPropertyMetadata(Transform.Identity,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        public TransformCanvas()
        {
            ClipToBounds = true;
        }

        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
                child.Measure(availableSize);

            return Size.Empty;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                var shapePosition = new TranslateTransform(GetLeft(child), GetTop(child));
                var renderTransform = new TransformGroup();
                renderTransform.Children.Add(shapePosition);
                renderTransform.Children.Add(Transform);
                child.RenderTransform = renderTransform;
                child.Arrange(new Rect(child.DesiredSize));
            }

            return finalSize;
        }
    }
}
