using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// This content presenter aligns to a chosen UI element, just like an adorner.
    /// So it must be positioned relative to the adorned element.
    /// The adorned element can be changed dynamically.
    /// </summary>    
    /// <remarks>
    /// Bind only to AdornedElement in xaml. All other DPs are bound dynamically as AdornedElement changes.
    /// </remarks>
    internal class AdornerlikeContentPresenter : ContentPresenter
    {
        static AdornerlikeContentPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdornerlikeContentPresenter),
                new FrameworkPropertyMetadata(typeof(AdornerlikeContentPresenter)));
        }

        public static readonly DependencyProperty AdornedElementProperty =
            DependencyProperty.Register("AdornedElement", typeof(UIElement), typeof(AdornerlikeContentPresenter),
                new FrameworkPropertyMetadata(OnAdornedElementChanged));

        private static void OnAdornedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AdornerlikeContentPresenter) d).OnAdornedElementChanged();

        public static readonly DependencyProperty AdornedElementTopProperty =
            DependencyProperty.Register("AdornedElementTop", typeof(double), typeof(AdornerlikeContentPresenter),
                new PropertyMetadata(0d, UpdateRenderTransform));

        public static readonly DependencyProperty AdornedElementLeftProperty =
            DependencyProperty.Register("AdornedElementLeft", typeof(double), typeof(AdornerlikeContentPresenter),
                new PropertyMetadata(0d, UpdateRenderTransform));

        public static readonly DependencyProperty AdornedElementTransformProperty =
            DependencyProperty.Register("AdornedElementTransform", typeof(Transform), typeof(AdornerlikeContentPresenter),
                new PropertyMetadata(Transform.Identity, UpdateRenderTransform));

        public static readonly DependencyProperty AdornedCanvasTransformProperty =
            DependencyProperty.Register("AdornedCanvasTransform", typeof(Transform), typeof(AdornerlikeContentPresenter),
                new PropertyMetadata(Transform.Identity, UpdateRenderTransform));

        private static void UpdateRenderTransform(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AdornerlikeContentPresenter)d).UpdateRenderTransform();

        public UIElement AdornedElement
        {
            get { return (UIElement)GetValue(AdornedElementProperty); }
            set { SetValue(AdornedElementProperty, value); }
        }

        public double AdornedElementTop
        {
            get { return (double)GetValue(AdornedElementTopProperty); }
            set { SetValue(AdornedElementTopProperty, value); }
        }

        public double AdornedElementLeft
        {
            get { return (double)GetValue(AdornedElementLeftProperty); }
            set { SetValue(AdornedElementLeftProperty, value); }
        }

        public Transform AdornedElementTransform
        {
            get { return (Transform)GetValue(AdornedElementTransformProperty); }
            set { SetValue(AdornedElementTransformProperty, value); }
        }

        public Transform AdornedCanvasTransform
        {
            get { return (Transform)GetValue(AdornedCanvasTransformProperty); }
            set { SetValue(AdornedCanvasTransformProperty, value); }
        }

        private void OnAdornedElementChanged()
        {
            if (AdornedElement == null)
            {
                this.ClearBinding(AdornedCanvasTransformProperty);
                this.ClearBinding(AdornedElementTransformProperty);
                this.ClearBinding(AdornedElementTopProperty);
                this.ClearBinding(AdornedElementLeftProperty);
            }
            else
            {
                var adornedCanvas = AdornedElement.FindParent<Canvas>();
                if (adornedCanvas != null)
                    this.SetBinding(AdornedCanvasTransformProperty, adornedCanvas, RenderTransformProperty);

                this.SetBinding(AdornedElementTransformProperty, AdornedElement, RenderTransformProperty);
                this.SetBinding(AdornedElementTopProperty, AdornedElement, Canvas.TopProperty);
                this.SetBinding(AdornedElementLeftProperty, AdornedElement, Canvas.LeftProperty);
            }
        }

        private void UpdateRenderTransform()
        {
            RenderTransform = CreateRenderTransform();
        }

        private Transform CreateRenderTransform()
        {
            if (AdornedElement == null)
                return Transform.Identity;

            var adornerTranslateTransform = new TranslateTransform(Canvas.GetLeft(this), Canvas.GetTop(this));
            var adornedTranslateTransform = new TranslateTransform(AdornedElementLeft, AdornedElementTop);

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(adornerTranslateTransform);
            transformGroup.Children.Add(AdornedElementTransform);
            transformGroup.Children.Add(adornedTranslateTransform);
            transformGroup.Children.Add(AdornedCanvasTransform);
            transformGroup.Children.Add(adornerTranslateTransform.GetInverse());
            return transformGroup;
        }
    }
}