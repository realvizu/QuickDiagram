using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// Also handles viewport transform (resize, pan and zoom) in a transitioned style (with a given transition speed).
    /// Also handles which shape has the focus and which one has the decorators (mini buttons).
    /// </summary>
    public class DiagramViewportViewModel : DiagramViewModelBase
    {
        public ObservableCollection<DiagramNodeViewModel> DiagramNodeViewModels { get; }
        public ObservableCollection<DiagramConnectorViewModel> DiagramConnectorViewModels { get; }
        public double MinZoom { get; }
        public double MaxZoom { get; }

        private readonly Viewport _viewport;
        private double _viewportZoom;
        private TransitionedTransform _viewportTransform;

        private readonly Map<IDiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;
        private readonly DiagramShapeViewModelFactory _diagramShapeViewModelFactory;
        private readonly DiagramShapeButtonCollectionViewModel _diagramShapeButtonCollectionViewModel;

        private readonly DiagramFocusTracker _diagramFocusTracker;

        /// <summary>The decorated shape is the one that has the minibuttons attached.</summary>
        private DiagramNodeViewModel _decoratedDiagramNode;

        public Viewport.ResizeCommand ViewportResizeCommand { get; }
        public Viewport.PanCommand ViewportPanCommand { get; }
        public Viewport.ZoomToContentCommand ViewportZoomToContentCommand { get; }
        public Viewport.ZoomCommand ViewportZoomCommand { get; }

        public DelegateCommand UnfocusAllCommand { get; }
        public DelegateCommand HideRelatedEntityListBoxCommand { get; }

        public event EntitySelectorRequestedEventHandler ShowEntitySelectorRequested;
        public event Action HideEntitySelectorRequested;
        public event Action<DiagramShapeViewModelBase> DiagramShapeRemoveRequested;

        public DiagramViewportViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            : base(diagram)
        {
            DiagramNodeViewModels = new ObservableCollection<DiagramNodeViewModel>();
            DiagramConnectorViewModels = new ObservableCollection<DiagramConnectorViewModel>();
            MinZoom = minZoom;
            MaxZoom = maxZoom;

            _viewport = new Viewport(diagram, minZoom, maxZoom, initialZoom);
            _viewportTransform = TransitionedTransform.Identity;

            _diagramFocusTracker = new DiagramFocusTracker();
            _diagramFocusTracker.DecoratedNodeChanged += OnDecoratedNodeChanged;

            _diagramShapeToViewModelMap = new Map<IDiagramShape, DiagramShapeViewModelBase>();
            _diagramShapeViewModelFactory = new DiagramShapeViewModelFactory(diagram, DiagramNodeViewModels);
            _diagramShapeButtonCollectionViewModel = new DiagramShapeButtonCollectionViewModel(diagram);

            ViewportResizeCommand = new Viewport.ResizeCommand(_viewport);
            ViewportPanCommand = new Viewport.PanCommand(_viewport);
            ViewportZoomToContentCommand = new Viewport.ZoomToContentCommand(_viewport);
            ViewportZoomCommand = new Viewport.ZoomCommand(_viewport);

            UnfocusAllCommand = new DelegateCommand(_diagramFocusTracker.UnfocusAll);
            HideRelatedEntityListBoxCommand = new DelegateCommand(HideRelatedEntitySelector);

            SubscribeToViewportEvents();
            SubscribeToDiagramEvents();
            SubscribeToDiagramShapeButtonEvents();

            AddDiagram(diagram);
        }


        public ObservableCollection<DiagramShapeButtonViewModelBase> DiagramShapeButtonViewModels
            => _diagramShapeButtonCollectionViewModel.DiagramNodeButtonViewModels;

        public double ViewportZoom
        {
            get { return _viewportZoom; }
            set
            {
                _viewportZoom = value;
                OnPropertyChanged();
            }
        }

        public TransitionedTransform ViewportTransform
        {
            get { return _viewportTransform; }
            set
            {
                _viewportTransform = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Designates the diagram node that has the minibuttons associated with it.
        /// </summary>
        public DiagramNodeViewModel DecoratedDiagramNode
        {
            get { return _decoratedDiagramNode; }
            set
            {
                _decoratedDiagramNode = value;
                OnPropertyChanged();
            }
        }

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
            => _viewport.ZoomToContent(transitionSpeed);

        /// <summary>
        /// Keeps the decorations (minibuttons) visible even when the shape loses focus.
        /// Used when a balloon selector is popped up.
        /// </summary>
        public void PinDecoration() => _diagramFocusTracker.PinDecoration();

        /// <summary>
        /// Lets the decorators (minibuttons) disappear when the shape loses focus.
        /// Exits the "pinned" mode.
        /// </summary>
        public void UnpinDecoration() => _diagramFocusTracker.UnpinDecoration();

        private void SubscribeToViewportEvents()
        {
            _viewport.LinearZoomChanged += OnViewportLinearZoomChanged;
            _viewport.TransformChanged += OnViewportTransformChanged;
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += OnShapeAdded;
            Diagram.ShapeRemoved += OnShapeRemoved;
            Diagram.Cleared += OnDiagramCleared;
        }

        private void SubscribeToDiagramShapeButtonEvents()
        {
            foreach (var showRelatedNodeButtonViewModel in DiagramShapeButtonViewModels.OfType<ShowRelatedNodeButtonViewModel>())
                showRelatedNodeButtonViewModel.EntitySelectorRequested += OnEntitySelectorRequested;
        }

        private void AddDiagram(IDiagram diagram)
        {
            foreach (var diagramNode in diagram.Nodes)
                OnShapeAdded(diagramNode);

            foreach (var diagramConnector in diagram.Connectors)
                OnShapeAdded(diagramConnector);
        }

        private void OnShapeAdded(IDiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeViewModelFactory.CreateViewModel(diagramShape);
            diagramShapeViewModel.RemoveRequested += OnShapeRemoveRequested;
            diagramShapeViewModel.FocusRequested += _diagramFocusTracker.Focus;

            AddToViewModels(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Set(diagramShape, diagramShapeViewModel);
        }

        private void OnShapeRemoveRequested(IDiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            if (diagramShapeViewModel == null)
                return;

            DiagramShapeRemoveRequested?.Invoke(diagramShapeViewModel);

            Diagram.RemoveShape(diagramShape);
        }

        private void OnShapeRemoved(IDiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            if (diagramShapeViewModel == null)
                return;

            _diagramFocusTracker.Unfocus(diagramShapeViewModel);
            diagramShapeViewModel.RemoveRequested -= OnShapeRemoveRequested;
            diagramShapeViewModel.FocusRequested -= _diagramFocusTracker.Focus;

            RemoveFromViewModels(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Remove(diagramShape);
        }

        private void OnDiagramCleared()
        {
            DiagramConnectorViewModels.Clear();
            DiagramNodeViewModels.Clear();
            _diagramShapeToViewModelMap.Clear();
        }

        private void HideRelatedEntitySelector()
        {
            HideEntitySelectorRequested?.Invoke();
        }

        private void OnViewportLinearZoomChanged(double newViewportZoom)
        {
            ViewportZoom = newViewportZoom;
        }

        private void OnViewportTransformChanged(TransitionedTransform newTransform)
        {
            ViewportTransform = newTransform;
            HideRelatedEntitySelector();
        }

        private void OnEntitySelectorRequested(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel,
            IEnumerable<IModelEntity> modelEntities)
        {
            ShowEntitySelectorRequested?.Invoke(diagramNodeButtonViewModel, modelEntities);
        }

        private void OnDecoratedNodeChanged(DiagramNodeViewModel newlyDecoratedDiagramNodeViewModel)
        {
            if (newlyDecoratedDiagramNodeViewModel == null)
                _diagramShapeButtonCollectionViewModel.HideButtons();
            else
                _diagramShapeButtonCollectionViewModel.AssignButtonsTo(newlyDecoratedDiagramNodeViewModel);

            DecoratedDiagramNode = newlyDecoratedDiagramNodeViewModel;
        }

        private void AddToViewModels(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            if (diagramShapeViewModel is DiagramNodeViewModel)
                DiagramNodeViewModels.Add((DiagramNodeViewModel)diagramShapeViewModel);

            else if (diagramShapeViewModel is DiagramConnectorViewModel)
                DiagramConnectorViewModels.Add((DiagramConnectorViewModel)diagramShapeViewModel);

            else
                throw new Exception($"Unexpected DiagramShapeViewModelBase: {diagramShapeViewModel.GetType().Name}");
        }

        private void RemoveFromViewModels(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            if (diagramShapeViewModel is DiagramNodeViewModel)
                DiagramNodeViewModels.Remove((DiagramNodeViewModel)diagramShapeViewModel);

            else if (diagramShapeViewModel is DiagramConnectorViewModel)
                DiagramConnectorViewModels.Remove((DiagramConnectorViewModel)diagramShapeViewModel);

            else
                throw new Exception($"Unexpected DiagramShapeViewModelBase: {diagramShapeViewModel.GetType().Name}");
        }
    }
}
