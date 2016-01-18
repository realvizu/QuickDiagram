using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Wraps a diagram item. Animates appear, move, disappear.
    /// </summary>
    internal class DiagramItemContainer : Control
    {
        private static readonly Duration AnimationDurationDefault = TimeSpan.FromMilliseconds(250);

        static DiagramItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramItemContainer),
                new FrameworkPropertyMetadata(typeof(DiagramItemContainer)));
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(DiagramItemContainer),
                new PropertyMetadata(double.NaN, OnXPropertyChanged));

        private static void OnXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DiagramItemContainer)d).OnXPropertyChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(DiagramItemContainer),
                new PropertyMetadata(double.NaN, OnYPropertyChanged));

        private static void OnYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DiagramItemContainer)d).OnYPropertyChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty AnimatedXProperty =
            DependencyProperty.Register("AnimatedX", typeof(double), typeof(DiagramItemContainer));

        public static readonly DependencyProperty AnimatedYProperty =
            DependencyProperty.Register("AnimatedY", typeof(double), typeof(DiagramItemContainer));

        public static readonly DependencyProperty ScalingProperty =
            DependencyProperty.Register("Scaling", typeof(double), typeof(DiagramItemContainer),
                new FrameworkPropertyMetadata(1d,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(DiagramItemContainer),
                new PropertyMetadata(AnimationDurationDefault));

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

        public double Scaling
        {
            get { return (double)GetValue(ScalingProperty); }
            set { SetValue(ScalingProperty, value); }
        }

        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public void OnBeforeRemove(Action<DiagramShapeViewModelBase> readyToBeRemovedCallback)
        {
            AnimateDisappear(readyToBeRemovedCallback);
        }

        private void OnXPropertyChanged(double oldValue, double newValue)
        {
            UpdatePosition(AnimatedXProperty, oldValue, newValue);
        }

        private void OnYPropertyChanged(double oldValue, double newValue)
        {
            UpdatePosition(AnimatedYProperty, oldValue, newValue);
        }

        private void UpdatePosition(DependencyProperty property, double oldValue, double newValue)
        {
            var animate = !double.IsNaN(oldValue);
            if (animate)
            {
                AnimateMove(property, newValue);
            }
            else
            {
                SetValue(property, newValue);
                AnimateAppear();
            }
        }

        private void AnimateAppear()
        {
            var animation = new DoubleAnimation(0, 1, AnimationDuration);
            BeginAnimation(ScalingProperty, animation);
        }

        private void AnimateMove(DependencyProperty property, double value)
        {
            var animation = new DoubleAnimation(value, AnimationDuration);
            BeginAnimation(property, animation);
        }

        private void AnimateDisappear(Action<DiagramShapeViewModelBase> readyToBeRemovedCallback)
        {
            var animation = new DoubleAnimation(1, 0, AnimationDuration);

            EventHandler onAnimationCompleted = null;
            onAnimationCompleted = (sender, e) =>
            {
                animation.Completed -= onAnimationCompleted;
                readyToBeRemovedCallback((DiagramShapeViewModelBase)DataContext);
            };

            animation.Completed += onAnimationCompleted;
            BeginAnimation(ScalingProperty, animation);
        }
    }
}
