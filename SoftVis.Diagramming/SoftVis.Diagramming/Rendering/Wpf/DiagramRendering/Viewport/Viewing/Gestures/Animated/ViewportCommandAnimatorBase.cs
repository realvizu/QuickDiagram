using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing.Gestures.Animated
{
    internal abstract class ViewportCommandAnimatorBase : Animatable
    {
        public event ViewportCommandHandler ViewportCommand;

        protected readonly IViewportGesture OriginalGesture;
        protected readonly Duration AnimationDuration;
        protected readonly EasingFunctionBase EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

        protected ViewportCommandAnimatorBase(IViewportGesture originalGesture, Duration animationDuration)
        {
            OriginalGesture = originalGesture;
            AnimationDuration = animationDuration;
        }

        public abstract void BeginAnimation();

        protected void SendCommand(ViewportCommandBase command)
        {
            ViewportCommand?.Invoke(this, command);
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
