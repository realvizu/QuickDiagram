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
        public ThreadSafeObservableCollection<DiagramNodeViewModel> DiagramNodeViewModels { get; }
        public ThreadSafeObservableCollection<DiagramConnectorViewModel> DiagramConnectorViewModels { get; }
        public MiniButtonPanelViewModel MiniButtonPanelViewModel { get; }

        private readonly Map<ModelNodeId, DiagramNodeViewModel> _diagramNodeToViewModelMap;
        private readonly Map<ModelRelationshipId, DiagramConnectorViewModel> _diagramConnectorToViewModelMap;
        private readonly IDiagramShapeUiFactory _diagramShapeUiFactory;

        public event Action ViewportManipulation;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event Action<DiagramNodeViewModel> RemoveDiagramNodeRequested;
        public event Action<IDiagramNode> DiagramNodeInvoked;
        public event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged;
        public event Action<IDiagramNode, Size2D> DiagramNodePayloadAreaSizeChanged;

        public DelegateCommand<IDiagramNode> DiagramNodeDoubleClickedCommand { get; }

        public DiagramViewportViewModel(
            IModelService modelService,
            IDiagramService diagramService,
            IDiagramShapeUiFactory diagramShapeUiFactory,
            double minZoom,
            double maxZoom,
            double initialZoom)
            : base(modelService, diagramService)
        {
            MinZoom = minZoom;
            MaxZoom = maxZoom;

            _diagramNodeToViewModelMap = new Map<ModelNodeId, DiagramNodeViewModel>();
            _diagramConnectorToViewModelMap = new Map<ModelRelationshipId, DiagramConnectorViewModel>();

            _diagramShapeUiFactory = diagramShapeUiFactory;
            _diagramShapeUiFactory.Initialize(modelService, this);

            ViewportCalculator = new AutoMoveViewportViewModel(modelService, diagramService, minZoom, maxZoom, initialZoom);
            DiagramNodeViewModels = new ThreadSafeObservableCollection<DiagramNodeViewModel>();
            DiagramConnectorViewModels = new ThreadSafeObservableCollection<DiagramConnectorViewModel>();
            MiniButtonPanelViewModel = new MiniButtonPanelViewModel();

            DiagramNodeDoubleClickedCommand = new DelegateCommand<IDiagramNode>(i => DiagramNodeInvoked?.Invoke(i));

            ViewportCalculator.TransformChanged += OnViewportTransformChanged;
            DiagramService.DiagramChanged += OnDiagramChanged;

            AddDiagram(diagramService.LatestDiagram);
        }

        public override void Dispose()
        {
            base.Dispose();

            ViewportCalculator.TransformChanged -= OnViewportTransformChanged;
            DiagramService.DiagramChanged -= OnDiagramChanged;

            ViewportCalculator.Dispose();
            MiniButtonPanelViewModel.Dispose();

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

        public void PinDecoration() => MiniButtonPanelViewModel.PinDecoration();
        public void UnpinDecoration() => MiniButtonPanelViewModel.UnpinDecoration();

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
            var diagramNodeUi = (DiagramNodeViewModel)_diagramShapeUiFactory.CreateDiagramNodeUi(DiagramService, diagramNode, MiniButtonPanelViewModel);

            diagramNodeUi.SizeChanged += OnDiagramNodeSizeChanged;
            diagramNodeUi.PayloadAreaSizeChanged += OnDiagramNodePayloadAreaSizeChanged;
            diagramNodeUi.ShowRelatedNodesRequested += OnShowRelatedNodesRequested;
            diagramNodeUi.RelatedNodeSelectorRequested += OnEntitySelectorRequested;
            diagramNodeUi.RemoveRequested += OnRemoveDiagramNodeRequested;

            _diagramNodeToViewModelMap.Set(diagramNode.Id, diagramNodeUi);
            DiagramNodeViewModels.Add(diagramNodeUi);
        }

        private void AddConnector(IDiagramConnector diagramConnector)
        {
            var diagramConnectorUi = (DiagramConnectorViewModel)_diagramShapeUiFactory.CreateDiagramConnectorUi(DiagramService, diagramConnector);

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

            diagramNodeViewModel.SizeChanged -= OnDiagramNodeSizeChanged;
            diagramNodeViewModel.PayloadAreaSizeChanged -= OnDiagramNodePayloadAreaSizeChanged;
            diagramNodeViewModel.ShowRelatedNodesRequested -= OnShowRelatedNodesRequested;
            diagramNodeViewModel.RelatedNodeSelectorRequested -= OnEntitySelectorRequested;
            diagramNodeViewModel.RemoveRequested -= OnRemoveDiagramNodeRequested;

            MiniButtonPanelViewModel.Unfocus(diagramNodeViewModel);
            _diagramNodeToViewModelMap.Remove(diagramNode.Id);
            DiagramNodeViewModels.Remove(diagramNodeViewModel);

            diagramNodeViewModel.Dispose();
        }

        private void RemoveConnector(IDiagramConnector diagramConnector)
        {
            if (!TryGetDiagramConnectorViewModel(diagramConnector.Id, out var diagramConnectorViewModel))
                return;

            MiniButtonPanelViewModel.Unfocus(diagramConnectorViewModel);
            _diagramConnectorToViewModelMap.Remove(diagramConnector.Id);
            DiagramConnectorViewModels.Remove(diagramConnectorViewModel);

            diagramConnectorViewModel.Dispose();
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

        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D newSize) => DiagramNodeSizeChanged?.Invoke(diagramNode, newSize);

        private void OnDiagramNodePayloadAreaSizeChanged(IDiagramNode diagramNode, Size2D newSize)
            => DiagramNodePayloadAreaSizeChanged?.Invoke(diagramNode, newSize);
    }
}