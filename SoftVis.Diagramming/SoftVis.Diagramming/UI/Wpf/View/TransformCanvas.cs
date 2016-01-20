using System;
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
        private static readonly Size UnlimitedSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

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
        }

        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
                child.Measure(UnlimitedSize);

            return Size.Empty;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children.OfType<PositionedItemContainer>())
            {
                child.RenderTransform = CreateRenderTransform(child); 
                child.Arrange(new Rect(child.DesiredSize));
            }
            return finalSize;
        }

        private Transform CreateRenderTransform(PositionedItemContainer child)
        {
            var itemTransform = child.GetItemTransform();
            var positionItem = new TranslateTransform(GetLeft(child), GetTop(child));

            var renderTransform = new TransformGroup();
            if (itemTransform != null) renderTransform.Children.Add(itemTransform);
            renderTransform.Children.Add(positionItem);
            renderTransform.Children.Add(Transform);
            return renderTransform;
        }
    }
}
