using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf.Collections;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.Transforms;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// Also handles viewport transform (resize, pan and zoom) in a transitioned style (with a given transition speed).
    /// Also handles which shape has the focus and which one has the decorators (mini buttons).
    /// </summary>
    public class DiagramViewportViewModel : DiagramViewModelBase, IDisposable
    {
        private bool _areDiagramNodeDescriptionsVisible;

        public double MinZoom { get; }
        public double MaxZoom { get; }
        public AutoMoveViewportViewModel ViewportCalculator { get; }
        public ThreadSafeObservableCollection<DiagramNodeViewModel> DiagramNodeViewModels { get; }
        public ThreadSafeObservableCollection<DiagramConnectorViewModel> DiagramConnectorViewModels { get; }
        public ThreadSafeObservableCollection<DiagramShapeButtonViewModelBase> DiagramNodeButtonViewModels { get; }
        public DecorationManagerViewModel<DiagramNodeViewModel> DecorationManager { get; }

        private readonly Map<IDiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;
        private readonly DiagramShapeViewModelFactory _diagramShapeViewModelFactory;

        public event Action ViewportManipulation;
        public event ShowRelatedNodeButtonEventHandler ShowEntitySelectorRequested;
        public event ShowRelatedNodeButtonEventHandler ShowRelatedEntitiesRequested;
        public event Action<DiagramShapeViewModelBase> DiagramShapeRemoveRequested;
        public event Action<IDiagramShape> ShowSourceRequested;

        public DelegateCommand<IDiagramShape> ShowSourceCommand { get; }

        public DiagramViewportViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            : base(diagram)
        {
            MinZoom = minZoom;
            MaxZoom = maxZoom;

            ViewportCalculator = new AutoMoveViewportViewModel(diagram, minZoom, maxZoom, initialZoom);
            DiagramNodeViewModels = new ThreadSafeObservableCollection<DiagramNodeViewModel>();
            DiagramConnectorViewModels = new ThreadSafeObservableCollection<DiagramConnectorViewModel>();
            DiagramNodeButtonViewModels = new ThreadSafeObservableCollection<DiagramShapeButtonViewModelBase>(CreateDiagramNodeButtons());
            DecorationManager = new DecorationManagerViewModel<DiagramNodeViewModel>(DiagramNodeButtonViewModels);

            _diagramShapeToViewModelMap = new Map<IDiagramShape, DiagramShapeViewModelBase>();
            _diagramShapeViewModelFactory = new DiagramShapeViewModelFactory(diagram, DiagramNodeViewModels);

            ShowSourceCommand = new DelegateCommand<IDiagramShape>(OnShowSourceCommand);

            SubscribeToViewportEvents();
            SubscribeToDiagramEvents();
            SubscribeToDiagramShapeButtonEvents();

            AddDiagram(diagram);
        }

        public void Dispose()
        {
            UnsubscribeFromViewportEvents();
            UnsubscribeFromDiagramEvents();
            UnsubscribeFromDiagramShapeButtonEvents();

            ViewportCalculator.Dispose();

            foreach (var diagramNodeViewModel in DiagramNodeViewModels)
                diagramNodeViewModel.Dispose();

            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels)
                diagramConnectorViewModel.Dispose();

            foreach (var diagramShapeButtonViewModel in DiagramNodeButtonViewModels)
                diagramShapeButtonViewModel.Dispose();
        }

        public void FollowDiagramNodes(IEnumerable<IDiagramNode> diagramNodes, TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
            => ViewportCalculator.FollowDiagramNodes(diagramNodes, transitionSpeed);

        public void SetFollowDiagramNodesMode(ViewportAutoMoveMode mode) => ViewportCalculator.Mode = mode;
        public void StopFollowingDiagramNodes() => ViewportCalculator.StopFollowingDiagramNodes();

        public void ExpandAllDiagramNodes() => SetDiagramNodeDescriptionVisibility(true);
        public void CollapseAllDiagramNodes() => SetDiagramNodeDescriptionVisibility(false);

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ZoomToContent(transitionSpeed);
        public void ZoomToRect(Rect rect, TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ZoomToRect(rect, transitionSpeed);
        public void EnsureRectIsVisible(Rect rect, TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ContainRect(rect, transitionSpeed);
        public bool IsDiagramContentVisible() => ViewportCalculator.IsDiagramRectVisible();

        public void PinDecoration() => DecorationManager.PinDecoration();
        public void UnpinDecoration() => DecorationManager.UnpinDecoration();

        private void SubscribeToViewportEvents()
        {
            ViewportCalculator.TransformChanged += OnViewportTransformChanged;
        }

        private void UnsubscribeFromViewportEvents()
        {
            ViewportCalculator.TransformChanged -= OnViewportTransformChanged;
        }

        // All diagram-induced view model manipulation must occur on the UI thread to avoid certain race conditions.
        // (E.g. avoid the case when creating a connector view model precedes the creation of its source and target node view models.)
        private void OnDiagramShapeAdded(IDiagramShape diagramShape) => EnsureUiThread(() => AddShape(diagramShape));
        private void OnDiagramShapeRemoved(IDiagramShape diagramShape) => EnsureUiThread(() => RemoveShape(diagramShape));
        private void OnDiagramCleared() => EnsureUiThread(ClearViewport);

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += OnDiagramShapeAdded;
            Diagram.ShapeRemoved += OnDiagramShapeRemoved;
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
            foreach (var showRelatedNodeButtonViewModel in DiagramNodeButtonViewModels.OfType<ShowRelatedNodeButtonViewModel>())
            {
                showRelatedNodeButtonViewModel.EntitySelectorRequested += OnEntitySelectorRequested;
                showRelatedNodeButtonViewModel.ShowRelatedEntitiesRequested += OnShowRelatedEntitiesRequested;
            }
        }

        private void UnsubscribeFromDiagramShapeButtonEvents()
        {
            foreach (var showRelatedNodeButtonViewModel in DiagramNodeButtonViewModels.OfType<ShowRelatedNodeButtonViewModel>())
            {
                showRelatedNodeButtonViewModel.EntitySelectorRequested -= OnEntitySelectorRequested;
                showRelatedNodeButtonViewModel.ShowRelatedEntitiesRequested -= OnShowRelatedEntitiesRequested;
            }
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
            var diagramShapeViewModel = _diagramShapeViewModelFactory.CreateViewModel(diagramShape, _areDiagramNodeDescriptionsVisible);
            diagramShapeViewModel.RemoveRequested += OnShapeRemoveRequested;

            AddToViewModels(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Set(diagramShape, diagramShapeViewModel);
        }

        private void OnShapeRemoveRequested(IDiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            if (diagramShapeViewModel == null)
                return;

            DiagramShapeRemoveRequested?.Invoke(diagramShapeViewModel);

            Diagram.RemoveDiagramShape(diagramShape);
        }

        private void RemoveShape(IDiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            if (diagramShapeViewModel == null)
                return;

            if (diagramShapeViewModel is DiagramNodeViewModel)
                DecorationManager.Unfocus(diagramShapeViewModel as DiagramNodeViewModel);

            diagramShapeViewModel.RemoveRequested -= OnShapeRemoveRequested;

            RemoveFromViewModels(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Remove(diagramShape);
        }

        private void ClearViewport()
        {
            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels.ToArray())
                RemoveFromViewModels(diagramConnectorViewModel);

            foreach (var diagramNodeViewModel in DiagramNodeViewModels.ToArray())
                RemoveFromViewModels(diagramNodeViewModel);

            _diagramShapeToViewModelMap.Clear();
        }

        private void OnViewportTransformChanged(TransitionedTransform newTransform)
            => ViewportManipulation?.Invoke();

        private void OnEntitySelectorRequested(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel, IReadOnlyList<IModelEntity> modelEntities)
            => ShowEntitySelectorRequested?.Invoke(diagramNodeButtonViewModel, modelEntities);

        private void OnShowRelatedEntitiesRequested(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel, IReadOnlyList<IModelEntity> modelEntities)
            => ShowRelatedEntitiesRequested?.Invoke(diagramNodeButtonViewModel, modelEntities);

        private void OnShowSourceCommand(IDiagramShape diagramShape)
            => ShowSourceRequested?.Invoke(diagramShape);

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
                var diagramNodeViewModel = (DiagramNodeViewModel)diagramShapeViewModel;
                DiagramNodeViewModels.Remove(diagramNodeViewModel);
                diagramNodeViewModel.Dispose();
            }
            else if (diagramShapeViewModel is DiagramConnectorViewModel)
            {
                var diagramConnectorViewModel = (DiagramConnectorViewModel)diagramShapeViewModel;
                DiagramConnectorViewModels.Remove(diagramConnectorViewModel);
                diagramConnectorViewModel.Dispose();
            }
            else
                throw new Exception($"Unexpected DiagramShapeViewModelBase: {diagramShapeViewModel.GetType().Name}");
        }

        private IEnumerable<DiagramShapeButtonViewModelBase> CreateDiagramNodeButtons()
        {
            yield return new CloseShapeButtonViewModel(Diagram);

            foreach (var entityRelationType in Diagram.GetEntityRelationTypes())
                yield return new ShowRelatedNodeButtonViewModel(Diagram, entityRelationType);
        }

        private void SetDiagramNodeDescriptionVisibility(bool isVisible)
        {
            _areDiagramNodeDescriptionsVisible = isVisible;

            foreach (var diagramNodeViewModel in DiagramNodeViewModels)
                diagramNodeViewModel.IsDescriptionVisible = _areDiagramNodeDescriptionsVisible;
        }
    }
}
