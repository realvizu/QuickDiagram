using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Wraps an item that is positioned on a canvas and animates appear, move, disappear.
    /// </summary>
    internal class AnimatedContentPresenter : ContentPresenter
    {
        private static readonly Duration AnimationDurationDefault = TimeSpan.FromMilliseconds(250);

        static AnimatedContentPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(typeof(AnimatedContentPresenter)));
        }

        public static readonly DependencyProperty AnimatedLeftProperty =
            DependencyProperty.Register("AnimatedLeft", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, 
                    FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnAnimatedLeftChanged));

        private static void OnAnimatedLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnAnimatedLeftChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty AnimatedTopProperty =
            DependencyProperty.Register("AnimatedTop", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, 
                    FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnAnimatedTopChanged));

        private static void OnAnimatedTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnAnimatedTopChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty ScalingProperty =
            DependencyProperty.Register("Scaling", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(1d,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnScalingChanged));

        private static void OnScalingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter) d).OnScalingChanged();

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(AnimatedContentPresenter),
                new PropertyMetadata(AnimationDurationDefault));

        public double AnimatedLeft
        {
            get { return (double)GetValue(AnimatedLeftProperty); }
            set { SetValue(AnimatedLeftProperty, value); }
        }

        public double AnimatedTop
        {
            get { return (double)GetValue(AnimatedTopProperty); }
            set { SetValue(AnimatedTopProperty, value); }
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

        public void OnBeforeRemove(Action<ViewModelBase> readyToBeRemovedCallback)
        {
            AnimateDisappear(readyToBeRemovedCallback);
        }

        private void OnAnimatedLeftChanged(double oldValue, double newValue)
        {
            UpdatePosition(Canvas.LeftProperty, oldValue, newValue);
        }

        private void OnAnimatedTopChanged(double oldValue, double newValue)
        {
            UpdatePosition(Canvas.TopProperty, oldValue, newValue);
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

        private void AnimateDisappear(Action<ViewModelBase> readyToBeRemovedCallback)
        {
            var animation = new DoubleAnimation(1, 0, AnimationDuration);

            EventHandler onAnimationCompleted = null;
            onAnimationCompleted = (sender, e) =>
            {
                animation.Completed -= onAnimationCompleted;
                readyToBeRemovedCallback((ViewModelBase)DataContext);
            };

            animation.Completed += onAnimationCompleted;
            BeginAnimation(ScalingProperty, animation);
        }

        private void OnScalingChanged()
        {
            RenderTransform = GetScalingTransform();
        }

        private Transform GetScalingTransform()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(-ActualWidth / 2, -ActualHeight / 2));
            transform.Children.Add(new ScaleTransform(Scaling, Scaling));
            transform.Children.Add(new TranslateTransform(ActualWidth / 2, ActualHeight / 2));
            return transform;
        }
    }
}
