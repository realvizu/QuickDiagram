using System.Windows;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// An animated content presenter with a Rect property.
    /// </summary>
    public class AnimatedRectContentPresenter : AnimatedContentPresenter
    {
        static AnimatedRectContentPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedRectContentPresenter),
                new FrameworkPropertyMetadata(typeof(AnimatedRectContentPresenter)));
        }

        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register("Rect", typeof(Rect), typeof(AnimatedRectContentPresenter),
                new FrameworkPropertyMetadata(OnRectChanged));

        private static void OnRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedRectContentPresenter) d).OnRectChanged((Rect) e.OldValue, (Rect) e.NewValue);

        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        protected virtual void OnRectChanged(Rect oldValue, Rect newValue)
        {
        }
    }
}
