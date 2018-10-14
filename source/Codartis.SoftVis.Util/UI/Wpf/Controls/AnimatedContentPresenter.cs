using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// Wraps an item that is positioned on a panel and animates appear, move, disappear.
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

        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, OnLeftChanged));

        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnLeftChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, OnTopChanged));

        private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnTopChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty LeftToAnimateProperty =
            DependencyProperty.Register("LeftToAnimate", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, OnLeftToAnimateChanged));

        private static void OnLeftToAnimateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnLeftToAnimateChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty TopToAnimateProperty =
            DependencyProperty.Register("TopToAnimate", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(double.NaN, OnTopToAnimateChanged));

        private static void OnTopToAnimateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnTopToAnimateChanged((double)e.OldValue, (double)e.NewValue);

        public static readonly DependencyProperty ScalingProperty =
            DependencyProperty.Register("Scaling", typeof(double), typeof(AnimatedContentPresenter),
                new FrameworkPropertyMetadata(1d, OnScalingChanged));

        private static void OnScalingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedContentPresenter)d).OnScalingChanged();

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(AnimatedContentPresenter),
                new PropertyMetadata(AnimationDurationDefault));

        public static readonly DependencyProperty AnimatedRectProperty =
            DependencyProperty.Register("AnimatedRect", typeof(Rect), typeof(AnimatedContentPresenter));

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        public double LeftToAnimate
        {
            get { return (double)GetValue(LeftToAnimateProperty); }
            set { SetValue(LeftToAnimateProperty, value); }
        }

        public double TopToAnimate
        {
            get { return (double)GetValue(TopToAnimateProperty); }
            set { SetValue(TopToAnimateProperty, value); }
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

        public Rect AnimatedRect
        {
            get { return (Rect)GetValue(AnimatedRectProperty); }
            set { SetValue(AnimatedRectProperty, value); }
        }

        public void OnBeforeRemove(Action<ViewModelBase> readyToBeRemovedCallback)
        {
            _isDisappearing = true;
            StopAnimation(TopProperty);
            StopAnimation(LeftProperty);
            AnimateDisappear(readyToBeRemovedCallback);
        }

        private void OnLeftChanged(double oldValue, double newValue) 
            => UpdatePosition(oldValue, newValue);

        private void OnTopChanged(double oldValue, double newValue) 
            => UpdatePosition(oldValue, newValue);

        private void OnLeftToAnimateChanged(double oldValue, double newValue)
            => UpdateAnimatedPosition(LeftProperty, oldValue, newValue);

        private void OnTopToAnimateChanged(double oldValue, double newValue)
            => UpdateAnimatedPosition(TopProperty, oldValue, newValue);

        private void UpdatePosition(double oldValue, double newValue)
        {
            AnimatedRect = new Rect(Left, Top, ActualWidth, ActualHeight);

            if (_isDisappearing)
                return;

            if (oldValue.IsUndefined() && newValue.IsDefined())
                AnimateAppear();

            SetVisibilityBasedOnPosition(newValue);
        }

        private void SetVisibilityBasedOnPosition(double newValue)
        {
            var newVisibility = newValue.IsDefined() ? Visibility.Visible : Visibility.Hidden;
            SetValue(VisibilityProperty, newVisibility);
        }

        private void UpdateAnimatedPosition(DependencyProperty property, double oldValue, double newValue)
        {
            if (_isDisappearing)
                return;

            if (oldValue.IsUndefined() && newValue.IsDefined())
                SetValue(property, newValue);

            if (oldValue.IsDefined() && newValue.IsDefined())
                AnimateMove(property, newValue);
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
            var animation = new DoubleAnimation { BeginTime = null };
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
