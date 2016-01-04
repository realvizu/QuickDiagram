using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Common;
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

        public static readonly DependencyProperty ZoomToCommandProperty =
            DependencyProperty.Register("ZoomToCommand", typeof(ICommand), typeof(DiagramViewportControl));

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
            MouseZoomCommand = new DelegateCommand(i => Zoom((ZoomCommandParameters)i, animate: true));
            KeyboardPanCommand = new DelegateCommand(i => PanInScreenSpace((Vector)i));
            KeyboardZoomCommand = new DelegateCommand(i => Zoom((ZoomCommandParameters)i, animate: false));
            ZoomToCommand = new DelegateCommand(i => ZoomTo((double)i));
            FitToViewCommand = new DelegateCommand(i => FitToView());
        }

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public double LargeZoomIncrement
        {
            get { return (double)GetValue(LargeZoomIncrementProperty); }
            set { SetValue(LargeZoomIncrementProperty, value); }
        }

        public double ExponentialZoom
        {
            get { return (double)GetValue(ExponentialZoomProperty); }
            set { SetValue(ExponentialZoomProperty, value); }
        }

        public double LinearZoom
        {
            get { return (double)GetValue(LinearZoomProperty); }
            set { SetValue(LinearZoomProperty, value); }
        }

        public double ViewportCenterX
        {
            get { return (double)GetValue(ViewportCenterXProperty); }
            set { SetValue(ViewportCenterXProperty, value); }
        }

        public double ViewportCenterY
        {
            get { return (double)GetValue(ViewportCenterYProperty); }
            set { SetValue(ViewportCenterYProperty, value); }
        }

        public Transform ViewportTransform
        {
            get { return (Transform)GetValue(ViewportTransformProperty); }
            set { SetValue(ViewportTransformProperty, value); }
        }

        public Rect DiagramContentRect
        {
            get { return (Rect)GetValue(DiagramContentRectProperty); }
            set { SetValue(DiagramContentRectProperty, value); }
        }

        public ICommand WidgetPanCommand
        {
            get { return (ICommand)GetValue(WidgetPanCommandProperty); }
            set { SetValue(WidgetPanCommandProperty, value); }
        }

        public ICommand ZoomToCommand
        {
            get { return (ICommand)GetValue(ZoomToCommandProperty); }
            set { SetValue(ZoomToCommandProperty, value); }
        }

        public ICommand FitToViewCommand
        {
            get { return (ICommand)GetValue(FitToViewCommandProperty); }
            set { SetValue(FitToViewCommandProperty, value); }
        }

        public ICommand MousePanCommand
        {
            get { return (ICommand)GetValue(MousePanCommandProperty); }
            set { SetValue(MousePanCommandProperty, value); }
        }

        public ICommand MouseZoomCommand
        {
            get { return (ICommand)GetValue(MouseZoomCommandProperty); }
            set { SetValue(MouseZoomCommandProperty, value); }
        }

        public ICommand KeyboardPanCommand
        {
            get { return (ICommand)GetValue(KeyboardPanCommandProperty); }
            set { SetValue(KeyboardPanCommandProperty, value); }
        }

        public ICommand KeyboardZoomCommand
        {
            get { return (ICommand)GetValue(KeyboardZoomCommandProperty); }
            set { SetValue(KeyboardZoomCommandProperty, value); }
        }

        private Point ViewportCenter => new Point(ViewportCenterX, ViewportCenterY);
        private Size Size => new Size(ActualWidth, ActualHeight);

        private void SetLinearViewportZoom(double linearViewportZoom)
        {
            SetProperty(LinearZoomProperty, linearViewportZoom);
        }

        private void SetViewportCenter(Point viewportCenter)
        {
            SetProperty(ViewportCenterXProperty, viewportCenter.X);
            SetProperty(ViewportCenterYProperty, viewportCenter.Y);
        }

        private void SetProperty(DependencyProperty property, double newValue)
        {
                SetValue(property, newValue);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateViewportTransform();
        }

        private void ZoomWithCenterInScreenSpace(double newLinearZoom, Point zoomCenterInScreenSpace, bool animate)
        {
            var zoomCenterInDiagramSpace = TransformToDiagramSpace(zoomCenterInScreenSpace);
            var newExponentialZoom = ScaleCalculator.LinearToExponential(newLinearZoom, MinZoom, MaxZoom);
            var newViewportCenter = (ViewportCenter - zoomCenterInDiagramSpace) * (ExponentialZoom / newExponentialZoom) + zoomCenterInDiagramSpace;
            SetViewportCenter(newViewportCenter);
            SetLinearViewportZoom(newLinearZoom);
        }

        private static void OnZoomRangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = (DiagramViewportControl)obj;
            control.LargeZoomIncrement = Math.Max(0, control.MaxZoom - control.MinZoom) * LargeZoomIncrementRate;
        }

        private static void OnLinearZoomChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((DiagramViewportControl)obj).SyncExponentialZoomValue((double)e.NewValue);
        }

        private static void OnViewportChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((DiagramViewportControl)obj).UpdateViewportTransform();
        }

        private void SyncExponentialZoomValue(double newValue)
        {
            ExponentialZoom = ScaleCalculator.LinearToExponential(newValue, MinZoom, MaxZoom);
        }

        private void UpdateViewportTransform()
        {
            Debug.WriteLine($"Size:{Size}, Zoom:{ExponentialZoom}, Center:{ViewportCenter}");
            if (double.IsNaN(ExponentialZoom))
                return;

            ViewportTransform = ViewportLogic.CalculateViewportTransform(Size, ExponentialZoom, ViewportCenter);
        }

        private Point TransformToDiagramSpace(Point pointInScreenSpace)
        {
            return ViewportTransform.Inverse.Transform(pointInScreenSpace);
        }

        private void FitToView()
        {
            var newExponentialZoom = CalculateFitToViewZoom();
            var newLinearZoom = ScaleCalculator.ExponentialToLinear(newExponentialZoom, MinZoom, MaxZoom);
            var contentCenter = DiagramContentRect.GetCenter();
            if (!double.IsNaN(newLinearZoom) && !contentCenter.IsExtreme())
            {
                SetLinearViewportZoom(newLinearZoom);
                SetViewportCenter(contentCenter);
            }
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

        private void ZoomTo(double linearZoom)
        {
            SetLinearViewportZoom(linearZoom);
        }

        private void Zoom(ZoomCommandParameters parameters, bool animate)
        {
            var newLinearZoomValue = CalculateZoom(parameters.Direction, parameters.Amount);
            ZoomWithCenterInScreenSpace(newLinearZoomValue, parameters.Center, animate);
        }

        private void PanInScreenSpace(PanDirection panDirection)
        {
            var viewportMoveVector = CalculatePanVector(panDirection);
            PanInScreenSpace(viewportMoveVector);
        }

        private void PanInScreenSpace(Vector mousePanVector)
        {
            var viewportMoveVectorInScreenSpace = mousePanVector * -1;
            var viewportMoveVectorInDiagramSpace = viewportMoveVectorInScreenSpace / ExponentialZoom;
            var newViewportCenter = ViewportCenter + viewportMoveVectorInDiagramSpace;
            SetViewportCenter(newViewportCenter);
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

        private double CalculateZoom(ZoomDirection zoomDirection, double zoomAmount)
        {
            var zoomSign = zoomDirection == ZoomDirection.In ? 1 : -1;

            var newZoom = LinearZoom + zoomAmount * zoomSign;

            if (newZoom < MinZoom)
                newZoom = MinZoom;

            if (newZoom > MaxZoom)
                newZoom = MaxZoom;

            return newZoom;
        }
    }
}
