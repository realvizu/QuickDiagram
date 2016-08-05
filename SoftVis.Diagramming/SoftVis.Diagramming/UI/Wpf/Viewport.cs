using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Calculates the properties of the viewport.
    /// Stateful, publishes events when calculated properties change.
    /// </summary>
    /// <remarks>
    /// Internally the zoom values change on an exponential scale to give a feel of depth.
    /// But externally the zoom values are converted to/from a linear scale for easy linear manipulation.
    /// </remarks>
    public class Viewport
    {
        private const double MinZoomDefault = 0.1;
        private const double MaxZoomDefault = 10;
        private const double InitialZoomDefault = 1;
        private static readonly Size ViewportSizeDefault = new Size(0, 0);
        private static readonly Point ViewportCenterDefault = new Point(0, 0);

        private readonly IDiagram _diagram;
        private readonly double _minZoom;
        private readonly double _maxZoom;
        private readonly double _defaultExponentialZoom;
        private double _exponentialZoom;
        private Size _sizeInScreenSpace;
        private Point _centerInDiagramSpace;
        private Rect _contentRect;

        private Transform _diagramSpaceToScreenSpaceTransform;

        public event Action<double> LinearZoomChanged;
        public event Action<TransitionedTransform> TransformChanged;

        public Viewport(IDiagram diagram, double minZoom = MinZoomDefault, double maxZoom = MaxZoomDefault, double initialZoom = InitialZoomDefault)
            : this(diagram, minZoom, maxZoom, initialZoom, ViewportSizeDefault, ViewportCenterDefault)
        {
        }

        private Viewport(IDiagram diagram, double minZoom, double maxZoom, double initialZoom,
            Size sizeInScreenSpace, Point centerInDiagramSpace)
        {
            _diagram = diagram;
            _minZoom = minZoom;
            _maxZoom = maxZoom;
            _defaultExponentialZoom = initialZoom;
            _exponentialZoom = initialZoom;
            _sizeInScreenSpace = sizeInScreenSpace;
            _centerInDiagramSpace = centerInDiagramSpace;
            _contentRect = diagram.ContentRect.ToWpf();

            UpdateCalculatedProperties(TransitionSpeed.Instant);
            SubscribeToDiagramEvents();
        }

        public void Resize(Size sizeInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Instant)
        {
            _sizeInScreenSpace = sizeInScreenSpace;
            UpdateCalculatedProperties(transitionSpeed);
        }

        public void Pan(Vector panVectorInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Fast)
        {
            var viewportMoveVectorInScreenSpace = panVectorInScreenSpace * -1;
            var viewportMoveVectorInDiagramSpace = viewportMoveVectorInScreenSpace / _exponentialZoom;

            _centerInDiagramSpace += viewportMoveVectorInDiagramSpace;
            UpdateCalculatedProperties(transitionSpeed);
        }

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
        {
            _exponentialZoom = CalculateZoomForContent(_contentRect.Size);
            _centerInDiagramSpace = _contentRect.GetCenter();
            UpdateCalculatedProperties(transitionSpeed);
        }

        public void ZoomWithCenterTo(double newLinearZoom, Point zoomCenterInScreenSpace,
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

        public Point ProjectFromDiagramSpaceToScreenSpace(Point pointInDiagramSpace)
        {
            return _diagramSpaceToScreenSpaceTransform.Transform(pointInDiagramSpace);
        }

        public Point ProjectFromScreenSpaceToDiagramSpace(Point pointInScreenSpace)
        {
            return _diagramSpaceToScreenSpaceTransform.Inverse?.Transform(pointInScreenSpace) ?? PointExtensions.Extreme;
        }

        private void UpdateCalculatedProperties(TransitionSpeed transitionSpeed)
        {
            UpdateLinearZoom();
            UpdateTransitionedTransform(transitionSpeed);
        }

        private void UpdateLinearZoom()
        {
            var newLinearZoom = ToLinearZoom(_exponentialZoom);
            LinearZoomChanged?.Invoke(newLinearZoom);
        }

        private void UpdateTransitionedTransform(TransitionSpeed transitionSpeed)
        {
            _diagramSpaceToScreenSpaceTransform = CreateTransformToScreenSpace();
            var transitionedTransform = new TransitionedTransform(_diagramSpaceToScreenSpaceTransform, transitionSpeed);
            TransformChanged?.Invoke(transitionedTransform);
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

        private void SubscribeToDiagramEvents()
        {
            _diagram.ShapeAdded += (o, a) => UpdateContentRect();
            _diagram.ShapeMoved += (o, a) => UpdateContentRect();
            _diagram.ShapeRemoved += (o, a) => UpdateContentRect();
            _diagram.Cleared += (o, a) => UpdateContentRect();
        }

        private void UpdateContentRect()
        {
            _contentRect = _diagram.ContentRect.ToWpf();
        }

        public class PanCommand : DelegateCommand<Vector, TransitionSpeed>
        {
            public PanCommand(Viewport viewport) : base(viewport.Pan)
            { }

            public void Execute(Vector panVector, TransitionSpeed transitionSpeed)
                => ExecuteCore(panVector, transitionSpeed);
        }

        public class ResizeCommand : DelegateCommand<Size, TransitionSpeed>
        {
            public ResizeCommand(Viewport viewport) : base(viewport.Resize)
            { }

            public void Execute(Size newSize, TransitionSpeed transitionSpeed)
                => ExecuteCore(newSize, transitionSpeed);
        }

        public class ZoomCommand : DelegateCommand<double, Point, TransitionSpeed>
        {
            public ZoomCommand(Viewport viewport) : base(viewport.ZoomWithCenterTo)
            { }

            public void Execute(double zoomValue, Point zoomCenter, TransitionSpeed transitionSpeed)
                => ExecuteCore(zoomValue, zoomCenter, transitionSpeed);
        }

        public class ZoomToContentCommand : DelegateCommand<TransitionSpeed>
        {
            public ZoomToContentCommand(Viewport viewport) : base(viewport.ZoomToContent)
            { }
        }
    }
}
