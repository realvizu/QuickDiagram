using System.Linq;
using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Calculates the properties of the viewport.
    /// </summary>
    public class ViewportViewModel : ViewModelBase
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
        private Rect _contentRect;

        private double _exponentialZoom;
        private ViewportDescriptor _viewportDescriptor;

        private ICommand _resizeCommand;
        private ICommand _panCommand;
        private ICommand _zoomToContentCommand;
        private ICommand _zoomCommand;

        public ViewportViewModel(double minZoom = MinZoomDefault, double maxZoom = MaxZoomDefault, double initialZoom = InitialZoomDefault)
            : this(minZoom, maxZoom, initialZoom, ViewportSizeDefault, ViewportCenterDefault, ContentRectDefault)
        {
        }

        private ViewportViewModel(double minZoom, double maxZoom, double initialZoom, 
            Size sizeInScreenSpace, Point centerInDiagramSpace, Rect contentRect)
        {
            _minZoom = minZoom;
            _maxZoom = maxZoom;
            _defaultExponentialZoom = initialZoom;
            _linearZoom = ToLinearZoom(initialZoom);
            _sizeInScreenSpace = sizeInScreenSpace;
            _centerInDiagramSpace = centerInDiagramSpace;
            _contentRect = contentRect;

            UpdateCalculatedProperties(TransitionSpeed.Instant);

            ResizeCommand = new DelegateCommand<Size, TransitionSpeed>(Resize);
            PanCommand = new DelegateCommand<Vector, TransitionSpeed>(Pan);
            ZoomToContentCommand = new DelegateCommand<TransitionSpeed>(ZoomToContent);
            ZoomCommand = new DelegateCommand<double, Point, TransitionSpeed>(ZoomWithCenterTo);
        }

        public ViewportDescriptor ViewportDescriptor
        {
            get { return _viewportDescriptor; }
            set
            {
                if (_viewportDescriptor != value)
                {
                    _viewportDescriptor = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ResizeCommand
        {
            get { return _resizeCommand; }
            set
            {
                _resizeCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand PanCommand
        {
            get { return _panCommand; }
            set
            {
                _panCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand ZoomToContentCommand
        {
            get { return _zoomToContentCommand; }
            set
            {
                _zoomToContentCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand ZoomCommand
        {
            get { return _zoomCommand; }
            set
            {
                _zoomCommand = value;
                OnPropertyChanged();
            }
        }

        public void Resize(Size sizeInScreenSpace, TransitionSpeed transitionSpeed)
        {
            _sizeInScreenSpace = sizeInScreenSpace;
            UpdateCalculatedProperties(transitionSpeed);
        }

        public void UpdateContentRect(Rect contentRect)
        {
            _contentRect = contentRect;
        }

        public void Pan(Vector panVectorInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
        {
            var viewportMoveVectorInScreenSpace = panVectorInScreenSpace * -1;
            var viewportMoveVectorInDiagramSpace = viewportMoveVectorInScreenSpace / _exponentialZoom;

            _centerInDiagramSpace += viewportMoveVectorInDiagramSpace;
            UpdateCalculatedProperties(transitionSpeed);
        }

        public void ZoomToContent(TransitionSpeed transitionSpeed)
        {
            var exponentialZoom = CalculateZoomForContent(_contentRect.Size);
            _linearZoom = ToLinearZoom(exponentialZoom);
            _centerInDiagramSpace = _contentRect.GetCenter();
            UpdateCalculatedProperties(transitionSpeed);
        }

        public void ZoomWithCenterTo(double newLinearZoom, Point zoomCenterInScreenSpace, TransitionSpeed transitionSpeed)
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
            _exponentialZoom = ToExponentialZoom(_linearZoom);
            ViewportDescriptor = new ViewportDescriptor(_sizeInScreenSpace, _centerInDiagramSpace,
                _linearZoom, _minZoom, _maxZoom, _exponentialZoom, transitionSpeed);
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
    }
}
