using Codartis.SoftVis.Rendering.Wpf.Viewport.Commands;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures.Animated
{
    public abstract class ViewportCommandAnimatorBase : Animatable
    {
        public event ViewportCommandHandler ViewportCommand;

        protected readonly IViewportGesture _originalGesture;
        protected readonly Duration _animationDuration;
        protected readonly EasingFunctionBase _easingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };

        protected ViewportCommandAnimatorBase(IViewportGesture originalGesture, Duration animationDuration)
        {
            _originalGesture = originalGesture;
            _animationDuration = animationDuration;
        }

        public abstract void BeginAnimation();

        protected void SendCommand(ViewportCommandBase command)
        {
            if (ViewportCommand != null)
                ViewportCommand(this, command);
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
