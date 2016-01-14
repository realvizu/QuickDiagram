using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        public static readonly DependencyProperty HintedTransformProperty =
            DependencyProperty.Register("HintedTransform", typeof(HintedTransform), typeof(AnimatedTransformCanvas),
                new FrameworkPropertyMetadata(HintedTransform.Identity, OnHintedTransformChanged));

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

        public HintedTransform HintedTransform
        {
            get { return (HintedTransform)GetValue(HintedTransformProperty); }
            set { SetValue(HintedTransformProperty, value); }
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

        private static void OnHintedTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedTransformCanvas)d).AnimateTransform((HintedTransform)e.OldValue, (HintedTransform)e.NewValue);
        }

        private void AnimateTransform(HintedTransform oldHintedTransform, HintedTransform newHintedTransform)
        {
            if (oldHintedTransform.Transform.Value == newHintedTransform.Transform.Value)
                return;

            if (newHintedTransform.AnimationHint == AnimationHint.None)
            {
                DontAnimate(newHintedTransform.Transform);
            }
            else
            {
                Animate(oldHintedTransform.Transform, newHintedTransform.Transform, newHintedTransform.AnimationHint);
            }
        }

        private void DontAnimate(Transform newTransform)
        {
            Transform.BeginAnimation(MatrixTransform.MatrixProperty, null);
            ((MatrixTransform) Transform).Matrix = newTransform.Value;
        }

        private void Animate(Transform oldValue, Transform newValue, AnimationHint animationHint)
        {
            var duration = AnimationHintToDuration(animationHint);

            var matrixAnimation = new MatrixAnimation(oldValue.Value, newValue.Value, duration, FillBehavior.HoldEnd);

            if (animationHint == AnimationHint.Long)
                matrixAnimation.EasingFunction = EasingFunction;

            Timeline.SetDesiredFrameRate(matrixAnimation, FrameRate);

            Transform.BeginAnimation(MatrixTransform.MatrixProperty, matrixAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        private Duration AnimationHintToDuration(AnimationHint animationHint)
        {
            switch (animationHint)
            {
                case AnimationHint.None: return TimeSpan.Zero;
                case AnimationHint.Short: return ShortAnimationDuration;
                case AnimationHint.Long: return LongAnimationDuration;
                default: throw new NotImplementedException($"Unexpected AnimationHint:{animationHint}");
            }
        }

    }
}