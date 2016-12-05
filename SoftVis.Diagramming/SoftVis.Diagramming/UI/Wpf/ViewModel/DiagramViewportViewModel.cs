using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf.Collections;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.Transforms;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// Also handles viewport transform (resize, pan and zoom) in a transitioned style (with a given transition speed).
    /// Also handles which shape has the focus and which one has the decorators (mini buttons).
    /// </summary>
    public class DiagramViewportViewModel : DiagramViewModelBase, IDisposable
    {
        public ThreadSafeObservableList<DiagramNodeViewModel> DiagramNodeViewModels { get; }
        public ThreadSafeObservableList<DiagramConnectorViewModel> DiagramConnectorViewModels { get; }
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

        public event Action InputReceived;
        public event EntitySelectorRequestedEventHandler ShowEntitySelectorRequested;
        public event Action<DiagramShapeViewModelBase> DiagramShapeRemoveRequested;

        public DiagramViewportViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            : base(diagram)
        {
            DiagramNodeViewModels = new ThreadSafeObservableList<DiagramNodeViewModel>();
            DiagramConnectorViewModels = new ThreadSafeObservableList<DiagramConnectorViewModel>();

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
            HideRelatedEntityListBoxCommand = new DelegateCommand(OnInputReceived);

            SubscribeToViewportEvents();
            SubscribeToDiagramEvents();
            SubscribeToDiagramShapeButtonEvents();

            AddDiagram(diagram);
        }

        public void Dispose()
        {
            _diagramFocusTracker.DecoratedNodeChanged -= OnDecoratedNodeChanged;

            UnsubscribeFromViewportEvents();
            UnsubscribeFromDiagramEvents();
            UnsubscribeFromDiagramShapeButtonEvents();

            foreach (var diagramNodeViewModel in DiagramNodeViewModels)
                diagramNodeViewModel.Dispose();

            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels)
                diagramConnectorViewModel.Dispose();

            _diagramShapeButtonCollectionViewModel.Dispose();
        }

        public ThreadSafeObservableList<DiagramShapeButtonViewModelBase> DiagramShapeButtonViewModels
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

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Slow) => _viewport.ZoomToContent(transitionSpeed);
        public bool IsDiagramContentVisible() => _viewport.IsDiagramRectVisible();

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

        private void UnsubscribeFromViewportEvents()
        {
            _viewport.LinearZoomChanged -= OnViewportLinearZoomChanged;
            _viewport.TransformChanged -= OnViewportTransformChanged;
        }

        // All diagram-induced view model manipulation must occur on the UI thread to avoid certain race conditions.
        // (E.g. avoid the case when creating a connector view model precedes the creation of its source and target node view models.)
        private void OnDiagramShapeAdded(IDiagramShape diagramShape) => EnsureUiThread(() => AddShape(diagramShape));
        private void OnDiagramShapeRemoved(IDiagramShape diagramShape) => EnsureUiThread(() => RemoveShape(diagramShape));
        private void OnDiagramCleared() => EnsureUiThread(ClearViewport);

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += OnDiagramShapeAdded;
            Diagram.ShapeRemoved +=  OnDiagramShapeRemoved;
            Diagram.DiagramCleared += OnDiagramCleared;
        }

        private void UnsubscribeFromDiagramEvents()
        {
            Diagram.ShapeAdded -= OnDiagramShapeAdded;
            Diagram.ShapeRemoved -= OnDiagramShapeRemoved;
            Diagram.DiagramCleared -= OnDiagramCleared;
        }

        private void SubscribeToDiagramShapeButtonEvents()
        {
            foreach (var showRelatedNodeButtonViewModel in DiagramShapeButtonViewModels.OfType<ShowRelatedNodeButtonViewModel>())
                showRelatedNodeButtonViewModel.EntitySelectorRequested += OnEntitySelectorRequested;
        }

        private void UnsubscribeFromDiagramShapeButtonEvents()
        {
            foreach (var showRelatedNodeButtonViewModel in DiagramShapeButtonViewModels.OfType<ShowRelatedNodeButtonViewModel>())
                showRelatedNodeButtonViewModel.EntitySelectorRequested -= OnEntitySelectorRequested;
        }

        private void AddDiagram(IDiagram diagram)
        {
            foreach (var diagramNode in diagram.Nodes)
                AddShape(diagramNode);

            foreach (var diagramConnector in diagram.Connectors)
                AddShape(diagramConnector);
        }

        private void AddShape(IDiagramShape diagramShape)
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

        private void RemoveShape(IDiagramShape diagramShape)
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

        private void ClearViewport()
        {
            DiagramConnectorViewModels.Clear();
            DiagramNodeViewModels.Clear();
            _diagramShapeToViewModelMap.Clear();
        }

        private void OnInputReceived()
        {
            InputReceived?.Invoke();
        }

        private void OnViewportLinearZoomChanged(double newViewportZoom)
        {
            ViewportZoom = newViewportZoom;
        }

        private void OnViewportTransformChanged(TransitionedTransform newTransform)
        {
            ViewportTransform = newTransform;
            OnInputReceived();
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
            {
                var diagramNodeViewModel = (DiagramNodeViewModel) diagramShapeViewModel;
                DiagramNodeViewModels.Remove(diagramNodeViewModel);
                diagramNodeViewModel.Dispose();
            }
            else if (diagramShapeViewModel is DiagramConnectorViewModel)
            {
                var diagramConnectorViewModel = (DiagramConnectorViewModel) diagramShapeViewModel;
                DiagramConnectorViewModels.Remove(diagramConnectorViewModel);
                diagramConnectorViewModel.Dispose();
            }
            else
                throw new Exception($"Unexpected DiagramShapeViewModelBase: {diagramShapeViewModel.GetType().Name}");
        }
    }
}
