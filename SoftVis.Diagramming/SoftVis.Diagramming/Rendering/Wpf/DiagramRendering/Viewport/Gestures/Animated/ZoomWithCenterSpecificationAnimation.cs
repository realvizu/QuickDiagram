using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures.Animated
{
    public class ZoomWithCenterSpecificationAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(ZoomWithCenterSpecification), typeof(ZoomWithCenterSpecificationAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(ZoomWithCenterSpecification), typeof(ZoomWithCenterSpecificationAnimation));

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(ZoomWithCenterSpecificationAnimation));

        public static readonly DependencyProperty DoubleAnimationProperty =
            DependencyProperty.Register("DoubleAnimation", typeof(DoubleAnimation), typeof(ZoomWithCenterSpecificationAnimation));

        public ZoomWithCenterSpecification From
        {
            get { return (ZoomWithCenterSpecification)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public ZoomWithCenterSpecification To
        {
            get { return (ZoomWithCenterSpecification)GetValue(ToProperty); }
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

        public ZoomWithCenterSpecificationAnimation()
        { }

        public ZoomWithCenterSpecificationAnimation(ZoomWithCenterSpecification fromValue, ZoomWithCenterSpecification toValue,
            Duration duration, IEasingFunction easingFunction)
        {
            if (fromValue.CenterInScreenSpace != toValue.CenterInScreenSpace)
                throw new ArgumentException("FromValue and ToValue centers must be the same!");

            Duration = duration;
            From = fromValue;
            To = toValue;
            Center = fromValue.CenterInScreenSpace;
            DoubleAnimation = new DoubleAnimation(fromValue.Zoom, toValue.Zoom, duration)
            {
                EasingFunction = easingFunction
            };
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(ZoomWithCenterSpecification);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            var zoomValue = DoubleAnimation.GetCurrentValue(From.Zoom, To.Zoom, animationClock);
            return new ZoomWithCenterSpecification(zoomValue, Center);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ZoomWithCenterSpecificationAnimation();
        }
    }
}
