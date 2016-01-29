using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// </summary>
    public class DiagramViewportViewModel : DiagramViewModelBase
    {
        private readonly Map<DiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;
        private readonly DiagramShapeViewModelFactory _diagramShapeViewModelFactory;
        private readonly DiagramButtonCollectionViewModel _diagramButtonCollectionViewModel;
        private readonly Viewport _viewport;

        private double _viewportZoom;
        private TransitionedTransform _transitionedViewportTransform = TransitionedTransform.Identity;

        public ObservableCollection<DiagramShapeViewModelBase> DiagramShapeViewModels { get; }
        public double MinZoom { get; }
        public double MaxZoom { get; }
        public Viewport.ResizeCommand ViewportResizeCommand { get; }
        public Viewport.PanCommand ViewportPanCommand { get; }
        public Viewport.ZoomToContentCommand ViewportZoomToContentCommand { get; }
        public Viewport.ZoomCommand ViewportZoomCommand { get; }

        public event EntitySelectorRequestedEventHandler EntitySelectorRequested;

        public DiagramViewportViewModel(IModel model, Diagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider,
            double minZoom, double maxZoom, double initialZoom)
            : base(model, diagram)
        {
            _diagramShapeToViewModelMap = new Map<DiagramShape, DiagramShapeViewModelBase>();
            _diagramShapeViewModelFactory = new DiagramShapeViewModelFactory(model, diagram, diagram.ConnectorTypeResolver);
            _diagramButtonCollectionViewModel = new DiagramButtonCollectionViewModel(model, diagram, diagramBehaviourProvider);
            _viewport = new Viewport(minZoom, maxZoom, initialZoom);

            DiagramShapeViewModels = new ObservableCollection<DiagramShapeViewModelBase>();
            MinZoom = minZoom;
            MaxZoom = maxZoom;
            ViewportResizeCommand = new Viewport.ResizeCommand(_viewport);
            ViewportPanCommand = new Viewport.PanCommand(_viewport);
            ViewportZoomToContentCommand = new Viewport.ZoomToContentCommand(_viewport);
            ViewportZoomCommand = new Viewport.ZoomCommand(_viewport);

            SubscribeToViewportEvents();
            SubscribeToDiagramEvents();
            SubscribeToDiagramButtonEvents();
            AddDiagram(diagram);
        }

        public ObservableCollection<DiagramButtonViewModelBase> DiagramButtonViewModels
            => _diagramButtonCollectionViewModel.DiagramButtonViewModels;

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

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
            => _viewport.ZoomToContent(transitionSpeed);

        private void SubscribeToViewportEvents()
        {
            _viewport.LinearZoomChanged += OnViewportLinearZoomChanged;
            _viewport.TransitionedTransformChanged += OnViewportTransitionedTransformChanged;
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += OnShapeAdded;
            Diagram.ShapeMoved += OnShapeMoved;
            Diagram.ShapeRemoved += OnShapeRemoved;
            Diagram.Cleared += OnDiagramCleared;
        }

        private void SubscribeToDiagramButtonEvents()
        {
            foreach (var showRelatedNodeButtonViewModel in DiagramButtonViewModels.OfType<ShowRelatedNodeButtonViewModel>())
                showRelatedNodeButtonViewModel.EntitySelectorRequested += OnEntitySelectorRequested;
        }

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
            if (diagramShapeViewModel == null)
                return;

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
            Diagram.RemoveShape(diagramShape);
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
                return;

            if (_diagramButtonCollectionViewModel.AreButtonsAssignedTo(diagramShapeViewModel))
                _diagramButtonCollectionViewModel.HideButtons();
        }

        private void UpdateDiagramContentRect()
        {
            _viewport.UpdateContentRect(Diagram.ContentRect.ToWpf());
        }

        private void OnViewportLinearZoomChanged(double viewportZoom)
        {
            ViewportZoom = viewportZoom;
        }

        private void OnViewportTransitionedTransformChanged(TransitionedTransform transitionedTransform)
        {
            TransitionedViewportTransform = transitionedTransform;
        }

        private void OnEntitySelectorRequested(Point attachPointInDiagramSpace,
            HandleOrientation handleOrientation, IEnumerable<IModelEntity> modelEntities)
        {
            var attachPointInScreenSpace = _viewport.ProjectFromDiagramSpaceToScreenSpace(attachPointInDiagramSpace);
            EntitySelectorRequested?.Invoke(attachPointInScreenSpace, handleOrientation, modelEntities);
        }
    }
}
