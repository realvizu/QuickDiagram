using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// </summary>
    public class DiagramViewportViewModel : ViewModelBase
    {
        private readonly Diagram _diagram;
        private readonly Map<DiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;
        private readonly DiagramShapeViewModelFactory _diagramShapeViewModelFactory;
        private readonly DiagramButtonCollectionViewModel _diagramButtonCollectionViewModel;
        private readonly Viewport _viewport;

        private double _minZoom;
        private double _maxZoom;
        private double _viewportZoom;
        private TransitionedTransform _transitionedViewportTransform = TransitionedTransform.Identity;

        private ICommand _resizeCommand;
        private ICommand _panCommand;
        private ICommand _zoomToContentCommand;
        private ICommand _zoomCommand;

        public ObservableCollection<DiagramShapeViewModelBase> DiagramShapeViewModels { get; }

        public DiagramViewportViewModel(Diagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider,
            double minZoom, double maxZoom, double initialZoom)
        {
            _diagram = diagram;
            _diagramShapeToViewModelMap = new Map<DiagramShape, DiagramShapeViewModelBase>();
            _diagramShapeViewModelFactory = new DiagramShapeViewModelFactory(diagram.ConnectorTypeResolver);
            _diagramButtonCollectionViewModel = new DiagramButtonCollectionViewModel(diagramBehaviourProvider);

            MinZoom = minZoom;
            MaxZoom = maxZoom;

            _viewport = new Viewport(minZoom, maxZoom, initialZoom);
            _viewport.LinearZoomChanged += OnViewportLinearZoomChanged;
            _viewport.TransitionedTransformChanged += OnViewportTransitionedTransformChanged;

            ResizeCommand = new DelegateCommand<Size, TransitionSpeed>(_viewport.Resize);
            PanCommand = new DelegateCommand<Vector, TransitionSpeed>(_viewport.Pan);
            ZoomToContentCommand = new DelegateCommand<TransitionSpeed>(_viewport.ZoomToContent);
            ZoomCommand = new DelegateCommand<double, Point, TransitionSpeed>(_viewport.ZoomWithCenterTo);

            DiagramShapeViewModels = new ObservableCollection<DiagramShapeViewModelBase>();

            diagram.ShapeAdded += OnShapeAdded;
            diagram.ShapeMoved += OnShapeMoved;
            diagram.ShapeRemoved += OnShapeRemoved;
            diagram.Cleared += OnDiagramCleared;

            AddDiagram(diagram);
        }

        public ObservableCollection<DiagramButtonViewModelBase> DiagramButtonViewModels
            => _diagramButtonCollectionViewModel.DiagramButtonViewModels;

        public double MinZoom
        {
            get { return _minZoom; }
            set
            {
                _minZoom = value;
                OnPropertyChanged();
            }
        }

        public double MaxZoom
        {
            get { return _maxZoom; }
            set
            {
                _maxZoom = value;
                OnPropertyChanged();
            }
        }

        public double ViewportZoom
        {
            get { return _viewportZoom; }
            set
            {
                _viewportZoom = value;
                OnPropertyChanged();
            }
        }

        public TransitionedTransform TransitionedViewportTransform
        {
            get { return _transitionedViewportTransform; }
            set
            {
                _transitionedViewportTransform = value;
                OnPropertyChanged();
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

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
            => _viewport.ZoomToContent(transitionSpeed);

        private void AddDiagram(Diagram diagram)
        {
            foreach (var diagramNode in diagram.Nodes)
                OnShapeAdded(null, diagramNode);

            foreach (var diagramConnector in diagram.Connectors)
                OnShapeAdded(null, diagramConnector);

            UpdateDiagramContentRect();
        }

        private void OnShapeAdded(object sender, DiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeViewModelFactory.CreateViewModel(diagramShape);
            diagramShapeViewModel.GotFocus += OnShapeFocused;
            diagramShapeViewModel.LostFocus += OnShapeUnfocused;
            diagramShapeViewModel.RemoveRequested += OnShapeRemoveRequested;

            DiagramShapeViewModels.Add(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Set(diagramShape, diagramShapeViewModel);

            UpdateDiagramContentRect();
        }

        private void OnShapeMoved(object sender, DiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            diagramShapeViewModel.UpdateState();

            UpdateDiagramContentRect();
        }

        private void OnShapeRemoved(object sender, DiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            OnShapeUnfocused(diagramShapeViewModel);
            diagramShapeViewModel.GotFocus -= OnShapeFocused;
            diagramShapeViewModel.LostFocus -= OnShapeUnfocused;
            diagramShapeViewModel.RemoveRequested -= OnShapeRemoveRequested;

            DiagramShapeViewModels.Remove(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Remove(diagramShape);

            UpdateDiagramContentRect();
        }

        private void OnDiagramCleared(object sender, EventArgs e)
        {
            DiagramShapeViewModels.Clear();
            _diagramShapeToViewModelMap.Clear();

            UpdateDiagramContentRect();
        }

        private void OnShapeRemoveRequested(DiagramShape diagramShape)
        {
            _diagram.RemoveShape(diagramShape);
        }

        private void OnShapeFocused(FocusableViewModelBase focusableViewModel)
        {
            var diagramShapeViewModel = focusableViewModel as DiagramShapeViewModelBase;
            if (diagramShapeViewModel == null)
                throw new ArgumentException("DiagramShapeViewModelBase expected");

            _diagramButtonCollectionViewModel.AssignButtonsTo(diagramShapeViewModel);
        }

        private void OnShapeUnfocused(FocusableViewModelBase focusableViewModel)
        {
            var diagramShapeViewModel = focusableViewModel as DiagramShapeViewModelBase;
            if (diagramShapeViewModel == null)
                throw new ArgumentException("DiagramShapeViewModelBase expected");

            if (_diagramButtonCollectionViewModel.AreButtonsAssignedTo(diagramShapeViewModel))
                _diagramButtonCollectionViewModel.HideButtons();
        }

        private void UpdateDiagramContentRect()
        {
            _viewport.UpdateContentRect(_diagram.ContentRect.ToWpf());
        }

        private void OnViewportLinearZoomChanged(double viewportZoom)
        {
            ViewportZoom = viewportZoom;
        }

        private void OnViewportTransitionedTransformChanged(TransitionedTransform transitionedTransform)
        {
            TransitionedViewportTransform = transitionedTransform;
        }
    }
}
