using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.Common;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramViewportControl.xaml
    /// </summary>
    public partial class DiagramViewportControl : UserControl
    {
        private const double LargeZoomIncrementDefault = 1;
        private const double LargeZoomIncrementProportion = .1d;
        private const double PanAndZoomControlSizeDefault = 100;

        private readonly DiagramFocusTracker _diagramFocusTracker;
        private bool _isViewportObscured;

        public static readonly DependencyProperty PanAndZoomControlHeightProperty =
            DependencyProperty.RegisterAttached("PanAndZoomControlHeight", typeof(double), typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(PanAndZoomControlSizeDefault,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty MinZoomProperty =
            ZoomableVisual.MinZoomProperty.AddOwner(typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(0d, OnZoomIntervalChanged));

        public static readonly DependencyProperty MaxZoomProperty =
            ZoomableVisual.MaxZoomProperty.AddOwner(typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(0d, OnZoomIntervalChanged));

        private static void OnZoomIntervalChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
            => ((DiagramViewportControl)obj).OnZoomIntervalChanged();

        public static readonly DependencyProperty LargeZoomIncrementProperty =
            DependencyProperty.Register("LargeZoomIncrement", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(LargeZoomIncrementDefault));

        public static readonly DependencyProperty ViewportZoomProperty =
            DependencyProperty.Register("ViewportZoom", typeof(double), typeof(DiagramViewportControl));

        /// <summary>
        /// Transforms DiagramSpace to ScreenSpace. Created by the Viewport class, used for rendering the diagram.
        /// </summary>
        public static readonly DependencyProperty ViewportTransformProperty =
            DependencyProperty.Register("ViewportTransform", typeof(Transform), typeof(DiagramViewportControl),
                new PropertyMetadata(Transform.Identity));

        /// <summary>
        /// Same as ViewportTransform but also contains a hint for the animation's length.
        /// </summary>
        public static readonly DependencyProperty TransitionedViewportTransformProperty =
            DependencyProperty.Register("TransitionedViewportTransform", typeof(TransitionedTransform), typeof(DiagramViewportControl),
                new PropertyMetadata(TransitionedTransform.Identity, OnTransitionedViewportTransformChanged));

        private static void OnTransitionedViewportTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DiagramViewportControl)d).ViewportTransform = ((TransitionedTransform)e.NewValue).Transform;

        public static readonly DependencyProperty WidgetPanCommandProperty =
            DependencyProperty.Register("WidgetPanCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty WidgetZoomCommandProperty =
            DependencyProperty.Register("WidgetZoomCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty WidgetZoomToContentCommandProperty =
            DependencyProperty.Register("WidgetZoomToContentCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty MousePanCommandProperty =
            DependencyProperty.Register("MousePanCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty MouseZoomCommandProperty =
            DependencyProperty.Register("MouseZoomCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty KeyboardPanCommandProperty =
            DependencyProperty.Register("KeyboardPanCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty KeyboardZoomCommandProperty =
            DependencyProperty.Register("KeyboardZoomCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty ViewportResizeCommandProperty =
            DependencyProperty.Register("ViewportResizeCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty ViewportPanCommandProperty =
            DependencyProperty.Register("ViewportPanCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty ViewportZoomCommandProperty =
            DependencyProperty.Register("ViewportZoomCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty ViewportZoomToContentCommandProperty =
            DependencyProperty.Register("ViewportZoomToContentCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public DiagramViewportControl()
        {
            _diagramFocusTracker = new DiagramFocusTracker(this);
            _isViewportObscured = false;

            KeyboardPanCommand = new DelegateCommand<Vector>(OnKeyboardPan);
            KeyboardZoomCommand = new DelegateCommand<ZoomDirection, double, Point>(OnKeyboardZoom);
            MousePanCommand = new DelegateCommand<Vector>(OnMousePan);
            MouseZoomCommand = new DelegateCommand<ZoomDirection, double, Point>(OnMouseZoom);
            WidgetPanCommand = new DelegateCommand<Vector>(OnWidgetPan);
            WidgetZoomCommand = new DelegateCommand<double>(OnWidgetZoom);
            WidgetZoomToContentCommand = new DelegateCommand(OnWidgetZoomToContent);

            Loaded += OnLoaded;

            InitializeComponent();
        }

        private void OnKeyboardPan(Vector panVector) 
            => PanViewport(panVector, TransitionSpeed.Fast);

        private void OnKeyboardZoom(ZoomDirection direction, double amount, Point center) 
            => ZoomTo(direction, amount, center, TransitionSpeed.Fast);

        private void OnMousePan(Vector panVector) 
            => PanViewport(panVector, TransitionSpeed.Instant);

        private void OnMouseZoom(ZoomDirection direction, double amount, Point center) 
            => ZoomTo(direction, amount, center, TransitionSpeed.Fast);

        private void OnWidgetPan(Vector panVector) 
            => PanViewport(panVector, TransitionSpeed.Fast);

        private void OnWidgetZoom(double newZoom) 
            => ZoomTo(newZoom, TransitionSpeed.Fast);

        private void OnWidgetZoomToContent() 
            => ZoomToContent(TransitionSpeed.Slow);

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            OnViewportResized(sizeInfo.NewSize);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            OnViewportResized(RenderSize);
        }

        private void OnViewportResized(Size newSize)
        {
            ViewportResizeCommand?.Execute(newSize, TransitionSpeed.Instant);
        }

        private void ZoomToContent(TransitionSpeed transitionSpeed)
        {
            ViewportZoomToContentCommand?.Execute(transitionSpeed);
        }

        private void ZoomTo(double newZoom, TransitionSpeed transitionSpeed)
        {
            var zoomCenter = new Rect(RenderSize).GetCenter();
            ZoomWithCenterTo(newZoom, zoomCenter, transitionSpeed);
        }

        private void ZoomTo(ZoomDirection direction, double amount, Point center, TransitionSpeed transitionSpeed)
        {
            var newLinearZoom = CalculateModifiedZoom(ViewportZoom, direction, amount);
            ZoomWithCenterTo(newLinearZoom, center, transitionSpeed);
        }

        private void ZoomWithCenterTo(double newZoom, Point zoomCenterInScreenSpace, TransitionSpeed transitionSpeed)
        {
            if (ViewportZoom != newZoom)
                ViewportZoomCommand?.Execute(newZoom, zoomCenterInScreenSpace, transitionSpeed);
        }

        private void PanViewport(Vector panVector, TransitionSpeed transitionSpeed)
        {
            ViewportPanCommand?.Execute(panVector, transitionSpeed);
        }

        private double CalculateModifiedZoom(double currentLinearZoom, ZoomDirection zoomDirection, double zoomAmount)
        {
            var zoomSign = zoomDirection == ZoomDirection.In ? 1 : -1;

            var newLinearZoom = currentLinearZoom + zoomAmount * zoomSign;

            if (newLinearZoom < MinZoom)
                newLinearZoom = MinZoom;

            if (newLinearZoom > MaxZoom)
                newLinearZoom = MaxZoom;

            return newLinearZoom;
        }

        private void OnZoomIntervalChanged()
        {
            LargeZoomIncrement = Math.Max(0, MaxZoom - MinZoom) * LargeZoomIncrementProportion;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isViewportObscured)
                _diagramFocusTracker.TrackMouse(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            _diagramFocusTracker.Unfocus();
        }

        private void OnPanAndZoomControlMouseEnter(object sender, MouseEventArgs e)
        {
            _isViewportObscured = true;
            _diagramFocusTracker.Unfocus();
        }

        private void OnPanAndZoomControlMouseLeave(object sender, MouseEventArgs e)
        {
            _isViewportObscured = false;
        }
    }
}
