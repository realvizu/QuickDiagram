using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// This panel arranges its children relative to a chosen UI element (the decorated element).
    /// The decorated UI element must be a canvas child. 
    /// This panel binds to the decorated elements's positions and transform properties 
    /// and also to its parent canvas' transform so it can arrange its children accordingly.
    /// The decorated element can be changed dynamically.
    /// The children's arrangement relative to the decorated element are specified with placement specifications.
    /// (See DecoratorPanel.)
    /// </summary>
    public class CanvasChildDecoratorPanel : DecoratorPanel
    {
        public static readonly DependencyProperty DecoratedElementProperty =
            DependencyProperty.Register("DecoratedElement", typeof(UIElement), typeof(CanvasChildDecoratorPanel),
                new FrameworkPropertyMetadata(OnDecoratedElementChanged));

        private static void OnDecoratedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((CanvasChildDecoratorPanel)d).OnDecoratedElementChanged();

        public static readonly DependencyProperty DecoratedElementTopProperty =
            DependencyProperty.Register("DecoratedElementTop", typeof(double), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(0d, UpdateRenderTransform));

        public static readonly DependencyProperty DecoratedElementLeftProperty =
            DependencyProperty.Register("DecoratedElementLeft", typeof(double), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(0d, UpdateRenderTransform));

        public static readonly DependencyProperty DecoratedElementTransformProperty =
            DependencyProperty.Register("DecoratedElementTransform", typeof(Transform), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(Transform.Identity, UpdateRenderTransform));

        public static readonly DependencyProperty DecoratedCanvasTransformProperty =
            DependencyProperty.Register("DecoratedCanvasTransform", typeof(Transform), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(Transform.Identity, UpdateRenderTransform));

        private static void UpdateRenderTransform(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((CanvasChildDecoratorPanel)d).UpdateRenderTransform();

        public UIElement DecoratedElement
        {
            get { return (UIElement)GetValue(DecoratedElementProperty); }
            set { SetValue(DecoratedElementProperty, value); }
        }

        public double DecoratedElementTop
        {
            get { return (double)GetValue(DecoratedElementTopProperty); }
            set { SetValue(DecoratedElementTopProperty, value); }
        }

        public double DecoratedElementLeft
        {
            get { return (double)GetValue(DecoratedElementLeftProperty); }
            set { SetValue(DecoratedElementLeftProperty, value); }
        }

        public Transform DecoratedElementTransform
        {
            get { return (Transform)GetValue(DecoratedElementTransformProperty); }
            set { SetValue(DecoratedElementTransformProperty, value); }
        }

        public Transform DecoratedCanvasTransform
        {
            get { return (Transform)GetValue(DecoratedCanvasTransformProperty); }
            set { SetValue(DecoratedCanvasTransformProperty, value); }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (DecoratedElement != null)
                ArrangeChildrenRelativeToRect(new Rect(DecoratedElement.RenderSize));

            return arrangeSize;
        }

        protected virtual void OnDecoratedElementChanged()
        {
            if (DecoratedElement == null)
            {
                this.ClearBinding(DecoratedCanvasTransformProperty);
                this.ClearBinding(DecoratedElementTransformProperty);
                this.ClearBinding(DecoratedElementTopProperty);
                this.ClearBinding(DecoratedElementLeftProperty);
            }
            else
            {
                var decoratedCanvas = DecoratedElement.FindParent<Canvas>();
                if (decoratedCanvas != null)
                    this.SetBinding(DecoratedCanvasTransformProperty, decoratedCanvas, RenderTransformProperty);

                this.SetBinding(DecoratedElementTransformProperty, DecoratedElement, RenderTransformProperty);
                this.SetBinding(DecoratedElementTopProperty, DecoratedElement, Canvas.TopProperty);
                this.SetBinding(DecoratedElementLeftProperty, DecoratedElement, Canvas.LeftProperty);
            }
        }

        private void UpdateRenderTransform()
        {
            RenderTransform = CreateRenderTransform();
        }

        private Transform CreateRenderTransform()
        {
            if (DecoratedElement == null)
                return Transform.Identity;

            var decoratedTranslateTransform = new TranslateTransform(DecoratedElementLeft, DecoratedElementTop);

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(DecoratedElementTransform);
            transformGroup.Children.Add(decoratedTranslateTransform);
            transformGroup.Children.Add(DecoratedCanvasTransform);
            return transformGroup;
        }
    }
}
