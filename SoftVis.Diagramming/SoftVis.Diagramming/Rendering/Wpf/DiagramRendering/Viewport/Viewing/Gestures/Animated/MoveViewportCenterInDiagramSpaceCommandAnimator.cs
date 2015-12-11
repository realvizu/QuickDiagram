using System.Windows;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing.Gestures.Animated
{
    internal class MoveViewportCenterInDiagramSpaceCommandAnimator : ViewportCommandAnimatorBase
    {
        private readonly MoveViewportCenterInDiagramSpaceCommand _originalCommand;

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(MoveViewportCenterInDiagramSpaceCommandAnimator),
            new PropertyMetadata(OnCenterPropertyChanged));

        internal MoveViewportCenterInDiagramSpaceCommandAnimator(IViewportGesture originalGesture, Duration animationDuration,
            MoveViewportCenterInDiagramSpaceCommand originalCommand)
            : base(originalGesture, animationDuration)
        {
            _originalCommand = originalCommand;
        }

        public override void BeginAnimation()
        {
            var animation = new PointAnimation(OriginalGesture.DiagramViewport.ViewportInDiagramSpace.GetCenter(),
                _originalCommand.NewCenterInDiagramSpace, AnimationDuration)
            {
                EasingFunction = EasingFunction
            };
            BeginAnimation(CenterProperty, animation);
        }

        private static void OnCenterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var commandAnimator = (MoveViewportCenterInDiagramSpaceCommandAnimator)obj;
            commandAnimator.OnCenterPropertyChanged((Point)args.NewValue);
        }

        private void OnCenterPropertyChanged(Point newCenter)
        {
            SendCommand(new MoveViewportCenterInDiagramSpaceCommand(OriginalGesture, newCenter));
        }
    }
}
