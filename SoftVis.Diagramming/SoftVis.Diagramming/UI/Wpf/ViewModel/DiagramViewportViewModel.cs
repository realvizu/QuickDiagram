using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// Also handles viewport transform (resize, pan and zoom) in a transitioned style (with a given transition speed).
    /// Also handles which shape has the focus and which one has the decorators (mini buttons).
    /// </summary>
    public class DiagramViewportViewModel : DiagramViewModelBase
    {
        private readonly Map<IDiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;
        private readonly DiagramShapeViewModelFactory _diagramShapeViewModelFactory;
        private readonly DiagramShapeButtonCollectionViewModel _diagramShapeButtonCollectionViewModel;

        private readonly Viewport _viewport;
        private double _viewportZoom;
        private TransitionedTransform _viewportTransform;

        /// <summary>The focused shape is the one that the user points to.</summary>
        private DiagramNodeViewModel _focusedDiagramNode;

        /// <summary>The decorated shape is the one that has the minibuttons attached.</summary>
        private DiagramNodeViewModel _decoratedDiagramNode;

        /// <summary>The decoration is pinned of the minibuttons stay visible even when focus is lost from a shape.</summary>
        private bool _isDecorationPinned;

        public ObservableCollection<DiagramShapeViewModelBase> DiagramShapeViewModels { get; }
        public double MinZoom { get; }
        public double MaxZoom { get; }

        public Viewport.ResizeCommand ViewportResizeCommand { get; }
        public Viewport.PanCommand ViewportPanCommand { get; }
        public Viewport.ZoomToContentCommand ViewportZoomToContentCommand { get; }
        public Viewport.ZoomCommand ViewportZoomCommand { get; }

        public event Action ViewportChanged;
        public event EntitySelectorRequestedEventHandler EntitySelectorRequested;

        public DiagramViewportViewModel(IReadOnlyModel model, IDiagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider,
            double minZoom, double maxZoom, double initialZoom)
            : base(model, diagram)
        {
            _diagramShapeToViewModelMap = new Map<IDiagramShape, DiagramShapeViewModelBase>();
            _diagramShapeViewModelFactory = new DiagramShapeViewModelFactory(model, diagram, diagramBehaviourProvider);
            _diagramShapeButtonCollectionViewModel = new DiagramShapeButtonCollectionViewModel(model, diagram, diagramBehaviourProvider);

            _viewport = new Viewport(diagram, minZoom, maxZoom, initialZoom);
            _viewportTransform = TransitionedTransform.Identity;
            _focusedDiagramNode = null;
            _isDecorationPinned = false;
            _decoratedDiagramNode = null;

            DiagramShapeViewModels = new ObservableCollection<DiagramShapeViewModelBase>();
            MinZoom = minZoom;
            MaxZoom = maxZoom;
            ViewportResizeCommand = new Viewport.ResizeCommand(_viewport);
            ViewportPanCommand = new Viewport.PanCommand(_viewport);
            ViewportZoomToContentCommand = new Viewport.ZoomToContentCommand(_viewport);
            ViewportZoomCommand = new Viewport.ZoomCommand(_viewport);

            SubscribeToViewportEvents();
            SubscribeToDiagramEvents();
            SubscribeToDiagramShapeButtonEvents();

            AddDiagram(diagram);
        }

        public ObservableCollection<DiagramShapeButtonViewModelBase> DiagramShapeButtonViewModels
            => _diagramShapeButtonCollectionViewModel.DiagramButtonViewModels;

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
        public void PinDecoration()
        {
            _isDecorationPinned = true;
        }

        /// <summary>
        /// Lets the decorators (minibuttons) disappear when the shape loses focus.
        /// Exits the "pinned" mode.
        /// </summary>
        public void UnpinDecoration()
        {
            _isDecorationPinned = false;
            ChangeDecorationTo(_focusedDiagramNode);
        }

        private void SubscribeToViewportEvents()
        {
            _viewport.LinearZoomChanged += OnViewportLinearZoomChanged;
            _viewport.TransformChanged += OnViewportTransformChanged;
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += (sender, shape) => OnShapeAdded(shape);
            Diagram.ShapeMoved += (sender, shape) => OnShapeMoved(shape);
            Diagram.ShapeRemoved += (sender, shape) => OnShapeRemoved(shape);
            Diagram.Cleared += (sender, args) => OnDiagramCleared();
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
            diagramShapeViewModel.GotFocus += OnShapeFocused;
            diagramShapeViewModel.LostFocus += OnShapeUnfocused;
            diagramShapeViewModel.RemoveRequested += OnShapeRemoveRequested;

            DiagramShapeViewModels.Add(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Set(diagramShape, diagramShapeViewModel);
        }

        private void OnShapeMoved(IDiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            diagramShapeViewModel.UpdatePropertiesFromDiagramShape();
        }

        private void OnShapeRemoved(IDiagramShape diagramShape)
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
        }

        private void OnDiagramCleared()
        {
            DiagramShapeViewModels.Clear();
            _diagramShapeToViewModelMap.Clear();
        }

        private void OnShapeRemoveRequested(IDiagramShape diagramShape)
        {
            Diagram.RemoveShape(diagramShape);
        }

        private void OnShapeFocused(FocusableViewModelBase focusableViewModel)
        {
            ChangeFocusTo(focusableViewModel as DiagramNodeViewModel);
        }

        private void OnShapeUnfocused(FocusableViewModelBase focusableViewModel)
        {
            ChangeFocusTo(null);
        }

        private void ChangeFocusTo(DiagramNodeViewModel diagramNodeViewModel)
        {
            _focusedDiagramNode = diagramNodeViewModel;

            if (!_isDecorationPinned)
                ChangeDecorationTo(diagramNodeViewModel);
        }

        private void ChangeDecorationTo(DiagramNodeViewModel newlyDecoratedDiagramNodeViewModel)
        {
            if (newlyDecoratedDiagramNodeViewModel == null)
                _diagramShapeButtonCollectionViewModel.HideButtons();
            else
                _diagramShapeButtonCollectionViewModel.AssignButtonsTo(newlyDecoratedDiagramNodeViewModel);

            DecoratedDiagramNode = newlyDecoratedDiagramNodeViewModel;
        }

        private void OnViewportLinearZoomChanged(double newViewportZoom)
        {
            ViewportZoom = newViewportZoom;
        }

        private void OnViewportTransformChanged(TransitionedTransform newTransform)
        {
            ViewportTransform = newTransform;
            ViewportChanged?.Invoke();
        }

        private void OnEntitySelectorRequested(Point attachPointInDiagramSpace,
            HandleOrientation handleOrientation, IEnumerable<IModelEntity> modelEntities)
        {
            var attachPointInScreenSpace = _viewport.ProjectFromDiagramSpaceToScreenSpace(attachPointInDiagramSpace);
            EntitySelectorRequested?.Invoke(attachPointInScreenSpace, handleOrientation, modelEntities);
        }
    }
}
