using System.Windows;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures.Animated
{
    internal class ResizeViewportCommandAnimator : ViewportCommandAnimatorBase
    {
        private readonly ResizeViewportCommand _originalCommand;

        public static readonly DependencyProperty SizeProperty = 
            DependencyProperty.Register("Size", typeof(Size), typeof(ResizeViewportCommandAnimator), 
                new PropertyMetadata(OnSizePropertyChanged));

        internal ResizeViewportCommandAnimator(IViewportGesture originalGesture, Duration animationDuration,
            ResizeViewportCommand originalCommand)
            : base(originalGesture, animationDuration)
        {
            _originalCommand = originalCommand;
        }

        public override void BeginAnimation()
        {
            var animation = new SizeAnimation(OriginalGesture.DiagramViewport.ViewportInScreenSpace.Size, 
                _originalCommand.NewSizeInScreenSpace, AnimationDuration)
            {
                EasingFunction = EasingFunction
            };
            BeginAnimation(SizeProperty, animation);
        }

        private static void OnSizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var commandAnimator = (ResizeViewportCommandAnimator)obj;
            commandAnimator.OnSizePropertyChanged((Size)args.NewValue);
        }

        private void OnSizePropertyChanged(Size newSize)
        {
            SendCommand(new ResizeViewportCommand(OriginalGesture, newSize));
        }
    }
}
