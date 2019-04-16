using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// This panel arranges its children relative to a chosen UI element (the decorated element).
    /// The decorated UI element must be a canvas child inside an AnimatedContentPresenter. 
    /// This panel binds to the presenter's positions and transform properties 
    /// and also to the canvas' transform so it can arrange its children accordingly.
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
            => ((CanvasChildDecoratorPanel) d).OnDecoratedElementChanged();

        public static readonly DependencyProperty DecoratedPresenterTopProperty =
            DependencyProperty.Register("DecoratedPresenterTop", typeof(double), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(0d, UpdateRenderTransform));

        public static readonly DependencyProperty DecoratedPresenterLeftProperty =
            DependencyProperty.Register("DecoratedPresenterLeft", typeof(double), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(0d, UpdateRenderTransform));

        public static readonly DependencyProperty DecoratedPresenterTransformProperty =
            DependencyProperty.Register("DecoratedPresenterTransform", typeof(Transform), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(Transform.Identity, UpdateRenderTransform));

        public static readonly DependencyProperty DecoratedCanvasTransformProperty =
            DependencyProperty.Register("DecoratedCanvasTransform", typeof(Transform), typeof(CanvasChildDecoratorPanel),
                new PropertyMetadata(Transform.Identity, UpdateRenderTransform));

        private static void UpdateRenderTransform(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((CanvasChildDecoratorPanel) d).UpdateRenderTransform();

        public UIElement DecoratedElement
        {
            get { return (UIElement) GetValue(DecoratedElementProperty); }
            set { SetValue(DecoratedElementProperty, value); }
        }

        public double DecoratedPresenterTop
        {
            get { return (double) GetValue(DecoratedPresenterTopProperty); }
            set { SetValue(DecoratedPresenterTopProperty, value); }
        }

        public double DecoratedPresenterLeft
        {
            get { return (double) GetValue(DecoratedPresenterLeftProperty); }
            set { SetValue(DecoratedPresenterLeftProperty, value); }
        }

        public Transform DecoratedPresenterTransform
        {
            get { return (Transform) GetValue(DecoratedPresenterTransformProperty); }
            set { SetValue(DecoratedPresenterTransformProperty, value); }
        }

        public Transform DecoratedCanvasTransform
        {
            get { return (Transform) GetValue(DecoratedCanvasTransformProperty); }
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
                this.ClearBinding(DecoratedPresenterTransformProperty);
                this.ClearBinding(DecoratedPresenterTopProperty);
                this.ClearBinding(DecoratedPresenterLeftProperty);
            }
            else
            {
                var decoratedCanvas = DecoratedElement.FindAncestor<Canvas>();
                if (decoratedCanvas != null)
                {
                    this.SetBinding(DecoratedCanvasTransformProperty, decoratedCanvas, RenderTransformProperty);
                }

                var decoratedPresenter = DecoratedElement.FindAncestor<AnimatedContentPresenter>();
                if (decoratedPresenter != null)
                {
                    this.SetBinding(DecoratedPresenterTransformProperty, decoratedPresenter, RenderTransformProperty);
                    this.SetBinding(DecoratedPresenterTopProperty, decoratedPresenter, Canvas.TopProperty);
                    this.SetBinding(DecoratedPresenterLeftProperty, decoratedPresenter, Canvas.LeftProperty);
                }
            }
        }

        private void UpdateRenderTransform()
        {
            RenderTransform = CreateRenderTransform();
        }

        private Transform CreateRenderTransform()
        {
            var decoratedCanvas = DecoratedElement?.FindAncestor<Canvas>();
            if (decoratedCanvas == null)
                return Transform.Identity;

            var transformFromDecoratedElementToCanvas = DecoratedElement.TransformToAncestor(decoratedCanvas) as Transform;
            if (transformFromDecoratedElementToCanvas == null)
                return Transform.Identity;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(transformFromDecoratedElementToCanvas);
            transformGroup.Children.Add(DecoratedCanvasTransform);
            return transformGroup;
        }
    }
}