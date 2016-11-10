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
                new FrameworkPropertyMetadata(Rect.Empty));

        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }
    }
}
