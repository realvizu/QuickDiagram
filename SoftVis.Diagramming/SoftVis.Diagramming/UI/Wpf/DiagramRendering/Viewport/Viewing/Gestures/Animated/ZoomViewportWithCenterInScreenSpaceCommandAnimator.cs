using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures.Animated
{
    internal class ZoomViewportWithCenterInScreenSpaceCommandAnimator : ViewportCommandAnimatorBase
    {
        private readonly ZoomViewportWithCenterInScreenSpaceCommand _originalCommand;

        public static readonly DependencyProperty ZoomWithCenterProperty = 
            DependencyProperty.Register("ZoomWithCenter", typeof(ZoomWithCenterSpecification), 
                typeof(ZoomViewportWithCenterInScreenSpaceCommandAnimator), 
                new PropertyMetadata(OnZoomWithCenterPropertyChanged));

        internal ZoomViewportWithCenterInScreenSpaceCommandAnimator(IViewportGesture originalGesture, 
            Duration animationDuration, ZoomViewportWithCenterInScreenSpaceCommand originalCommand)
            : base(originalGesture, animationDuration)
        {
            _originalCommand = originalCommand;
        }

        public override void BeginAnimation()
        {
            var originalSpecification = new ZoomWithCenterSpecification(
                OriginalGesture.DiagramViewport.Zoom, _originalCommand.ZoomCenterInScreenSpace);

            var newSpecification = new ZoomWithCenterSpecification(
                _originalCommand.NewZoom, _originalCommand.ZoomCenterInScreenSpace);

            var animation = new ZoomWithCenterSpecificationAnimation(
                originalSpecification, newSpecification, AnimationDuration, EasingFunction);

            BeginAnimation(ZoomWithCenterProperty, animation);
        }

        private static void OnZoomWithCenterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var commandAnimator = (ZoomViewportWithCenterInScreenSpaceCommandAnimator)obj;
            commandAnimator.OnZoomWithCenterPropertyChanged((ZoomWithCenterSpecification)args.NewValue);
        }

        private void OnZoomWithCenterPropertyChanged(ZoomWithCenterSpecification newSpecification)
        {
            SendCommand(new ZoomViewportWithCenterInScreenSpaceCommand(OriginalGesture, 
                newSpecification.Zoom, newSpecification.CenterInScreenSpace));
        }
    }
}
