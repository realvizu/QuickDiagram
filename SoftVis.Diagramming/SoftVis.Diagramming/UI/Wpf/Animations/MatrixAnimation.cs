using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.UI.Wpf.Common;

namespace Codartis.SoftVis.UI.Wpf.Animations
{
    public class MatrixAnimation : MatrixAnimationBase
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Matrix?), typeof(MatrixAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Matrix?), typeof(MatrixAnimation));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(MatrixAnimation));

        public MatrixAnimation()
        {
        }

        public MatrixAnimation(Matrix toValue, Duration duration)
        {
            To = toValue;
            Duration = duration;
        }

        public MatrixAnimation(Matrix toValue, Duration duration, FillBehavior fillBehavior)
        {
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        public MatrixAnimation(Matrix fromValue, Matrix toValue, Duration duration)
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
        }

        public MatrixAnimation(Matrix fromValue, Matrix toValue, Duration duration, FillBehavior fillBehavior)
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        public Matrix? From
        {
            get { return (Matrix?)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public Matrix? To
        {
            get { return (Matrix?)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MatrixAnimation();
        }

        protected override Matrix GetCurrentValueCore(Matrix defaultOriginValue, Matrix defaultDestinationValue, 
            AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
                return Matrix.Identity;

            var animationTime = animationClock.CurrentProgress.Value;
            if (EasingFunction != null)
                animationTime = EasingFunction.Ease(animationTime);

            var from = From ?? defaultOriginValue;
            var to = To ?? defaultDestinationValue;

            var newMatrix = InterpolateMatrix(@from, to, animationTime);
            return newMatrix;
        }

        private Matrix InterpolateMatrix(Matrix @from, Matrix to, double animationTime)
        {
            var m11 = Interpolate(@from.M11, to.M11, animationTime);
            var m12 = Interpolate(@from.M12, to.M12, animationTime);
            var m21 = Interpolate(@from.M21, to.M21, animationTime);
            var m22 = Interpolate(@from.M22, to.M22, animationTime);
            var offsetX = Interpolate(@from.OffsetX, to.OffsetX, animationTime);
            var offsetY = Interpolate(@from.OffsetY, to.OffsetY, animationTime);
            var matrix = new Matrix(m11, m12, m21, m22, offsetX, offsetY);

            //Debug.WriteLine($"{this.GetHashCode():000000} : {matrix.ToDebugString()}");
            return matrix;
        }

        private static double Interpolate(double from, double to, double scale)
        {
            return (to - from) * scale + from;
        }
    }
}
