using System.Linq;
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
            foreach (var child in Children.OfType<DiagramItemContainer>())
            {
                child.RenderTransform = CreateRenderTransform(child); 
                child.Arrange(new Rect(child.DesiredSize));
            }
            return finalSize;
        }

        private Transform CreateRenderTransform(DiagramItemContainer child)
        {
            var appearDisappearTransform = CreateAppearDisappearTransform(child);
            var positionChild = new TranslateTransform(GetLeft(child), GetTop(child));

            var renderTransform = new TransformGroup();
            renderTransform.Children.Add(appearDisappearTransform);
            renderTransform.Children.Add(positionChild);
            renderTransform.Children.Add(Transform);
            return renderTransform;
        }

        private static Transform CreateAppearDisappearTransform(DiagramItemContainer child)
        {
            var childScaling = child.Scaling;
            var childWidth = child.ActualWidth;
            var childHeight = child.ActualHeight;

            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(-childWidth/2, -childHeight/2));
            transform.Children.Add(new ScaleTransform(childScaling, childScaling));
            transform.Children.Add(new TranslateTransform(childWidth/2, childHeight/2));
            return transform;
        }
    }
}
