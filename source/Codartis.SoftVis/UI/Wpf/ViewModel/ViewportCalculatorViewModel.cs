using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;
using Codartis.Util.UI.Wpf.Commands;
using Codartis.Util.UI.Wpf.Transforms;

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
    public class ViewportCalculatorViewModel : ModelObserverViewModelBase
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
        private Rect _diagramContentRect;

        public ResizeDelegateCommand ResizeCommand { get; }
        public PanDelegateCommand PanCommand { get; }
        public ZoomToContentDelegateCommand ZoomToContentCommand { get; }
        public ZoomDelegateCommand ZoomCommand { get; }

        public event Action<TransitionedTransform> TransformChanged;

        public ViewportCalculatorViewModel(
            IModelService modelService,
            IDiagramService diagramService,
            double minZoom,
            double maxZoom,
            double initialZoom)
            : this(modelService, diagramService, minZoom, maxZoom, initialZoom, ViewportSizeDefault, ViewportCenterDefault)
        {
        }

        private ViewportCalculatorViewModel(
            IModelService modelService,
            IDiagramService diagramService,
            double minZoom,
            double maxZoom,
            double initialZoom,
            Size sizeInScreenSpace,
            Point centerInDiagramSpace)
            : base(modelService, diagramService)
        {
            _minZoom = minZoom;
            _maxZoom = maxZoom;
            _defaultExponentialZoom = initialZoom;
            _exponentialZoom = initialZoom;
            _sizeInScreenSpace = sizeInScreenSpace;
            _centerInDiagramSpace = centerInDiagramSpace;
            _diagramContentRect = diagramService.LatestDiagram.Rect.ToWpf();

            ResizeCommand = new ResizeDelegateCommand(Resize);
            PanCommand = new PanDelegateCommand(Pan);
            ZoomToContentCommand = new ZoomToContentDelegateCommand(ZoomToContent);
            ZoomCommand = new ZoomDelegateCommand(ZoomWithCenterTo);

            UpdateCalculatedProperties(TransitionSpeed.Instant);

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            DiagramService.DiagramChanged -= OnDiagramChanged;
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
            ZoomToRect(_diagramContentRect, transitionSpeed);
        }

        public virtual void ZoomToRect(Rect rect, TransitionSpeed transitionSpeed)
        {
            if (rect.IsUndefined())
                return;

            _exponentialZoom = CalculateMinZoomToContainRect(rect.Size, _defaultExponentialZoom);
            _centerInDiagramSpace = rect.GetCenter();
            UpdateCalculatedProperties(transitionSpeed);
        }

        public virtual void ZoomWithCenterTo(
            double newLinearZoom,
            Point zoomCenterInScreenSpace,
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

        public virtual void ContainRect(Rect rect, TransitionSpeed transitionSpeed)
        {
            if (rect.IsUndefined() || rect.IsZero())
                return;

            _exponentialZoom = CalculateMinZoomToContainRect(rect.Size, _exponentialZoom);

            var viewportInDiagramSpace = ProjectViewportIntoDiagramSpace();
            var vector = CalculateVectorToCoverRect(rect, viewportInDiagramSpace);
            _centerInDiagramSpace = _centerInDiagramSpace + vector;

            UpdateCalculatedProperties(transitionSpeed);
        }

        public bool IsDiagramRectVisible()
        {
            var viewportRect = ProjectViewportIntoDiagramSpace();
            return viewportRect.IntersectsWith(_diagramContentRect);
        }

        private void OnDiagramChanged(DiagramEvent @event)
        {
            _diagramContentRect = @event.NewDiagram.Rect.ToWpf();
        }

        // ReSharper disable once UnusedMember.Local
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

        private double CalculateMinZoomToContainRect(Size contentSize, double maxZoom)
        {
            if (contentSize.IsZero())
                return maxZoom;

            return new[]
            {
                maxZoom,
                _sizeInScreenSpace.Width / contentSize.Width,
                _sizeInScreenSpace.Height / contentSize.Height
            }.Min();
        }

        private static Vector CalculateVectorToCoverRect(Rect rectToCover, Rect coveringRect)
        {
            double xOffset = 0;
            double yOffset = 0;

            if (rectToCover.Left < coveringRect.Left)
                xOffset = rectToCover.Left - coveringRect.Left;
            else if (rectToCover.Right > coveringRect.Right)
                xOffset = rectToCover.Right - coveringRect.Right;

            if (rectToCover.Top < coveringRect.Top)
                yOffset = rectToCover.Top - coveringRect.Top;
            else if (rectToCover.Bottom > coveringRect.Bottom)
                yOffset = rectToCover.Bottom - coveringRect.Bottom;

            return new Vector(xOffset, yOffset);
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
            public PanDelegateCommand(Action<Vector, TransitionSpeed> action)
                : base(action)
            {
            }

            public void Execute(Vector panVector, TransitionSpeed transitionSpeed) => ExecuteCore(panVector, transitionSpeed);
        }

        public class ResizeDelegateCommand : DelegateCommand<Size, TransitionSpeed>
        {
            public ResizeDelegateCommand(Action<Size, TransitionSpeed> action)
                : base(action)
            {
            }

            public void Execute(Size newSize, TransitionSpeed transitionSpeed) => ExecuteCore(newSize, transitionSpeed);
        }

        public class ZoomDelegateCommand : DelegateCommand<double, Point, TransitionSpeed>
        {
            public ZoomDelegateCommand(Action<double, Point, TransitionSpeed> action)
                : base(action)
            {
            }

            public void Execute(double zoomValue, Point zoomCenter, TransitionSpeed transitionSpeed) => ExecuteCore(zoomValue, zoomCenter, transitionSpeed);
        }

        public class ZoomToContentDelegateCommand : DelegateCommand<TransitionSpeed>
        {
            public ZoomToContentDelegateCommand(Action<TransitionSpeed> action)
                : base(action)
            {
            }
        }
    }
}