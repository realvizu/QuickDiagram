using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf.Collections;
using Codartis.Util.UI.Wpf.Commands;
using Codartis.Util.UI.Wpf.Transforms;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/destroys diagram shape viewmodels accordingly.
    /// However, the diagram shape viewmodels are responsible for updating themselves.
    /// Also handles viewport transform (resize, pan and zoom) in a transitioned style (with a given transition speed).
    /// Also handles which shape has the focus and which one has the decorators (mini buttons).
    /// </summary>
    public class DiagramViewportViewModel : ModelObserverViewModelBase, IDiagramViewportUi
    {
        public double MinZoom { get; }
        public double MaxZoom { get; }
        public AutoMoveViewportViewModel ViewportCalculator { get; }
        public IDiagramShapeUiFactory DiagramShapeUiFactory { get; }
        public ThreadSafeObservableCollection<DiagramNodeViewModel> DiagramNodeViewModels { get; }
        public ThreadSafeObservableCollection<DiagramConnectorViewModel> DiagramConnectorViewModels { get; }
        public MiniButtonPanelViewModel MiniButtonPanelViewModel { get; }

        private readonly Map<ModelNodeId, DiagramNodeViewModel> _diagramNodeToViewModelMap;
        private readonly Map<ModelRelationshipId, DiagramConnectorViewModel> _diagramConnectorToViewModelMap;

        public event Action ViewportManipulation;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event Action<DiagramNodeViewModel> RemoveDiagramNodeRequested;
        public event Action<IDiagramNode> DiagramNodeInvoked;
        public event Action<IDiagramNode, Size2D> DiagramNodeHeaderSizeChanged;

        public DelegateCommand<IDiagramNode> DiagramNodeDoubleClickedCommand { get; }

        public DiagramViewportViewModel(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramShapeUiFactory diagramShapeUiFactory,
            [NotNull] IDecorationManager<IMiniButton, IDiagramShapeUi> miniButtonManager,
            double minZoom,
            double maxZoom,
            double initialZoom)
            : base(modelEventSource, diagramEventSource)
        {
            MinZoom = minZoom;
            MaxZoom = maxZoom;

            _diagramNodeToViewModelMap = new Map<ModelNodeId, DiagramNodeViewModel>();
            _diagramConnectorToViewModelMap = new Map<ModelRelationshipId, DiagramConnectorViewModel>();

            DiagramShapeUiFactory = diagramShapeUiFactory;
            MiniButtonPanelViewModel = (MiniButtonPanelViewModel)miniButtonManager;

            ViewportCalculator = new AutoMoveViewportViewModel(modelEventSource, diagramEventSource, minZoom, maxZoom, initialZoom);
            DiagramNodeViewModels = new ThreadSafeObservableCollection<DiagramNodeViewModel>();
            DiagramConnectorViewModels = new ThreadSafeObservableCollection<DiagramConnectorViewModel>();

            DiagramNodeDoubleClickedCommand = new DelegateCommand<IDiagramNode>(i => DiagramNodeInvoked?.Invoke(i));

            ViewportCalculator.TransformChanged += OnViewportTransformChanged;
            DiagramEventSource.DiagramChanged += OnDiagramChanged;

            AddDiagram(diagramEventSource.LatestDiagram);
        }

        public IDecorationManager<IMiniButton, IDiagramShapeUi> MiniButtonManager => MiniButtonPanelViewModel;
        public IEnumerable<IDiagramNodeUi> DiagramNodeUis => DiagramNodeViewModels;
        public IEnumerable<IDiagramConnectorUi> DiagramConnectorUis => DiagramConnectorViewModels;

        public override void Dispose()
        {
            base.Dispose();

            ViewportCalculator.TransformChanged -= OnViewportTransformChanged;
            DiagramEventSource.DiagramChanged -= OnDiagramChanged;

            ViewportCalculator.Dispose();
            MiniButtonManager.Dispose();

            foreach (var diagramNodeViewModel in DiagramNodeViewModels)
                diagramNodeViewModel.Dispose();

            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels)
                diagramConnectorViewModel.Dispose();
        }

        public void FollowDiagramNodes(IEnumerable<ModelNodeId> nodeIds, TransitionSpeed transitionSpeed = TransitionSpeed.Slow)
            => ViewportCalculator.FollowDiagramNodes(nodeIds, transitionSpeed);

        public void SetFollowDiagramNodesMode(ViewportAutoMoveMode mode) => ViewportCalculator.Mode = mode;
        public void StopFollowingDiagramNodes() => ViewportCalculator.StopFollowingDiagramNodes();

        public void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ZoomToContent(transitionSpeed);
        public void ZoomToRect(Rect rect, TransitionSpeed transitionSpeed = TransitionSpeed.Medium) => ViewportCalculator.ZoomToRect(rect, transitionSpeed);

        public void EnsureRectIsVisible(Rect rect, TransitionSpeed transitionSpeed = TransitionSpeed.Medium)
            => ViewportCalculator.ContainRect(rect, transitionSpeed);

        public bool IsDiagramContentVisible() => ViewportCalculator.IsDiagramRectVisible();

        public void PinDecoration() => MiniButtonManager.PinDecoration();
        public void UnpinDecoration() => MiniButtonManager.UnpinDecoration();

        public bool TryGetDiagramNodeUi(ModelNodeId modelNodeId, out IDiagramNodeUi viewModel)
        {
            var result = TryGetDiagramNodeViewModel(modelNodeId, out var diagramNodeViewModel);
            viewModel = diagramNodeViewModel;
            return result;
        }

        private bool TryGetDiagramNodeViewModel(ModelNodeId modelNodeId, out DiagramNodeViewModel viewModel)
            => _diagramNodeToViewModelMap.TryGet(modelNodeId, out viewModel);

        private bool TryGetDiagramConnectorViewModel(ModelRelationshipId modelRelationshipId, out DiagramConnectorViewModel viewModel)
            => _diagramConnectorToViewModelMap.TryGet(modelRelationshipId, out viewModel);

        private void OnDiagramChanged(DiagramEvent @event)
        {
            if (@event.NewDiagram.IsEmpty)
            {
                ClearViewport();
                return;
            }

            foreach (var diagramChange in @event.ShapeEvents)
            {
                // All diagram-induced view model manipulation must occur on the UI thread to avoid certain race conditions.
                // (E.g. avoid the case when creating a connector view model precedes the creation of its source and target node view models.)
                EnsureUiThread(() => DispatchDiagramChangeEvent(diagramChange));
            }
        }

        private void DispatchDiagramChangeEvent(DiagramShapeEventBase diagramShapeEvent)
        {
            switch (diagramShapeEvent)
            {
                case DiagramNodeAddedEvent nodeAddedEvent:
                    AddNode(nodeAddedEvent.NewNode);
                    break;
                case DiagramNodeRemovedEvent nodeRemovedEvent:
                    RemoveNode(nodeRemovedEvent.OldNode);
                    break;
                case DiagramConnectorAddedEvent connectorAddedEvent:
                    AddConnector(connectorAddedEvent.NewConnector);
                    break;
                case DiagramConnectorRemovedEvent connectorRemovedEvent:
                    RemoveConnector(connectorRemovedEvent.OldConnector);
                    break;

                case DiagramConnectorRouteChangedEvent diagramConnectorRouteChangedEvent:
                    UpdateConnectorRoute(diagramConnectorRouteChangedEvent.NewConnector);
                    break;

                case DiagramNodeChangedEvent diagramNodeChangedEvent:
                    UpdateNode(diagramNodeChangedEvent.NewNode);
                    break;
            }
        }

        private void AddDiagram(IDiagram diagram)
        {
            // TODO: traverse the hierarchy, add nodes by parentNodeIds
            foreach (var diagramNode in diagram.Nodes)
                AddNode(diagramNode);

            foreach (var diagramConnector in diagram.Connectors)
                AddConnector(diagramConnector);
        }

        private void AddNode(IDiagramNode diagramNode)
        {
            var diagramNodeUi = (DiagramNodeViewModel)DiagramShapeUiFactory.CreateDiagramNodeUi(diagramNode, MiniButtonManager);

            diagramNodeUi.HeaderSizeChanged += OnDiagramNodeHeaderSizeChanged;
            diagramNodeUi.ShowRelatedNodesRequested += OnShowRelatedNodesRequested;
            diagramNodeUi.RelatedNodeSelectorRequested += OnEntitySelectorRequested;
            diagramNodeUi.RemoveRequested += OnRemoveDiagramNodeRequested;

            _diagramNodeToViewModelMap.Set(diagramNode.Id, diagramNodeUi);
            DiagramNodeViewModels.Add(diagramNodeUi);
        }

        private void AddConnector(IDiagramConnector diagramConnector)
        {
            var diagramConnectorUi = (DiagramConnectorViewModel)DiagramShapeUiFactory.CreateDiagramConnectorUi(diagramConnector, MiniButtonManager);

            _diagramConnectorToViewModelMap.Set(diagramConnector.Id, diagramConnectorUi);
            DiagramConnectorViewModels.Add(diagramConnectorUi);
        }

        private void OnRemoveDiagramNodeRequested(IDiagramNode diagramNode)
        {
            if (_diagramNodeToViewModelMap.TryGet(diagramNode.Id, out var diagramNodeViewModel))
                RemoveDiagramNodeRequested?.Invoke(diagramNodeViewModel);
        }

        private void RemoveNode(IDiagramNode diagramNode)
        {
            if (!TryGetDiagramNodeViewModel(diagramNode.Id, out var diagramNodeViewModel))
                return;

            diagramNodeViewModel.HeaderSizeChanged -= OnDiagramNodeHeaderSizeChanged;
            diagramNodeViewModel.ShowRelatedNodesRequested -= OnShowRelatedNodesRequested;
            diagramNodeViewModel.RelatedNodeSelectorRequested -= OnEntitySelectorRequested;
            diagramNodeViewModel.RemoveRequested -= OnRemoveDiagramNodeRequested;

            MiniButtonManager.Unfocus(diagramNodeViewModel);
            _diagramNodeToViewModelMap.Remove(diagramNode.Id);
            DiagramNodeViewModels.Remove(diagramNodeViewModel);

            diagramNodeViewModel.Dispose();
        }

        private void RemoveConnector(IDiagramConnector diagramConnector)
        {
            if (!TryGetDiagramConnectorViewModel(diagramConnector.Id, out var diagramConnectorViewModel))
                return;

            MiniButtonManager.Unfocus(diagramConnectorViewModel);
            _diagramConnectorToViewModelMap.Remove(diagramConnector.Id);
            DiagramConnectorViewModels.Remove(diagramConnectorViewModel);

            diagramConnectorViewModel.Dispose();
        }

        private void UpdateConnectorRoute([NotNull] IDiagramConnector diagramConnector)
        {
            if (!TryGetDiagramConnectorViewModel(diagramConnector.Id, out var diagramConnectorViewModel))
                return;

            diagramConnectorViewModel.Update(diagramConnector);
        }

        private void UpdateNode([NotNull] IDiagramNode diagramNode)
        {
            if (!TryGetDiagramNodeViewModel(diagramNode.Id, out var diagramNodeViewModel))
                return;

            diagramNodeViewModel.Update(diagramNode);
        }

        private void ClearViewport()
        {
            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels.ToArray())
                RemoveConnector(diagramConnectorViewModel.DiagramConnector);

            foreach (var diagramNodeViewModel in DiagramNodeViewModels.ToArray())
                RemoveNode(diagramNodeViewModel.DiagramNode);

            _diagramConnectorToViewModelMap.Clear();
            _diagramNodeToViewModelMap.Clear();
        }

        private void OnViewportTransformChanged(TransitionedTransform newTransform) => ViewportManipulation?.Invoke();

        private void OnShowRelatedNodesRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => ShowRelatedNodesRequested?.Invoke(ownerButton, modelNodes);

        private void OnEntitySelectorRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => RelatedNodeSelectorRequested?.Invoke(ownerButton, modelNodes);

        private void OnDiagramNodeHeaderSizeChanged(IDiagramNode diagramNode, Size2D newSize)
            => DiagramNodeHeaderSizeChanged?.Invoke(diagramNode, newSize);
    }
}