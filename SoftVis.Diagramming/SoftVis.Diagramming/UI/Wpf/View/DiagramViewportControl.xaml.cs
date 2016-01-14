using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Animations;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramViewportControl.xaml
    /// </summary>
    public partial class DiagramViewportControl : UserControl
    {
        private static readonly Point ViewportCenterDefault = new Point(0, 0);
        private static readonly Size ViewportSizeDefault = new Size(0, 0);
        private const double LinearViewportZoomDefault = 1;
        private const double MinZoomDefault = 0.1;
        private const double MaxZoomDefault = 10;
        private const double LargeZoomIncrementDefault = 1;
        private const double LargeZoomIncrementProportion = .1d;

        private readonly Viewport _viewport = new Viewport(ViewportSizeDefault, 
            ViewportCenterDefault, LinearViewportZoomDefault, MinZoomDefault, MaxZoomDefault);

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(MinZoomDefault, OnZoomRangeChanged));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(MaxZoomDefault, OnZoomRangeChanged));

        public static readonly DependencyProperty InitialZoomProperty =
            DependencyProperty.Register("InitialZoom", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(LinearViewportZoomDefault, OnInitialZoomChanged));

        public static readonly DependencyProperty LargeZoomIncrementProperty =
            DependencyProperty.Register("LargeZoomIncrement", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(LargeZoomIncrementDefault));

        /// <summary>
        /// The zoom value that is manipulated by the different zoom gestures/widgets on a linear scale.
        /// This is converted to an exponential scale by the Viewport class.
        /// </summary>
        public static readonly DependencyProperty LinearViewportZoomProperty =
            DependencyProperty.Register("LinearViewportZoom", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(LinearViewportZoomDefault));

        /// <summary>
        /// The center of the viewport in DiagramSpace.
        /// </summary>
        public static readonly DependencyProperty ViewportCenterXProperty =
            DependencyProperty.Register("ViewportCenterX", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(ViewportCenterDefault.X));

        public static readonly DependencyProperty ViewportCenterYProperty =
            DependencyProperty.Register("ViewportCenterY", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(ViewportCenterDefault.Y));

        /// <summary>
        /// Transforms DiagramSpace to ScreenSpace. Also contains a hint for the animation's length.
        /// Created by the Viewport class, used for rendering the diagram.
        /// </summary>
        public static readonly DependencyProperty ViewportTransformProperty =
            DependencyProperty.Register("ViewportTransform", typeof(HintedTransform), typeof(DiagramViewportControl),
                new PropertyMetadata(HintedTransform.Identity));

        public static readonly DependencyProperty DiagramContentRectProperty =
            DependencyProperty.Register("DiagramContentRect", typeof(Rect), typeof(DiagramViewportControl));

        public static readonly DependencyProperty WidgetPanCommandProperty =
            DependencyProperty.Register("WidgetPanCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty WidgetZoomCommandProperty =
            DependencyProperty.Register("WidgetZoomCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty FitToViewCommandProperty =
            DependencyProperty.Register("FitToViewCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty MousePanCommandProperty =
            DependencyProperty.Register("MousePanCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty MouseZoomCommandProperty =
            DependencyProperty.Register("MouseZoomCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty KeyboardPanCommandProperty =
            DependencyProperty.Register("KeyboardPanCommand", typeof(ICommand), typeof(DiagramViewportControl));

        public static readonly DependencyProperty KeyboardZoomCommandProperty =
            DependencyProperty.Register("KeyboardZoomCommand", typeof(ICommand), typeof(DiagramViewportControl));

        private static void OnZoomRangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
            => ((DiagramViewportControl)obj).OnZoomRangeChanged();

        private static void OnInitialZoomChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
            => ((DiagramViewportControl)obj).OnInitialZoomChanged();

        public DiagramViewportControl()
        {
            InitializeComponent();

            KeyboardPanCommand = new DelegateCommand(i => PanInScreenSpace((Vector)i, AnimationHint.Short));
            KeyboardZoomCommand = new DelegateCommand(i => Zoom((ZoomCommandParameters) i, AnimationHint.Short));
            MousePanCommand = new DelegateCommand(i => PanInScreenSpace((Vector)i, AnimationHint.None));
            MouseZoomCommand = new DelegateCommand(i => Zoom((ZoomCommandParameters)i, AnimationHint.Short));
            WidgetPanCommand = new DelegateCommand(i => PanInScreenSpace((Vector)i, AnimationHint.Short));
            WidgetZoomCommand = new DelegateCommand(i => ZoomTo((double)i, AnimationHint.Short));
            FitToViewCommand = new DelegateCommand(i => ZoomToContent(AnimationHint.Long));
        }

        private Point ViewportCenter
        {
            get { return new Point(ViewportCenterX, ViewportCenterY); }
            set
            {
                ViewportCenterX = value.X;
                ViewportCenterY = value.Y;
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Keyboard.Focus(DiagramItemsControl);
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            Keyboard.Focus(DiagramItemsControl);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            _viewport.Resize(sizeInfo.NewSize);
            ViewportTransform = new HintedTransform(_viewport.DiagramSpaceToScreenSpace, AnimationHint.None);
        }

        private void OnZoomRangeChanged()
        {
            LargeZoomIncrement = Math.Max(0, MaxZoom - MinZoom) * LargeZoomIncrementProportion;
            _viewport.UpdateZoomRange(MinZoom, MaxZoom);
            ViewportTransform = new HintedTransform(_viewport.DiagramSpaceToScreenSpace, AnimationHint.None);
        }

        private void OnInitialZoomChanged()
        {
            _viewport.UpdateDefaultZoom(InitialZoom);
            LinearViewportZoom = InitialZoom;
            ViewportTransform = new HintedTransform(_viewport.DiagramSpaceToScreenSpace, AnimationHint.None);
        }

        private void ZoomToContent(AnimationHint animationHint)
        {
            _viewport.ZoomToContent(DiagramContentRect);
            LinearViewportZoom = _viewport.LinearZoom;
            ViewportCenter = _viewport.CenterInDiagramSpace;
            ViewportTransform = new HintedTransform(_viewport.DiagramSpaceToScreenSpace, animationHint);
        }

        private void ZoomTo(double newZoom, AnimationHint animationHint)
        {
            if (LinearViewportZoom != newZoom)
            {
                _viewport.ZoomTo(newZoom);
                LinearViewportZoom = newZoom;
                ViewportTransform = new HintedTransform(_viewport.DiagramSpaceToScreenSpace, animationHint);
            }
        }

        private void Zoom(ZoomCommandParameters zoomCommand, AnimationHint animationHint)
        {
            var newLinearZoom = CalculateModifiedZoom(LinearViewportZoom, zoomCommand.Direction, zoomCommand.Amount);
            ZoomWithCenterInScreenSpace(newLinearZoom, zoomCommand.Center, animationHint);
        }

        private void ZoomWithCenterInScreenSpace(double newLinearZoom, Point zoomCenterInScreenSpace,
            AnimationHint animationHint)
        {
            _viewport.ZoomWithCenterTo(newLinearZoom, zoomCenterInScreenSpace);
            LinearViewportZoom = _viewport.LinearZoom;
            ViewportCenter = _viewport.CenterInDiagramSpace;
            ViewportTransform = new HintedTransform(_viewport.DiagramSpaceToScreenSpace, animationHint);
        }

        private void PanInScreenSpace(Vector panVector, AnimationHint animationHint)
        {
            _viewport.Pan(panVector);
            ViewportCenter = _viewport.CenterInDiagramSpace;
            ViewportTransform = new HintedTransform(_viewport.DiagramSpaceToScreenSpace, animationHint);
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
    }
}
