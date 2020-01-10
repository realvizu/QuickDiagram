using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Util.UI.Wpf.Animations;
using Codartis.SoftVis.Util.UI.Wpf.Transforms;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// A canvas whose RenderTransform is animated.
    /// </summary>
    public class AnimatedRenderTransformCanvas : Canvas
    {
        private const int FrameRate = 30;
        private static readonly Duration FastAnimationDurationDefault = TimeSpan.FromMilliseconds(200);
        private static readonly Duration MediumAnimationDurationDefault = TimeSpan.FromMilliseconds(500);
        private static readonly Duration SlowAnimationDurationDefault = TimeSpan.FromMilliseconds(1000);

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
            => ((AnimatedRenderTransformCanvas)d).TransitionTransform((TransitionedTransform)e.OldValue, (TransitionedTransform)e.NewValue);

        public static readonly DependencyProperty FastAnimationDurationProperty =
            DependencyProperty.Register("FastAnimationDuration", typeof(Duration), typeof(AnimatedRenderTransformCanvas),
                new PropertyMetadata(FastAnimationDurationDefault));

        public static readonly DependencyProperty MediumAnimationDurationProperty =
            DependencyProperty.Register("MediumAnimationDuration", typeof(Duration), typeof(AnimatedRenderTransformCanvas),
                new PropertyMetadata(MediumAnimationDurationDefault));

        public static readonly DependencyProperty SlowAnimationDurationProperty =
            DependencyProperty.Register("SlowAnimationDuration", typeof(Duration), typeof(AnimatedRenderTransformCanvas),
                new PropertyMetadata(SlowAnimationDurationDefault));

        public static readonly DependencyProperty MediumAnimationEasingFunctionProperty =
            DependencyProperty.Register("MediumAnimationEasingFunction", typeof(EasingFunctionBase), typeof(AnimatedRenderTransformCanvas));

        public static readonly DependencyProperty SlowAnimationEasingFunctionProperty =
            DependencyProperty.Register("SlowAnimationEasingFunction", typeof(EasingFunctionBase), typeof(AnimatedRenderTransformCanvas));

        public AnimatedRenderTransformCanvas()
        {
            RenderTransform = new MatrixTransform();
        }

        public TransitionedTransform AnimatedRenderTransform
        {
            get { return (TransitionedTransform)GetValue(AnimatedRenderTransformProperty); }
            set { SetValue(AnimatedRenderTransformProperty, value); }
        }

        public Duration FastAnimationDuration
        {
            get { return (Duration)GetValue(FastAnimationDurationProperty); }
            set { SetValue(FastAnimationDurationProperty, value); }
        }

        public Duration MediumAnimationDuration
        {
            get { return (Duration)GetValue(MediumAnimationDurationProperty); }
            set { SetValue(MediumAnimationDurationProperty, value); }
        }

        public Duration SlowAnimationDuration
        {
            get { return (Duration)GetValue(SlowAnimationDurationProperty); }
            set { SetValue(SlowAnimationDurationProperty, value); }
        }

        public EasingFunctionBase MediumAnimationEasingFunction
        {
            get { return (EasingFunctionBase)GetValue(MediumAnimationEasingFunctionProperty); }
            set { SetValue(MediumAnimationEasingFunctionProperty, value); }
        }

        public EasingFunctionBase SlowAnimationEasingFunction
        {
            get { return (EasingFunctionBase)GetValue(SlowAnimationEasingFunctionProperty); }
            set { SetValue(SlowAnimationEasingFunctionProperty, value); }
        }

        // ReSharper disable once UnusedParameter.Local
        private void TransitionTransform(TransitionedTransform oldTransitionedTransform, TransitionedTransform newTransitionedTransform)
        {
            if (newTransitionedTransform.TransitionSpeed == TransitionSpeed.Instant)
                DontAnimate(newTransitionedTransform.Transform);
            else
                Animate(newTransitionedTransform.Transform, newTransitionedTransform.TransitionSpeed);
        }

        private void DontAnimate(Transform newTransform)
        {
            RenderTransform.BeginAnimation(MatrixTransform.MatrixProperty, null);
            ((MatrixTransform)RenderTransform).Matrix = newTransform.Value;
        }

        private void Animate(Transform newValue, TransitionSpeed transitionSpeed)
        {
            var duration = TransitionSpeedToDuration(transitionSpeed);

            var matrixAnimation = new MatrixAnimation(newValue.Value, duration, FillBehavior.HoldEnd)
            {
                EasingFunction = GetEasingFunction(transitionSpeed)
            };

            Timeline.SetDesiredFrameRate(matrixAnimation, FrameRate);

            RenderTransform.BeginAnimation(MatrixTransform.MatrixProperty, matrixAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        private EasingFunctionBase GetEasingFunction(TransitionSpeed transitionSpeed)
        {
            switch (transitionSpeed)
            {
                case TransitionSpeed.Medium:
                    return MediumAnimationEasingFunction;
                case TransitionSpeed.Slow:
                    return SlowAnimationEasingFunction;
                default:
                    return null;
            }
        }

        private Duration TransitionSpeedToDuration(TransitionSpeed transitionSpeed)
        {
            switch (transitionSpeed)
            {
                case TransitionSpeed.Instant: return TimeSpan.Zero;
                case TransitionSpeed.Fast: return FastAnimationDuration;
                case TransitionSpeed.Medium: return MediumAnimationDuration;
                case TransitionSpeed.Slow: return SlowAnimationDuration;
                default: throw new NotImplementedException($"Unexpected TransitionSpeed:{transitionSpeed}");
            }
        }

    }
}