using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    /// <summary>
    /// Smooths out the scale and transform changes during zoom calculation.
    /// </summary>
    internal class AnimatedGesture : Animatable, IGesture
    {
        public event ScaleChangedEventHandler ScaleChanged;
        public event TranslateChangedEventHandler TranslateChanged;

        private readonly IGesture _gesture;
        private readonly Duration _animationDuration;
        private readonly EasingFunctionBase _easingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };

        public AnimatedGesture(IGesture gesture, TimeSpan animationTimeSpan)
        {
            _gesture = gesture;
            _gesture.ScaleChanged += OnScaleChanged;
            _gesture.TranslateChanged += OnTranslateChanged;
            _animationDuration = new Duration(animationTimeSpan);
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", 
            typeof(ScaleSpecification), typeof(AnimatedGesture), new PropertyMetadata(OnScalePropertyChanged));

        public static readonly DependencyProperty TranslateProperty = DependencyProperty.Register("Translate", 
            typeof(Vector), typeof(AnimatedGesture), new PropertyMetadata(OnTranslatePropertyChanged));

        public IGestureTarget Target
        {
            get { return _gesture.Target; }
        }

        private void OnScaleChanged(object sender, ScaleChangedEventArgs args)
        {
            var originScaleSpecification = new ScaleSpecification(_gesture.Target.LinearScale, args.ScaleCenter);
            var destinationScaleSpecification = new ScaleSpecification(args.NewScale, args.ScaleCenter);
            var animation = new ScaleSpecificationAnimation(originScaleSpecification, destinationScaleSpecification, _animationDuration, _easingFunction);
            BeginAnimation(ScaleProperty, animation);
        }

        private void OnTranslateChanged(object sender, TranslateChangedEventArgs args)
        {
            var animation = new VectorAnimation(_gesture.Target.Translate, args.NewTranslate, _animationDuration)
            {
                EasingFunction = _easingFunction
            };
            BeginAnimation(TranslateProperty, animation);
        }

        private static void OnScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scaleSpecification = (ScaleSpecification)e.NewValue;
            var a = d as AnimatedGesture;
            a.OnScaleChanged(scaleSpecification.Scale, scaleSpecification.Center);
        }

        private static void OnTranslatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var a = d as AnimatedGesture;
            a.OnTranslateChanged((Vector)e.NewValue);
        }

        private void OnScaleChanged(double scale, Point center)
        {
            if (ScaleChanged != null)
                ScaleChanged(_gesture, new ScaleChangedEventArgs(scale, center));
        }

        private void OnTranslateChanged(Vector translate)
        {
            if (TranslateChanged != null)
                TranslateChanged(_gesture, new TranslateChangedEventArgs(translate));
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
