using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Util.UI.Wpf.Animations;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A canvas whose RenderTransform is animated.
    /// </summary>
    internal class AnimatedRenderTransformCanvas : Canvas
    {
        private const int FrameRate = 30;
        private static readonly Duration ShortAnimationDurationDefault = TimeSpan.FromMilliseconds(200);
        private static readonly Duration LongAnimationDurationDefault = TimeSpan.FromMilliseconds(500);

        static AnimatedRenderTransformCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedRenderTransformCanvas),
                new FrameworkPropertyMetadata(typeof(AnimatedRenderTransformCanvas)));
        }

        public static readonly DependencyProperty AnimatedRenderTransformProperty =
            DependencyProperty.Register("AnimatedRenderTransform", typeof(TransitionedTransform), typeof(AnimatedRenderTransformCanvas),
                new FrameworkPropertyMetadata(TransitionedTransform.Identity, 
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnTransitionedTransformChanged));

        private static void OnTransitionedTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedRenderTransformCanvas) d).TransitionTransform((TransitionedTransform) e.OldValue, (TransitionedTransform) e.NewValue);

        public static readonly DependencyProperty ShortAnimationDurationProperty =
            DependencyProperty.Register("ShortAnimationDuration", typeof(Duration), typeof(AnimatedRenderTransformCanvas),
                new PropertyMetadata(ShortAnimationDurationDefault));

        public static readonly DependencyProperty LongAnimationDurationProperty =
            DependencyProperty.Register("LongAnimationDuration", typeof(Duration), typeof(AnimatedRenderTransformCanvas),
                new PropertyMetadata(LongAnimationDurationDefault));

        public static readonly DependencyProperty LongAnimationEasingFunctionProperty =
            DependencyProperty.Register("LongAnimationEasingFunction", typeof(EasingFunctionBase), typeof(AnimatedRenderTransformCanvas));

        public AnimatedRenderTransformCanvas()
        {
            RenderTransform = new MatrixTransform();
        }

        public TransitionedTransform AnimatedRenderTransform
        {
            get { return (TransitionedTransform)GetValue(AnimatedRenderTransformProperty); }
            set { SetValue(AnimatedRenderTransformProperty, value); }
        }

        public Duration ShortAnimationDuration
        {
            get { return (Duration)GetValue(ShortAnimationDurationProperty); }
            set { SetValue(ShortAnimationDurationProperty, value); }
        }

        public Duration LongAnimationDuration
        {
            get { return (Duration)GetValue(LongAnimationDurationProperty); }
            set { SetValue(LongAnimationDurationProperty, value); }
        }

        public EasingFunctionBase LongAnimationEasingFunction
        {
            get { return (EasingFunctionBase)GetValue(LongAnimationEasingFunctionProperty); }
            set { SetValue(LongAnimationEasingFunctionProperty, value); }
        }

        private void TransitionTransform(TransitionedTransform oldTransitionedTransform, TransitionedTransform newTransitionedTransform)
        {
            if (oldTransitionedTransform.Transform.Value == newTransitionedTransform.Transform.Value)
                return;

            if (newTransitionedTransform.TransitionSpeed == TransitionSpeed.Instant)
            {
                DontAnimate(newTransitionedTransform.Transform);
            }
            else
            {
                Animate(oldTransitionedTransform.Transform, newTransitionedTransform.Transform, newTransitionedTransform.TransitionSpeed);
            }
        }

        private void DontAnimate(Transform newTransform)
        {
            RenderTransform.BeginAnimation(MatrixTransform.MatrixProperty, null);
            ((MatrixTransform)RenderTransform).Matrix = newTransform.Value;
        }

        private void Animate(Transform oldValue, Transform newValue, TransitionSpeed transitionSpeed)
        {
            var duration = TransitionSpeedToDuration(transitionSpeed);

            var matrixAnimation = new MatrixAnimation(oldValue.Value, newValue.Value, duration, FillBehavior.HoldEnd);

            if (transitionSpeed == TransitionSpeed.Slow)
                matrixAnimation.EasingFunction = LongAnimationEasingFunction;

            Timeline.SetDesiredFrameRate(matrixAnimation, FrameRate);

            RenderTransform.BeginAnimation(MatrixTransform.MatrixProperty, matrixAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        private Duration TransitionSpeedToDuration(TransitionSpeed transitionSpeed)
        {
            switch (transitionSpeed)
            {
                case TransitionSpeed.Instant: return TimeSpan.Zero;
                case TransitionSpeed.Fast: return ShortAnimationDuration;
                case TransitionSpeed.Slow: return LongAnimationDuration;
                default: throw new NotImplementedException($"Unexpected TransitionSpeed:{transitionSpeed}");
            }
        }

    }
}