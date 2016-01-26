using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Animations;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A canvas with an additional Transform that is animated for each change.
    /// </summary>
    internal class AnimatedTransformCanvas : TransformCanvas
    {
        private const int FrameRate = 30;
        private static readonly Duration ShortAnimationDurationDefault = TimeSpan.FromMilliseconds(200);
        private static readonly Duration LongAnimationDurationDefault = TimeSpan.FromMilliseconds(500);

        static AnimatedTransformCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedTransformCanvas),
                new FrameworkPropertyMetadata(typeof(AnimatedTransformCanvas)));
        }

        public static readonly DependencyProperty TransitionedTransformProperty =
            DependencyProperty.Register("TransitionedTransform", typeof(TransitionedTransform), typeof(AnimatedTransformCanvas),
                new FrameworkPropertyMetadata(TransitionedTransform.Identity, 
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnTransitionedTransformChanged));

        private static void OnTransitionedTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((AnimatedTransformCanvas) d).TransitionTransform((TransitionedTransform) e.OldValue, (TransitionedTransform) e.NewValue);

        public static readonly DependencyProperty ShortAnimationDurationProperty =
            DependencyProperty.Register("ShortAnimationDuration", typeof(Duration), typeof(AnimatedTransformCanvas),
                new PropertyMetadata(ShortAnimationDurationDefault));

        public static readonly DependencyProperty LongAnimationDurationProperty =
            DependencyProperty.Register("LongAnimationDuration", typeof(Duration), typeof(AnimatedTransformCanvas),
                new PropertyMetadata(LongAnimationDurationDefault));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(AnimatedTransformCanvas));

        public AnimatedTransformCanvas()
        {
            Transform = new MatrixTransform();
        }

        public TransitionedTransform TransitionedTransform
        {
            get { return (TransitionedTransform)GetValue(TransitionedTransformProperty); }
            set { SetValue(TransitionedTransformProperty, value); }
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

        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
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
            Transform.BeginAnimation(MatrixTransform.MatrixProperty, null);
            ((MatrixTransform) Transform).Matrix = newTransform.Value;
        }

        private void Animate(Transform oldValue, Transform newValue, TransitionSpeed transitionSpeed)
        {
            var duration = TransitionSpeedToDuration(transitionSpeed);

            var matrixAnimation = new MatrixAnimation(oldValue.Value, newValue.Value, duration, FillBehavior.HoldEnd);

            if (transitionSpeed == TransitionSpeed.Slow)
                matrixAnimation.EasingFunction = EasingFunction;

            Timeline.SetDesiredFrameRate(matrixAnimation, FrameRate);

            Transform.BeginAnimation(MatrixTransform.MatrixProperty, matrixAnimation, HandoffBehavior.SnapshotAndReplace);
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