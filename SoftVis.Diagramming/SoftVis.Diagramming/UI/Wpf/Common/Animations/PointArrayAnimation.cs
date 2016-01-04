using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.UI.Wpf.Common.Animations
{
    /// <summary>
    /// Animates a point array into another point array.
    /// </summary>
    public class PointArrayAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Point[]), typeof(PointArrayAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Point[]), typeof(PointArrayAnimation));

        public static readonly DependencyProperty OriginalToProperty =
            DependencyProperty.Register("OriginalTo", typeof(Point[]), typeof(PointArrayAnimation));

        public static readonly DependencyProperty AnimationsProperty =
            DependencyProperty.Register("Animations", typeof(PointAnimation[]), typeof(PointArrayAnimation));

        public Point[] From
        {
            get { return (Point[])GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public Point[] To
        {
            get { return (Point[])GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public Point[] OriginalTo
        {
            get { return (Point[])GetValue(OriginalToProperty); }
            set { SetValue(OriginalToProperty, value); }
        }

        public PointAnimation[] Animations
        {
            get { return (PointAnimation[])GetValue(AnimationsProperty); }
            set { SetValue(AnimationsProperty, value); }
        }

        private PointArrayAnimation()
        {
        }

        public PointArrayAnimation(Point[] @from, Point[] to, Duration duration, IEasingFunction easingFunction = null)
        {
            if (@from == null || !@from.Any())
                throw new ArgumentException("Array should not be null or empty.", nameof(@from));

            if (to == null || !to.Any())
                throw new ArgumentException("Array should not be null or empty.", nameof(to));

            OriginalTo = to;
            From = CalculateSourceValue(@from, to);
            To = CalculateTargetValue(@from, to);
            Duration = duration;
            Animations = CreatePointAnimations(From, To, Duration, easingFunction);
        }

        public override Type TargetPropertyType => typeof(Point[]);

        private static Point[] CalculateSourceValue(Point[] @from, Point[] to)
        {
            var missingSourcePointCount = to.Length - @from.Length;
            if (missingSourcePointCount > 0)
                return ExtendPointArray(@from, missingSourcePointCount);
            return @from;
        }

        private static Point[] CalculateTargetValue(Point[] @from, Point[] to)
        {
            var missingTargetPointCount = @from.Length - to.Length;
            if (missingTargetPointCount > 0)
                return ExtendPointArray(to, missingTargetPointCount);
            return to;
        }

        private static Point[] ExtendPointArray(Point[] points, int missingPointCount)
        {
            var indexToRepeat = (int)Math.Floor(points.Length / 2d);
            var pointToRepeat = points[indexToRepeat];
            var result = points.ToList();
            result.InsertRange(indexToRepeat, Enumerable.Repeat(pointToRepeat, missingPointCount));
            return result.ToArray();
        }

        private static PointAnimation[] CreatePointAnimations(Point[] @from, Point[] to, Duration duration, IEasingFunction easingFunction)
        {
            var pointAnimations = new PointAnimation[@from.Length];
            for (var i = 0; i < @from.Length; i++)
            {
                pointAnimations[i] = new PointAnimation(@from[i], to[i], duration);
                if (easingFunction != null)
                    pointAnimations[i].EasingFunction = easingFunction;
            }
            return pointAnimations;
        }

        public override object GetCurrentValue(object defaultFrom, object defaultTo, AnimationClock animationClock)
        {
            return animationClock.CurrentProgress.HasValue
                   && animationClock.CurrentProgress.Value.IsEqualWithTolerance(1)
                ? OriginalTo
                : Animations.Select(i => (Point)i.GetCurrentValue(i.From ?? defaultFrom, i.To ?? defaultTo, animationClock)).ToArray();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new PointArrayAnimation();
        }
    }
}
