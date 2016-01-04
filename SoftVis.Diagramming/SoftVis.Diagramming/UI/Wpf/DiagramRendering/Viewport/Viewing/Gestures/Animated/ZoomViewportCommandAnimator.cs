using System.Windows;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures.Animated
{
    internal class ZoomViewportCommandAnimator : ViewportCommandAnimatorBase
    {
        private readonly ZoomViewportCommand _originalCommand;

        public static readonly DependencyProperty ZoomProperty = 
            DependencyProperty.Register("Zoom", typeof(double), typeof(ZoomViewportCommandAnimator), 
                new PropertyMetadata(OnZoomPropertyChanged));

        internal ZoomViewportCommandAnimator(IViewportGesture originalGesture, Duration animationDuration,
            ZoomViewportCommand originalCommand)
            : base(originalGesture, animationDuration)
        {
            _originalCommand = originalCommand;
        }

        public override void BeginAnimation()
        {
            var animation = new DoubleAnimation(OriginalGesture.DiagramViewport.Zoom, 
                _originalCommand.NewZoom, AnimationDuration)
            {
                EasingFunction = EasingFunction
            };
            BeginAnimation(ZoomProperty, animation);
        }

        private static void OnZoomPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var commandAnimator = (ZoomViewportCommandAnimator)obj;
            commandAnimator.OnZoomPropertyChanged((double)args.NewValue);
        }

        private void OnZoomPropertyChanged(double newZoom)
        {
            SendCommand(new ZoomViewportCommand(OriginalGesture, newZoom));
        }
    }
}
