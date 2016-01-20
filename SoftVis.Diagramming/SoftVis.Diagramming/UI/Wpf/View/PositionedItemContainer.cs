using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Wraps an item the is positioned on a canvas. 
    /// </summary>    
    internal class PositionedItemContainer : ContentPresenter
    {
        static PositionedItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PositionedItemContainer),
                new FrameworkPropertyMetadata(typeof(PositionedItemContainer)));
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(PositionedItemContainer),
                new FrameworkPropertyMetadata(double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnXPropertyChanged));

        private static void OnXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((PositionedItemContainer)d).OnXPropertyChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(PositionedItemContainer),
                new FrameworkPropertyMetadata(double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnYPropertyChanged));

        private static void OnYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((PositionedItemContainer)d).OnYPropertyChanged((double)e.OldValue, (double)e.NewValue);

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public virtual Transform GetItemTransform()
        {
            return null;
        }

        protected virtual void OnXPropertyChanged(double oldValue, double newValue)
        { }

        protected virtual void OnYPropertyChanged(double oldValue, double newValue)
        { }
    }
}