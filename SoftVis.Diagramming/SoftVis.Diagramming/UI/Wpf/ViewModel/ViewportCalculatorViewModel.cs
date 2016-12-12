using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.Transforms;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Calculates the properties of the viewport.
    /// Stateful, publishes events when calculated properties change.
    /// </summary>
    /// <remarks>
    /// Internally the zoom values change on an exponential scale to give a feel of depth.
    /// But externally the zoom values are converted to/from a linear scale for easy linear manipulation.
    /// </remarks>
    public class ViewportCalculatorViewModel : DiagramViewModelBase
    {
        private static readonly Size ViewportSizeDefault = new Size(0, 0);
        private static readonly Point ViewportCenterDefault = new Point(0, 0);

        private readonly double _minZoom;
        private readonly double _maxZoom;
        private readonly double _defaultExponentialZoom;
        private double _exponentialZoom;
        private Size _sizeInScreenSpace;
        private Point _centerInDiagramSpace;
        private Transform _diagramSpaceToScreenSpaceTransform;
        private double _linearZoom;
        private TransitionedTransform _transform;

        public ResizeDelegateCommand ResizeCommand { get; }
        public PanDelegateCommand PanCommand { get; }
        public ZoomToContentDelegateCommand ZoomToContentCommand { get; }
        public ZoomDelegateCommand ZoomCommand { get; }

        public event Action<TransitionedTransform> TransformChanged;

        public ViewportCalculatorViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            : this(diagram, minZoom, maxZoom, initialZoom, ViewportSizeDefault, ViewportCenterDefault)
        {
        }

        private ViewportCalculatorViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom,
            Size sizeInScreenSpace, Point centerInDiagramSpace)
            :base(diagram)
        {
            _minZoom = minZoom;
            _maxZoom = maxZoom;
            _defaultExponentialZoom = initialZoom;
            _exponentialZoom = initialZoom;
            _sizeInScreenSpace = sizeInScreenSpace;
            _centerInDiagramSpace = centerInDiagramSpace;

            ResizeCommand = new ResizeDelegateCommand(Resize);
            PanCommand = new PanDelegateCommand(Pan);
            ZoomToContentCommand = new ZoomToContentDelegateCommand(ZoomToContent);
            ZoomCommand = new ZoomDelegateCommand(ZoomWithCenterTo);

            UpdateCalculatedProperties(TransitionSpeed.Instant);
        }

        public double LinearZoom
        {
            get { return _linearZoom; }
            set
            {
                _linearZoom = value;
                OnPropertyChanged();
            }
        }

        public TransitionedTransform Transform
        {
            get { return _transform; }
            set
            {
                _transform = value;
                OnPropertyChanged();
            }
        }

        public virtual void Resize(Size sizeInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Instant)
        {
            _sizeInScreenSpace = sizeInScreenSpace;
            UpdateCalculatedProperties(transitionSpeed);
        }

        public virtual void Pan(Vector panVectorInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Fast)
        {
            var viewportMoveVectorInScreenSpace = panVectorInScreenSpace * -1;
            var viewportMoveVectorInDiagramSpace = viewportMoveVectorInScreenSpace / _exponentialZoom;

            _centerInDiagramSpace += viewportMoveVectorInDiagramSpace;
            UpdateCalculatedProperties(transitionSpeed);
        }

        public virtual void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Medium)
        {
            ZoomToRect(Diagram.ContentRect.ToWpf(), transitionSpeed);
        }

        public virtual void ZoomToRect(Rect rect, TransitionSpeed transitionSpeed)
        {
            if (rect.IsUndefined())
            {
#if DEBUG
                Debugger.Break();
#else
                return;
#endif
            }

            _exponentialZoom = CalculateZoomForContent(rect.Size);
            _centerInDiagramSpace = rect.GetCenter();
            UpdateCalculatedProperties(transitionSpeed);
        }

        public virtual void ZoomWithCenterTo(double newLinearZoom, Point zoomCenterInScreenSpace,
            TransitionSpeed transitionSpeed = TransitionSpeed.Fast)
        {
            var oldExponentialZoom = _exponentialZoom;
            var newExponentialZoom = ToExponentialZoom(newLinearZoom);
            var relativeZoom = oldExponentialZoom / newExponentialZoom;
            var zoomCenterInDiagramSpace = ProjectFromScreenSpaceToDiagramSpace(zoomCenterInScreenSpace);

            _centerInDiagramSpace = (_centerInDiagramSpace - zoomCenterInDiagramSpace) * relativeZoom + zoomCenterInDiagramSpace;
            _exponentialZoom = newExponentialZoom;
            UpdateCalculatedProperties(transitionSpeed);
        }

        public bool IsDiagramRectVisible()
        {
            var diagramRect = Diagram.ContentRect.ToWpf();
            var viewportRect = ProjectViewportIntoDiagramSpace();
            return viewportRect.IntersectsWith(diagramRect);
        }

        private Point ProjectFromDiagramSpaceToScreenSpace(Point pointInDiagramSpace)
        {
            return _diagramSpaceToScreenSpaceTransform.Transform(pointInDiagramSpace);
        }

        private Point ProjectFromScreenSpaceToDiagramSpace(Point pointInScreenSpace)
        {
            return _diagramSpaceToScreenSpaceTransform.Inverse?.Transform(pointInScreenSpace) ?? PointExtensions.Undefined;
        }

        private void UpdateCalculatedProperties(TransitionSpeed transitionSpeed)
        {
            UpdateLinearZoom();
            UpdateTransitionedTransform(transitionSpeed);
        }

        private void UpdateLinearZoom()
        {
            LinearZoom = ToLinearZoom(_exponentialZoom);
        }

        private void UpdateTransitionedTransform(TransitionSpeed transitionSpeed)
        {
            _diagramSpaceToScreenSpaceTransform = CreateTransformToScreenSpace();
            Transform = new TransitionedTransform(_diagramSpaceToScreenSpaceTransform, transitionSpeed);
            TransformChanged?.Invoke(Transform);
        }

        private double ToExponentialZoom(double linearZoom)
        {
            return ScaleCalculator.LinearToExponential(linearZoom, _minZoom, _maxZoom);
        }

        private double ToLinearZoom(double exponentialZoom)
        {
            return ScaleCalculator.ExponentialToLinear(exponentialZoom, _minZoom, _maxZoom);
        }

        private double CalculateZoomForContent(Size contentSize)
        {
            return new[]
            {
                _defaultExponentialZoom,
                _sizeInScreenSpace.Width / contentSize.Width,
                _sizeInScreenSpace.Height / contentSize.Height
            }.Min();
        }

        private Transform CreateTransformToScreenSpace()
        {
            var viewportInDiagramSpace = ProjectViewportIntoDiagramSpace();
            var diagramSpaceToScreenSpace = GetDiagramSpaceToScreenSpaceTransform(viewportInDiagramSpace);
            return diagramSpaceToScreenSpace;
        }

        private Rect ProjectViewportIntoDiagramSpace()
        {
            if (_sizeInScreenSpace.IsEmpty)
                return new Rect(Size.Empty);

            var projectedSize = new Size(_sizeInScreenSpace.Width / _exponentialZoom, _sizeInScreenSpace.Height / _exponentialZoom);
            var projectedTopLeft = new Point(_centerInDiagramSpace.X - projectedSize.Width / 2, _centerInDiagramSpace.Y - projectedSize.Height / 2);
            return new Rect(projectedTopLeft, projectedSize);
        }

        private Transform GetDiagramSpaceToScreenSpaceTransform(Rect viewportInDiagramSpace)
        {
            var translateVector = (Vector)viewportInDiagramSpace.TopLeft * -1;

            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(translateVector.X, translateVector.Y));
            transform.Children.Add(new ScaleTransform(_exponentialZoom, _exponentialZoom));
            return transform;
        }

        public class PanDelegateCommand : DelegateCommand<Vector, TransitionSpeed>
        {
            public PanDelegateCommand(Action<Vector, TransitionSpeed> action) : base(action)
            { }

            public void Execute(Vector panVector, TransitionSpeed transitionSpeed)
                => ExecuteCore(panVector, transitionSpeed);
        }

        public class ResizeDelegateCommand : DelegateCommand<Size, TransitionSpeed>
        {
            public ResizeDelegateCommand(Action<Size, TransitionSpeed> action) : base(action)
            { }

            public void Execute(Size newSize, TransitionSpeed transitionSpeed)
                => ExecuteCore(newSize, transitionSpeed);
        }

        public class ZoomDelegateCommand : DelegateCommand<double, Point, TransitionSpeed>
        {
            public ZoomDelegateCommand(Action<double, Point, TransitionSpeed> action) : base(action)
            { }

            public void Execute(double zoomValue, Point zoomCenter, TransitionSpeed transitionSpeed)
                => ExecuteCore(zoomValue, zoomCenter, transitionSpeed);
        }

        public class ZoomToContentDelegateCommand : DelegateCommand<TransitionSpeed>
        {
            public ZoomToContentDelegateCommand(Action<TransitionSpeed> action) : base(action)
            { }
        }
    }
}
