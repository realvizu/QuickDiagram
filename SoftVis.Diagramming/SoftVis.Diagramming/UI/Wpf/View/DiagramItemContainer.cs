using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.UI.Wpf.View
{
    internal class DiagramItemContainer : Control
    {
        private static readonly Duration Duration = new Duration(new TimeSpan(0, 0, 2));

        static DiagramItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramItemContainer),
                new FrameworkPropertyMetadata(typeof(DiagramItemContainer)));
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(DiagramItemContainer),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.None, XPropertyChangedCallback));

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(DiagramItemContainer),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.None, YPropertyChangedCallback));

        public static readonly DependencyProperty AnimatedXProperty =
            DependencyProperty.Register("AnimatedX", typeof(double), typeof(DiagramItemContainer),
                new FrameworkPropertyMetadata(double.NaN));

        public static readonly DependencyProperty AnimatedYProperty =
            DependencyProperty.Register("AnimatedY", typeof(double), typeof(DiagramItemContainer),
                new FrameworkPropertyMetadata(double.NaN));


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

        public double AnimatedX
        {
            get { return (double)GetValue(AnimatedXProperty); }
            set { SetValue(AnimatedXProperty, value); }
        }

        public double AnimatedY
        {
            get { return (double)GetValue(AnimatedYProperty); }
            set { SetValue(AnimatedYProperty, value); }
        }

        private static void XPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var oldValue = (double)dependencyPropertyChangedEventArgs.OldValue;
            var newValue = (double)dependencyPropertyChangedEventArgs.NewValue;

            var animate = !double.IsNaN(oldValue);
            if (animate)
            {
                var animation = new DoubleAnimation(newValue, Duration);
                ((DiagramItemContainer)dependencyObject).BeginAnimation(Canvas.LeftProperty, animation);
            }
            else
            {
                ((DiagramItemContainer)dependencyObject).AnimatedX = newValue;
            }
        }

        private static void YPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var oldValue = (double)dependencyPropertyChangedEventArgs.OldValue;
            var newValue = (double)dependencyPropertyChangedEventArgs.NewValue;

            var animate = !double.IsNaN(oldValue);
            if (animate)
            {
                var animation = new DoubleAnimation(newValue, Duration);
                ((DiagramItemContainer)dependencyObject).BeginAnimation(Canvas.TopProperty, animation);
            }
            else
            {
                ((DiagramItemContainer)dependencyObject).AnimatedY = newValue;
            }
        }
    }
}
