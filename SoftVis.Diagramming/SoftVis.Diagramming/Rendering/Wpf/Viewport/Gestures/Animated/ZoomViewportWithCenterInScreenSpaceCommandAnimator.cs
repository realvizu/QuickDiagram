using Codartis.SoftVis.Rendering.Wpf.Viewport.Commands;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures.Animated
{
    public class ZoomViewportWithCenterInScreenSpaceCommandAnimator : ViewportCommandAnimatorBase
    {
        private readonly ZoomViewportWithCenterInScreenSpaceCommand _originalCommand;

        public static readonly DependencyProperty ZoomWithCenterProperty = DependencyProperty.Register("ZoomWithCenter",
            typeof(ZoomWithCenterSpecification), typeof(ZoomViewportWithCenterInScreenSpaceCommandAnimator), 
            new PropertyMetadata(OnZoomWithCenterPropertyChanged));

        public ZoomViewportWithCenterInScreenSpaceCommandAnimator(IViewportGesture originalGesture, Duration animationDuration, 
            ZoomViewportWithCenterInScreenSpaceCommand originalCommand)
            : base(originalGesture, animationDuration)
        {
            _originalCommand = originalCommand;
        }

        public override void BeginAnimation()
        {
            var originalSpecification = new ZoomWithCenterSpecification(
                _originalGesture.Viewport.Zoom, _originalCommand.ZoomCenterInScreenSpace);

            var newSpecification = new ZoomWithCenterSpecification(
                _originalCommand.NewZoom, _originalCommand.ZoomCenterInScreenSpace);

            var animation = new ZoomWithCenterSpecificationAnimation(
                originalSpecification, newSpecification, _animationDuration, _easingFunction);

            BeginAnimation(ZoomWithCenterProperty, animation);
        }

        private static void OnZoomWithCenterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var commandAnimator = obj as ZoomViewportWithCenterInScreenSpaceCommandAnimator;
            commandAnimator.OnZoomWithCenterPropertyChanged((ZoomWithCenterSpecification)args.NewValue);
        }

        private void OnZoomWithCenterPropertyChanged(ZoomWithCenterSpecification newSpecification)
        {
            SendCommand(new ZoomViewportWithCenterInScreenSpaceCommand(_originalGesture, 
                newSpecification.Zoom, newSpecification.CenterInScreenSpace));
        }
    }
}
