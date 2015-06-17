using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    public class ScaleSpecificationAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(ScaleSpecification), typeof(ScaleSpecificationAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(ScaleSpecification), typeof(ScaleSpecificationAnimation));

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(ScaleSpecificationAnimation));

        public static readonly DependencyProperty DoubleAnimationProperty =
            DependencyProperty.Register("DoubleAnimation", typeof(DoubleAnimation), typeof(ScaleSpecificationAnimation));

        public ScaleSpecification From
        {
            get { return (ScaleSpecification)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public ScaleSpecification To
        {
            get { return (ScaleSpecification)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public DoubleAnimation DoubleAnimation
        {
            get { return (DoubleAnimation)GetValue(DoubleAnimationProperty); }
            set { SetValue(DoubleAnimationProperty, value); }
        }

        public ScaleSpecificationAnimation()
        { }

        public ScaleSpecificationAnimation(ScaleSpecification fromValue, ScaleSpecification toValue, Duration duration,
            IEasingFunction easingFunction)
        {
            if (fromValue.Center != toValue.Center)
                throw new ArgumentException("FromValue and ToValue centers must be the same!");

            Duration = duration;
            From = fromValue;
            To = toValue;
            Center = fromValue.Center;
            DoubleAnimation = new DoubleAnimation(fromValue.Scale, toValue.Scale, duration)
            {
                EasingFunction = easingFunction
            };
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(ScaleSpecification);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            var scaleValue = DoubleAnimation.GetCurrentValue(From.Scale, To.Scale, animationClock);
            return new ScaleSpecification(scaleValue, Center);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ScaleSpecificationAnimation();
        }
    }
}
