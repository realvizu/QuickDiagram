using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
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
    public class DiagramViewportViewModel : ModelObserverViewModelBase, IDiagramShapeUiRepository
    {
        public double MinZoom { get; }
        public double MaxZoom { get; }
        public AutoMoveViewportViewModel ViewportCalculator { get; }
        public ThreadSafeObservableCollection<DiagramNodeViewModelBase> DiagramNodeViewModels { get; }
        public ThreadSafeObservableCollection<DiagramConnectorViewModel> DiagramConnectorViewModels { get; }
        public MiniButtonPanelViewModel MiniButtonPanelViewModel { get; }

        private readonly Map<IDiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;
        private readonly IDiagramShapeUiFactory _diagramShapeUiFactory;

        public event Action ViewportManipulation;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event Action<DiagramShapeViewModelBase> DiagramShapeRemoveRequested;
        public event Action<IDiagramNode> DiagramNodeInvoked;

        public DelegateCommand<IDiagramNode> DiagramNodeDoubleClickedCommand { get; }

        public DiagramViewportViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
            IDiagramShapeUiFactory diagramShapeUiFactory, double minZoom, double maxZoom, double initialZoom)
            : base(modelStore, diagramStore)
        {
            MinZoom = minZoom;
            MaxZoom = maxZoom;

            _diagramShapeToViewModelMap = new Map<IDiagramShape, DiagramShapeViewModelBase>();
            _diagramShapeUiFactory = diagramShapeUiFactory;
            _diagramShapeUiFactory.Initialize(this);

            ViewportCalculator = new AutoMoveViewportViewModel(modelStore, diagramStore, minZoom, maxZoom, initialZoom);
            DiagramNodeViewModels = new ThreadSafeObservableCollection<DiagramNodeViewModelBase>();
            DiagramConnectorViewModels = new ThreadSafeObservableCollection<DiagramConnectorViewModel>();
            MiniButtonPanelViewModel = new MiniButtonPanelViewModel();

            DiagramNodeDoubleClickedCommand = new DelegateCommand<IDiagramNode>(i => DiagramNodeInvoked?.Invoke(i));

            ViewportCalculator.TransformChanged += OnViewportTransformChanged;
            DiagramStore.DiagramChanged += OnDiagramChanged;

            AddDiagram(diagramStore.CurrentDiagram);
        }

        public override void Dispose()
        {
            base.Dispose();

            ViewportCalculator.TransformChanged -= OnViewportTransformChanged;
            DiagramStore.DiagramChanged -= OnDiagramChanged;

            ViewportCalculator.Dispose();
            MiniButtonPanelViewModel.Dispose();

            foreach (var diagramNodeViewModel in DiagramNodeViewModels)
                diagramNodeViewModel.Dispose();

            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels)
                diagramConnectorViewModel.Dispose();
        }

        public void FollowDiagramNodes(IEnumerable<IDiagramNode> diagramNodes, TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
            => ViewportCalculator.FollowDiagramNodes(diagramNodes, transitionSpeed);

        public void SetFollowDiagramNodesMode(ViewportAutoMoveMode mode) => ViewportCalculator.Mode = mode;
        public void StopFollowingDiagramNodes() => ViewportCalculator.StopFollowingDiagramNodes();

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ZoomToContent(transitionSpeed);
        public void ZoomToRect(Rect rect, TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ZoomToRect(rect, transitionSpeed);
        public void EnsureRectIsVisible(Rect rect, TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ContainRect(rect, transitionSpeed);
        public bool IsDiagramContentVisible() => ViewportCalculator.IsDiagramRectVisible();

        public void PinDecoration() => MiniButtonPanelViewModel.PinDecoration();
        public void UnpinDecoration() => MiniButtonPanelViewModel.UnpinDecoration();

        public DiagramNodeViewModelBase GetDiagramNodeViewModel(IDiagramNode diagramNode) 
            => _diagramShapeToViewModelMap.Get(diagramNode) as DiagramNodeViewModelBase;

        public DiagramConnectorViewModel GetDiagramConnectorViewModel(IDiagramConnector diagramConnector) 
            => _diagramShapeToViewModelMap.Get(diagramConnector) as DiagramConnectorViewModel;

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            // All diagram-induced view model manipulation must occur on the UI thread to avoid certain race conditions.
            // (E.g. avoid the case when creating a connector view model precedes the creation of its source and target node view models.)
            EnsureUiThread(() => DispatchDiagramChangeEvent(diagramEvent));
        }

        private void DispatchDiagramChangeEvent(DiagramEventBase diagramEvent)
        {
            switch (diagramEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    AddNode(diagramNodeAddedEvent.DiagramNode);
                    break;
                case DiagramNodeRemovedEvent diagramNodeRemovedEvent:
                    RemoveNode(diagramNodeRemovedEvent.DiagramNode);
                    break;
                case DiagramConnectorAddedEvent diagramConnectorAddedEvent:
                    AddConnector(diagramConnectorAddedEvent.DiagramConnector);
                    break;
                case DiagramConnectorRemovedEvent diagramConnectorRemovedEvent:
                    RemoveConnector(diagramConnectorRemovedEvent.DiagramConnector);
                    break;
                case DiagramClearedEvent _:
                    ClearViewport();
                    break;
            }
        }

        private void AddDiagram(IDiagram diagram)
        {
            foreach (var diagramNode in diagram.Nodes)
                AddNode(diagramNode);

            foreach (var diagramConnector in diagram.Connectors)
                AddConnector(diagramConnector);
        }

        private void AddNode(IDiagramNode diagramNode)
        {
            var diagramNodeViewModel = _diagramShapeUiFactory.CreateDiagramNodeViewModel(diagramNode);
            DiagramNodeViewModels.Add(diagramNodeViewModel);

            diagramNodeViewModel.ShowRelatedNodesRequested += OnShowRelatedNodesRequested;
            diagramNodeViewModel.RelatedNodeSelectorRequested += OnEntitySelectorRequested;
            diagramNodeViewModel.RemoveRequested += OnShapeRemoveRequested;

            _diagramShapeToViewModelMap.Set(diagramNode, diagramNodeViewModel);
        }

        private void AddConnector(IDiagramConnector diagramConnector)
        {
            var diagramConnectorViewModel = _diagramShapeUiFactory.CreateDiagramConnectorViewModel(diagramConnector);
            DiagramConnectorViewModels.Add(diagramConnectorViewModel);

            diagramConnectorViewModel.RemoveRequested += OnShapeRemoveRequested;

            _diagramShapeToViewModelMap.Set(diagramConnector, diagramConnectorViewModel);
        }

        private void OnShapeRemoveRequested(IDiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            if (diagramShapeViewModel == null)
                return;

            DiagramShapeRemoveRequested?.Invoke(diagramShapeViewModel);
        }

        private void RemoveNode(IDiagramNode diagramNode)
        {
            var diagramNodeViewModel = GetDiagramNodeViewModel(diagramNode);

            diagramNodeViewModel.ShowRelatedNodesRequested -= OnShowRelatedNodesRequested;
            diagramNodeViewModel.RelatedNodeSelectorRequested -= OnEntitySelectorRequested;
            diagramNodeViewModel.RemoveRequested -= OnShapeRemoveRequested;

            MiniButtonPanelViewModel.Unfocus(diagramNodeViewModel);
            _diagramShapeToViewModelMap.Remove(diagramNode);
            DiagramNodeViewModels.Remove(diagramNodeViewModel);

            diagramNodeViewModel.Dispose();
        }

        private void RemoveConnector(IDiagramConnector diagramConnector)
        {
            var diagramConnectorViewModel = GetDiagramConnectorViewModel(diagramConnector);

            diagramConnectorViewModel.RemoveRequested -= OnShapeRemoveRequested;

            MiniButtonPanelViewModel.Unfocus(diagramConnectorViewModel);
            _diagramShapeToViewModelMap.Remove(diagramConnector);
            DiagramConnectorViewModels.Remove(diagramConnectorViewModel);

            diagramConnectorViewModel.Dispose();
        }

        private void ClearViewport()
        {
            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels.ToArray())
                RemoveConnector(diagramConnectorViewModel.DiagramConnector);

            foreach (var diagramNodeViewModel in DiagramNodeViewModels.ToArray())
                RemoveNode(diagramNodeViewModel.DiagramNode);

            _diagramShapeToViewModelMap.Clear();
        }

        private void OnViewportTransformChanged(TransitionedTransform newTransform)
            => ViewportManipulation?.Invoke();

        private void OnShowRelatedNodesRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => ShowRelatedNodesRequested?.Invoke(ownerButton, modelNodes);

        private void OnEntitySelectorRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => RelatedNodeSelectorRequested?.Invoke(ownerButton, modelNodes);
    }
}
