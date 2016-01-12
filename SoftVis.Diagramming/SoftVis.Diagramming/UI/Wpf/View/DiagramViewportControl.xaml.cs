using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramViewportControl.xaml
    /// </summary>
    public partial class DiagramViewportControl : UserControl
    {
        private const double PanAmount = 50d;
        private const double LargeZoomIncrementRate = .1d;

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(OnZoomRangeChanged));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramViewportControl),
                new PropertyMetadata(OnZoomRangeChanged));

        public static readonly DependencyProperty LargeZoomIncrementProperty =
            DependencyProperty.Register("LargeZoomIncrement", typeof(double), typeof(DiagramViewportControl));

        /// <summary>
        /// The zoom value changes on an exponential scale to give a feeling of depth.
        /// </summary>
        public static readonly DependencyProperty ExponentialZoomProperty =
            DependencyProperty.Register("ExponentialZoom", typeof(double), typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(double.NaN, OnViewportChanged));

        /// <summary>
        /// The zoom value on a linear scale. This is used for the slider value and zoom animation.
        /// This is converted to an exponential scale for the viewport zoom value.
        /// </summary>
        public static readonly DependencyProperty LinearZoomProperty =
            DependencyProperty.Register("LinearZoom", typeof(double), typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(OnLinearZoomChanged));

        public static readonly DependencyProperty ViewportCenterXProperty =
            DependencyProperty.Register("ViewportCenterX", typeof(double), typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(0d, OnViewportChanged));

        public static readonly DependencyProperty ViewportCenterYProperty =
            DependencyProperty.Register("ViewportCenterY", typeof(double), typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(0d, OnViewportChanged));

        public static readonly DependencyProperty ViewportTransformProperty =
            DependencyProperty.Register("ViewportTransform", typeof(Transform), typeof(DiagramViewportControl),
                new FrameworkPropertyMetadata(Transform.Identity));

        public static readonly DependencyProperty DiagramContentRectProperty =
            DependencyProperty.Register("DiagramContentRect", typeof(Rect), typeof(DiagramViewportControl));

        public static readonly DependencyProperty WidgetPanCommandProperty =
            DependencyProperty.Register("WidgetPanCommand", typeof(ICommand), typeof(DiagramViewportControl));

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

        public DiagramViewportControl()
        {
            InitializeComponent();

            WidgetPanCommand = new DelegateCommand(i => PanInScreenSpace((PanDirection)i));
            MousePanCommand = new DelegateCommand(i => PanInScreenSpace((Vector)i));
            MouseZoomCommand = new DelegateCommand(i => Zoom((ZoomCommandParameters)i));
            KeyboardPanCommand = new DelegateCommand(i => PanInScreenSpace((Vector)i));
            KeyboardZoomCommand = new DelegateCommand(i => Zoom((ZoomCommandParameters)i));
            FitToViewCommand = new DelegateCommand(i => FitToView());
        }

        private Point ViewportCenter => new Point(ViewportCenterX, ViewportCenterY);
        private Size Size => new Size(ActualWidth, ActualHeight);

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateViewportTransform();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Keyboard.Focus(DiagramItemsControl);
        }

        private static void OnLinearZoomChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((DiagramViewportControl)obj).SyncExponentialZoomValue((double)e.NewValue);
        }

        private static void OnZoomRangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((DiagramViewportControl)obj).UpdateLargeZoomIncrement();
        }

        private static void OnViewportChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((DiagramViewportControl)obj).UpdateViewportTransform();
        }

        private void FitToView()
        {
            var newExponentialZoom = CalculateFitToViewZoom();
            var newLinearZoom = ScaleCalculator.ExponentialToLinear(newExponentialZoom, MinZoom, MaxZoom);
            var contentCenter = DiagramContentRect.GetCenter();
            SetLinearViewportZoom(newLinearZoom);
            SetViewportCenter(contentCenter);
        }

        private void Zoom(ZoomCommandParameters zoomCommand)
        {
            var newLinearZoom = CalculateLinearZoom(zoomCommand.Direction, zoomCommand.Amount);
            ZoomWithCenterInScreenSpace(newLinearZoom, zoomCommand.Center);
        }

        private void PanInScreenSpace(PanDirection panDirection)
        {
            var panVector = CalculatePanVector(panDirection);
            PanInScreenSpace(panVector);
        }

        private void PanInScreenSpace(Vector panVector)
        {
            var viewportMoveVectorInScreenSpace = panVector * -1;
            var viewportMoveVectorInDiagramSpace = viewportMoveVectorInScreenSpace / ExponentialZoom;
            var newViewportCenter = ViewportCenter + viewportMoveVectorInDiagramSpace;
            SetViewportCenter(newViewportCenter);
        }

        private void ZoomWithCenterInScreenSpace(double newLinearZoom, Point zoomCenterInScreenSpace)
        {
            var zoomCenterInDiagramSpace = TransformToDiagramSpace(zoomCenterInScreenSpace);
            var newExponentialZoom = ScaleCalculator.LinearToExponential(newLinearZoom, MinZoom, MaxZoom);
            var newViewportCenter = (ViewportCenter - zoomCenterInDiagramSpace) * (ExponentialZoom / newExponentialZoom) + zoomCenterInDiagramSpace;
            SetLinearViewportZoom(newLinearZoom);
            SetViewportCenter(newViewportCenter);
        }

        private Point TransformToDiagramSpace(Point pointInScreenSpace)
        {
            return ViewportTransform.Inverse.Transform(pointInScreenSpace);
        }

        private double CalculateFitToViewZoom()
        {
            return new[]
            {
                1.0,
                Width / DiagramContentRect.Width,
                Height / DiagramContentRect.Height
            }.Min();
        }

        private static Vector CalculatePanVector(PanDirection panDirection)
        {
            Vector vector;
            switch (panDirection)
            {
                case PanDirection.Up:
                    vector = new Vector(0, -PanAmount);
                    break;
                case PanDirection.Down:
                    vector = new Vector(0, PanAmount);
                    break;
                case PanDirection.Left:
                    vector = new Vector(-PanAmount, 0);
                    break;
                case PanDirection.Right:
                    vector = new Vector(PanAmount, 0);
                    break;
                default:
                    throw new Exception($"Unexpected PanDirection: {panDirection}");
            }
            return vector;
        }

        private double CalculateLinearZoom(ZoomDirection zoomDirection, double zoomAmount)
        {
            var zoomSign = zoomDirection == ZoomDirection.In ? 1 : -1;

            var newZoom = LinearZoom + zoomAmount * zoomSign;

            if (newZoom < MinZoom)
                newZoom = MinZoom;

            if (newZoom > MaxZoom)
                newZoom = MaxZoom;

            return newZoom;
        }

        private void UpdateLargeZoomIncrement()
        {
            LargeZoomIncrement = Math.Max(0, MaxZoom - MinZoom) * LargeZoomIncrementRate;
        }

        private void UpdateViewportTransform()
        {
            Debug.WriteLine($"Size:{Size}, Zoom:{ExponentialZoom}, Center:{ViewportCenter}");
            if (double.IsNaN(ExponentialZoom))
                return;

            ViewportTransform = ViewportLogic.CalculateViewportTransform(Size, ExponentialZoom, ViewportCenter);
        }

        private void SyncExponentialZoomValue(double newValue)
        {
            ExponentialZoom = ScaleCalculator.LinearToExponential(newValue, MinZoom, MaxZoom);
        }

        private void SetLinearViewportZoom(double linearViewportZoom)
        {
            if (double.IsNaN(linearViewportZoom))
                return;

            LinearZoom = linearViewportZoom;
        }

        private void SetViewportCenter(Point viewportCenter)
        {
            if (viewportCenter.IsExtreme())
                return;

            ViewportCenterX = viewportCenter.X;
            ViewportCenterY = viewportCenter.Y;
        }
    }
}
