using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Wraps an item that is positioned on a canvas. 
    /// Animates appear, move, disappear.
    /// </summary>
    internal class AnimatedPositionedItemContainer : PositionedItemContainer
    {
        private static readonly Duration AnimationDurationDefault = TimeSpan.FromMilliseconds(250);

        static AnimatedPositionedItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedPositionedItemContainer),
                new FrameworkPropertyMetadata(typeof(AnimatedPositionedItemContainer)));
        }

        public static readonly DependencyProperty AnimatedXProperty =
            DependencyProperty.Register("AnimatedX", typeof(double), typeof(AnimatedPositionedItemContainer),
                new FrameworkPropertyMetadata(double.NaN, 
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty AnimatedYProperty =
            DependencyProperty.Register("AnimatedY", typeof(double), typeof(AnimatedPositionedItemContainer),
                new FrameworkPropertyMetadata(double.NaN, 
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty ScalingProperty =
            DependencyProperty.Register("Scaling", typeof(double), typeof(AnimatedPositionedItemContainer),
                new FrameworkPropertyMetadata(1d,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(AnimatedPositionedItemContainer),
                new PropertyMetadata(AnimationDurationDefault));

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

        public void OnBeforeRemove(Action<ViewModelBase> readyToBeRemovedCallback)
        {
            AnimateDisappear(readyToBeRemovedCallback);
        }

        public override Transform GetItemTransform()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(-ActualWidth / 2, -ActualHeight / 2));
            transform.Children.Add(new ScaleTransform(Scaling, Scaling));
            transform.Children.Add(new TranslateTransform(ActualWidth / 2, ActualHeight / 2));
            return transform;
        }

        protected override void OnXPropertyChanged(double oldValue, double newValue)
        {
            UpdatePosition(AnimatedXProperty, oldValue, newValue);
        }

        protected override void OnYPropertyChanged(double oldValue, double newValue)
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
    }
}
