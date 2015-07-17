using System;
using System.Windows;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Smooths out the viewport manipulation movements.
    /// </summary>
    internal class AnimatedViewportGesture : Animatable, IViewportGesture
    {
        public event ViewportCommandHandler ViewportCommand;

        private readonly IViewportGesture _gesture;
        private readonly Duration _animationDuration;
        private readonly EasingFunctionBase _easingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };

        public AnimatedViewportGesture(IViewportGesture gesture, TimeSpan animationTimeSpan)
        {
            _gesture = gesture;
            _gesture.ViewportCommand += OnViewportCommand;
            _animationDuration = new Duration(animationTimeSpan);
        }

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom",
            typeof(double), typeof(AnimatedViewportGesture), new PropertyMetadata(OnZoomPropertyChanged));

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center",
            typeof(Point), typeof(AnimatedViewportGesture), new PropertyMetadata(OnMovePropertyChanged));

        public static readonly DependencyProperty ZoomWithCenterProperty = DependencyProperty.Register("ZoomWithCenter",
            typeof(ZoomWithCenterSpecification), typeof(AnimatedViewportGesture), new PropertyMetadata(OnZoomWithCenterPropertyChanged));

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size",
            typeof(Size), typeof(AnimatedViewportGesture), new PropertyMetadata(OnSizePropertyChanged));

        public IViewportHost ViewportHost
        {
            get { return _gesture.ViewportHost; }
        }

        private void OnViewportCommand(object sender, ViewportCommandBase command)
        {
            if (command is ZoomViewportWithCenterInScreenSpaceCommand)
            {
                var zoomWithCenterCommand = (ZoomViewportWithCenterInScreenSpaceCommand)command;
                var zoomWithCenterSpecification = new ZoomWithCenterSpecification(zoomWithCenterCommand.NewZoom, zoomWithCenterCommand.ZoomCenterInScreenSpace);
                AnimateZoomWithCenter(zoomWithCenterSpecification);
            }

            if (command is ZoomViewportCommand)
            {
                AnimateZoom(((ZoomViewportCommand)command).NewZoom);
            }

            if (command is MoveViewportCenterInDiagramSpaceCommand)
            {
                AnimateMoveInDiagramSpace(((MoveViewportCenterInDiagramSpaceCommand)command).NewCenterInDiagramSpace);
            }

            if (command is ResizeViewportCommand)
            {
                AnimateSize(((ResizeViewportCommand)command).NewSizeInScreenSpace);
            }
        }

        private void AnimateZoom(double newZoom)
        {
            var animation = new DoubleAnimation(_gesture.ViewportHost.ViewportZoom, newZoom, _animationDuration)
            {
                EasingFunction = _easingFunction
            };
            BeginAnimation(ZoomProperty, animation);
        }

        private void AnimateMoveInDiagramSpace(Point newCenter)
        {
            var animation = new PointAnimation(_gesture.ViewportHost.ViewportInDiagramSpace.GetCenter(), newCenter, _animationDuration)
            {
                EasingFunction = _easingFunction
            };
            BeginAnimation(CenterProperty, animation);
        }

        private void AnimateZoomWithCenter(ZoomWithCenterSpecification newZoomWithCenterSpecification)
        {
            var originalZoomWithCenterSpecification = new ZoomWithCenterSpecification(
                _gesture.ViewportHost.ViewportZoom,
                newZoomWithCenterSpecification.CenterInScreenSpace);

            var animation = new ZoomWithCenterSpecificationAnimation(
                originalZoomWithCenterSpecification, newZoomWithCenterSpecification, _animationDuration, _easingFunction);

            BeginAnimation(ZoomWithCenterProperty, animation);
        }

        private void AnimateSize(Size newSize)
        {
            var animation = new SizeAnimation(_gesture.ViewportHost.ViewportInScreenSpace.Size, newSize, _animationDuration)
            {
                EasingFunction = _easingFunction
            };
            BeginAnimation(SizeProperty, animation);
        }

        private static void OnZoomPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var animatedGesture = obj as AnimatedViewportGesture;
            animatedGesture.OnZoomChanged((double)args.NewValue);
        }

        private static void OnMovePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var animatedGesture = obj as AnimatedViewportGesture;
            animatedGesture.OnMoveChanged((Point)args.NewValue);
        }

        private static void OnZoomWithCenterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var animatedGesture = obj as AnimatedViewportGesture;
            animatedGesture.OnZoomWithCenterChanged((ZoomWithCenterSpecification)args.NewValue);
        }

        private static void OnSizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var animatedGesture = obj as AnimatedViewportGesture;
            animatedGesture.OnSizeChanged((Size)args.NewValue);
        }

        private void OnZoomChanged(double newZoom)
        {
            ZoomViewportTo(newZoom);
        }

        private void OnMoveChanged(Point newCenter)
        {
            MoveViewportTo(newCenter);
        }

        private void OnZoomWithCenterChanged(ZoomWithCenterSpecification zoomWithCenterSpecification)
        {
            ZoomViewportWithCenter(zoomWithCenterSpecification);
        }

        private void OnSizeChanged(Size newSize)
        {
            ResizeViewportTo(newSize);
        }

        protected void ZoomViewportTo(double newZoom)
        {
            SendCommand(new ZoomViewportCommand(newZoom));
        }

        protected void MoveViewportTo(Point newCenter)
        {
            SendCommand(new MoveViewportCenterInDiagramSpaceCommand(newCenter));
        }

        protected void ZoomViewportWithCenter(ZoomWithCenterSpecification zoomWithCenterSpecification)
        {
            SendCommand(new ZoomViewportWithCenterInScreenSpaceCommand(zoomWithCenterSpecification.Zoom, zoomWithCenterSpecification.CenterInScreenSpace));
        }

        protected void ResizeViewportTo(Size newSize)
        {
            SendCommand(new ResizeViewportCommand(newSize));
        }

        private void SendCommand(ViewportCommandBase command)
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
