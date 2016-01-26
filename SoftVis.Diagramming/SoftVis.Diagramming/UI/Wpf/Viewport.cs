using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Calculates the properties of the viewport.
    /// Stateful, publishes events when calculated properties change.
    /// </summary>
    public class Viewport
    {
        private const double MinZoomDefault = 0.1;
        private const double MaxZoomDefault = 10;
        private const double InitialZoomDefault = 1;
        private static readonly Size ViewportSizeDefault = new Size(0, 0);
        private static readonly Point ViewportCenterDefault = new Point(0, 0);
        private static readonly Rect ContentRectDefault = Rect.Empty;

        private readonly double _minZoom;
        private readonly double _maxZoom;
        private readonly double _defaultExponentialZoom;
        private double _linearZoom;
        private Size _sizeInScreenSpace;
        private Point _centerInDiagramSpace;
        private double _exponentialZoom;
        private TransitionedTransform _transitionedTransform = TransitionedTransform.Identity;

        public Rect ContentRect { get; set; }

        public event Action<double> LinearZoomChanged;
        public event Action<TransitionedTransform> TransitionedTransformChanged;

        public Viewport(double minZoom = MinZoomDefault, double maxZoom = MaxZoomDefault, double initialZoom = InitialZoomDefault)
            : this(minZoom, maxZoom, initialZoom, ViewportSizeDefault, ViewportCenterDefault, ContentRectDefault)
        {
        }

        private Viewport(double minZoom, double maxZoom, double initialZoom,
            Size sizeInScreenSpace, Point centerInDiagramSpace, Rect contentRect)
        {
            _minZoom = minZoom;
            _maxZoom = maxZoom;
            _defaultExponentialZoom = initialZoom;
            _sizeInScreenSpace = sizeInScreenSpace;
            _centerInDiagramSpace = centerInDiagramSpace;
            _linearZoom = ToLinearZoom(initialZoom);
            ContentRect = contentRect;

            UpdateCalculatedProperties(TransitionSpeed.Instant);
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
            var exponentialZoom = CalculateZoomForContent(ContentRect.Size);
            _linearZoom = ToLinearZoom(exponentialZoom);
            _centerInDiagramSpace = ContentRect.GetCenter();
            UpdateCalculatedProperties(transitionSpeed);
        }

        public void ZoomWithCenterTo(double newLinearZoom, Point zoomCenterInScreenSpace,
            TransitionSpeed transitionSpeed = TransitionSpeed.Fast)
        {
            var zoomCenterInDiagramSpace = ProjectToDiagramSpace(zoomCenterInScreenSpace);
            var newExponentialZoom = ToExponentialZoom(newLinearZoom);
            var relativeZoom = _exponentialZoom / newExponentialZoom;

            _linearZoom = newLinearZoom;
            _centerInDiagramSpace = (_centerInDiagramSpace - zoomCenterInDiagramSpace) * relativeZoom + zoomCenterInDiagramSpace;
            UpdateCalculatedProperties(transitionSpeed);
        }

        private void UpdateCalculatedProperties(TransitionSpeed transitionSpeed)
        {
            UpdateLinearZoom(_linearZoom);
            _exponentialZoom = ToExponentialZoom(_linearZoom);
            var transform = CreateTransformToScreenSpace();
            UpdateTransitionedTransform(new TransitionedTransform(transform, transitionSpeed));
        }

        private void UpdateLinearZoom(double newLinearZoom)
        {
            _linearZoom = newLinearZoom;
            LinearZoomChanged?.Invoke(_linearZoom);
        }

        private void UpdateTransitionedTransform(TransitionedTransform newTransitionedTransform)
        {
            _transitionedTransform = newTransitionedTransform;
            TransitionedTransformChanged?.Invoke(_transitionedTransform);
        }

        private double ToExponentialZoom(double linearZoom)
        {
            return ScaleCalculator.LinearToExponential(linearZoom, _minZoom, _maxZoom);
        }

        private double ToLinearZoom(double exponentialZoom)
        {
            return ScaleCalculator.ExponentialToLinear(exponentialZoom, _minZoom, _maxZoom);
        }

        private Point ProjectToDiagramSpace(Point pointInScreenSpace)
        {
            var screenCenter = new Rect(_sizeInScreenSpace).GetCenter();
            var vectorToScreenCenter = screenCenter - pointInScreenSpace;
            var vectorResizedToDiagramSpace = vectorToScreenCenter / _exponentialZoom;
            return _centerInDiagramSpace - vectorResizedToDiagramSpace;
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
    }
}
