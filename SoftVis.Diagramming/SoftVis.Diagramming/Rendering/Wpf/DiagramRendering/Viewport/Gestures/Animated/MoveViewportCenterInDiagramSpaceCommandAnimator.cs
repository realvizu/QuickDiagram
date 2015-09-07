using System.Windows;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures.Animated
{
    public class MoveViewportCenterInDiagramSpaceCommandAnimator : ViewportCommandAnimatorBase
    {
        private readonly MoveViewportCenterInDiagramSpaceCommand _originalCommand;

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center",
            typeof(Point), typeof(MoveViewportCenterInDiagramSpaceCommandAnimator), new PropertyMetadata(OnCenterPropertyChanged));

        public MoveViewportCenterInDiagramSpaceCommandAnimator(IViewportGesture originalGesture, Duration animationDuration, 
            MoveViewportCenterInDiagramSpaceCommand originalCommand)
            : base(originalGesture, animationDuration)
        {
            _originalCommand = originalCommand;
        }

        public override void BeginAnimation()
        {
            var animation = new PointAnimation(_originalGesture.DiagramViewport.ViewportInDiagramSpace.GetCenter(),
                _originalCommand.NewCenterInDiagramSpace, _animationDuration)
            {
                EasingFunction = _easingFunction
            };
            BeginAnimation(CenterProperty, animation);
        }

        private static void OnCenterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var commandAnimator = obj as MoveViewportCenterInDiagramSpaceCommandAnimator;
            commandAnimator.OnCenterPropertyChanged((Point)args.NewValue);
        }

        private void OnCenterPropertyChanged(Point newCenter)
        {
            SendCommand(new MoveViewportCenterInDiagramSpaceCommand(_originalGesture, newCenter));
        }
    }
}
