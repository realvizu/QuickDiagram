using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// Wraps an item that is positioned on a canvas and animates appear, move, disappear.
    /// </summary>
    public class AnimatedContentPresenter : ContentPresenter
    {
        private static readonly Duration AnimationDurationDefault = TimeSpan.FromMilliseconds(250);

        private bool _isDisappearing;

        static AnimatedContentPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(typeof(AnimatedContentPresenter)));
        }

        public AnimatedContentPresenter()
        {
            // Make sure that this control is initially not visible otherwise its appear effect won't work.
            SetValue(VisibilityProperty, Visibility.Hidden);
        }

        public static readonly DependencyProperty AnimatedLeftProperty =
            DependencyProperty.Register("AnimatedLeft", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, 
                    //FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnAnimatedLeftChanged));

        private static void OnAnimatedLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnAnimatedLeftChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty AnimatedTopProperty =
            DependencyProperty.Register("AnimatedTop", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, 
                    //FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OnAnimatedTopChanged));

        private static void OnAnimatedTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnAnimatedTopChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty TargetPropertyForLeftProperty =
            DependencyProperty.Register("TargetPropertyForLeft", typeof(DependencyProperty), typeof(AnimatedContentPresenter));

        public static readonly DependencyProperty TargetPropertyForTopProperty =
            DependencyProperty.Register("TargetPropertyForTop", typeof(DependencyProperty), typeof(AnimatedContentPresenter));

        public static readonly DependencyProperty ScalingProperty =
            DependencyProperty.Register("Scaling", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(1d,
                    //FrameworkPropertyMetadataOptions.AffectsParentArrange,
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

        public DependencyProperty TargetPropertyForLeft
        {
            get { return (DependencyProperty)GetValue(TargetPropertyForLeftProperty); }
            set { SetValue(TargetPropertyForLeftProperty, value); }
        }

        public DependencyProperty TargetPropertyForTop
        {
            get { return (DependencyProperty)GetValue(TargetPropertyForTopProperty); }
            set { SetValue(TargetPropertyForTopProperty, value); }
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
            _isDisappearing = true;
            StopAnimation(TargetPropertyForTop);
            StopAnimation(TargetPropertyForLeft);
            AnimateDisappear(readyToBeRemovedCallback);
        }

        private void OnAnimatedLeftChanged(double oldValue, double newValue)
        {
            UpdatePosition(TargetPropertyForLeft, oldValue, newValue);
        }

        private void OnAnimatedTopChanged(double oldValue, double newValue)
        {
            UpdatePosition(TargetPropertyForTop, oldValue, newValue);
        }

        private void UpdatePosition(DependencyProperty property, double oldValue, double newValue)
        {
            if (_isDisappearing)
                return;

            var animateAppear = double.IsNaN(oldValue) && !double.IsNaN(newValue);
            if (animateAppear)
            {
                SetValue(property, newValue);
                AnimateAppear();
                SetValue(VisibilityProperty, Visibility.Visible);
            }

            var animateMove = !double.IsNaN(oldValue) && !double.IsNaN(newValue);
            if (animateMove)
            {
                AnimateMove(property, newValue);
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

        private void StopAnimation(DependencyProperty property)
        {
            var animation = new DoubleAnimation {BeginTime = null};
            BeginAnimation(property, animation);
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
