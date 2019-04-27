using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Codartis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// An animated content presenter with Rect and Center properties.
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

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(AnimatedRectContentPresenter));

        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void OnRectChanged(Rect oldValue, Rect newValue)
        {
            Center = newValue.GetCenter();
        }
    }
}
