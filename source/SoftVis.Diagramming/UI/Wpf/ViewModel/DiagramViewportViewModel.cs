using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Geometry;
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

        private readonly Map<IDiagramNode, DiagramNodeViewModelBase> _diagramNodeToViewModelMap;
        private readonly Map<IDiagramConnector, DiagramConnectorViewModel> _diagramConnectorToViewModelMap;
        private readonly IDiagramShapeUiFactory _diagramShapeUiFactory;

        public event Action ViewportManipulation;
        public event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
        public event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        public event Action<DiagramNodeViewModelBase> RemoveDiagramNodeRequested;
        public event Action<IDiagramNode> DiagramNodeInvoked;
        public event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged;

        public DelegateCommand<IDiagramNode> DiagramNodeDoubleClickedCommand { get; }

        public DiagramViewportViewModel(IModelService modelService, IDiagramService diagramService,
            IDiagramShapeUiFactory diagramShapeUiFactory, double minZoom, double maxZoom, double initialZoom)
            : base(modelService, diagramService)
        {
            MinZoom = minZoom;
            MaxZoom = maxZoom;

            _diagramNodeToViewModelMap = new Map<IDiagramNode, DiagramNodeViewModelBase>(new DiagramNodeIdEqualityComparer());
            _diagramConnectorToViewModelMap = new Map<IDiagramConnector, DiagramConnectorViewModel>(new DiagramConnectorIdEqualityComparer());

            _diagramShapeUiFactory = diagramShapeUiFactory;
            _diagramShapeUiFactory.Initialize(modelService, this);

            ViewportCalculator = new AutoMoveViewportViewModel(modelService, diagramService, minZoom, maxZoom, initialZoom);
            DiagramNodeViewModels = new ThreadSafeObservableCollection<DiagramNodeViewModelBase>();
            DiagramConnectorViewModels = new ThreadSafeObservableCollection<DiagramConnectorViewModel>();
            MiniButtonPanelViewModel = new MiniButtonPanelViewModel();

            DiagramNodeDoubleClickedCommand = new DelegateCommand<IDiagramNode>(i => DiagramNodeInvoked?.Invoke(i));

            ViewportCalculator.TransformChanged += OnViewportTransformChanged;
            DiagramService.DiagramChanged += OnDiagramChanged;

            AddDiagram(diagramService.Diagram);
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

        public bool TryGetDiagramNodeUi(IDiagramNode diagramNode, out IDiagramNodeUi viewModel)
        {
            var result = TryGetDiagramNodeViewModel(diagramNode, out var diagramNodeViewModel);
            viewModel = diagramNodeViewModel;
            return result;
        }

        private bool TryGetDiagramNodeViewModel(IDiagramNode diagramNode, out DiagramNodeViewModelBase viewModel)
            => _diagramNodeToViewModelMap.TryGet(diagramNode, out viewModel);

        private bool TryGetDiagramConnectorViewModel(IDiagramConnector diagramConnector, out DiagramConnectorViewModel viewModel)
            => _diagramConnectorToViewModelMap.TryGet(diagramConnector, out viewModel);

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
            var diagramNodeUi = (DiagramNodeViewModelBase)_diagramShapeUiFactory.CreateDiagramNodeUi(DiagramService, diagramNode);

            diagramNodeUi.SizeChanged += OnDiagramNodeSizeChanged;
            diagramNodeUi.ShowRelatedNodesRequested += OnShowRelatedNodesRequested;
            diagramNodeUi.RelatedNodeSelectorRequested += OnEntitySelectorRequested;
            diagramNodeUi.RemoveRequested += OnRemoveDiagramNodeRequested;

            _diagramNodeToViewModelMap.Set(diagramNode, diagramNodeUi);

            if (DiagramService.TryGetContainerNode(diagramNode, out var containerNode)
                && IsNodeVisibleOnDiagram(containerNode, out var containerNodeUi))
            {
                ((IContainerDiagramNodeUi)containerNodeUi).AddChildNode(diagramNodeUi);
            }
            else
            {
                DiagramNodeViewModels.Add(diagramNodeUi);
            }
        }

        private bool IsNodeVisibleOnDiagram(IDiagramNode diagramNode, out IDiagramNodeUi diagramNodeUi)
        {
            diagramNodeUi = null;

            var result = _diagramNodeToViewModelMap.TryGet(diagramNode, out var diagramNodeViewModel);
            if (result)
                diagramNodeUi = diagramNodeViewModel;

            return result;
        }

        private void AddConnector(IDiagramConnector diagramConnector)
        {
            var diagramConnectorViewModel = (DiagramConnectorViewModel)_diagramShapeUiFactory.CreateDiagramConnectorUi(DiagramService, diagramConnector);
            DiagramConnectorViewModels.Add(diagramConnectorViewModel);

            _diagramConnectorToViewModelMap.Set(diagramConnector, diagramConnectorViewModel);
        }

        private void OnRemoveDiagramNodeRequested(IDiagramNode diagramNode)
        {
            if (_diagramNodeToViewModelMap.TryGet(diagramNode, out var diagramNodeViewModel))
                RemoveDiagramNodeRequested?.Invoke(diagramNodeViewModel);
        }

        private void RemoveNode(IDiagramNode diagramNode)
        {
            if (!TryGetDiagramNodeViewModel(diagramNode, out var diagramNodeViewModel))
                return;

            diagramNodeViewModel.SizeChanged -= OnDiagramNodeSizeChanged;
            diagramNodeViewModel.ShowRelatedNodesRequested -= OnShowRelatedNodesRequested;
            diagramNodeViewModel.RelatedNodeSelectorRequested -= OnEntitySelectorRequested;
            diagramNodeViewModel.RemoveRequested -= OnRemoveDiagramNodeRequested;

            MiniButtonPanelViewModel.Unfocus(diagramNodeViewModel);
            _diagramNodeToViewModelMap.Remove(diagramNode);
            DiagramNodeViewModels.Remove(diagramNodeViewModel);

            diagramNodeViewModel.Dispose();
        }

        private void RemoveConnector(IDiagramConnector diagramConnector)
        {
            if (!TryGetDiagramConnectorViewModel(diagramConnector, out var diagramConnectorViewModel))
                return;

            MiniButtonPanelViewModel.Unfocus(diagramConnectorViewModel);
            _diagramConnectorToViewModelMap.Remove(diagramConnector);
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

        private void OnViewportTransformChanged(TransitionedTransform newTransform)
            => ViewportManipulation?.Invoke();

        private void OnShowRelatedNodesRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => ShowRelatedNodesRequested?.Invoke(ownerButton, modelNodes);

        private void OnEntitySelectorRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
            => RelatedNodeSelectorRequested?.Invoke(ownerButton, modelNodes);

        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D newSize)
            => DiagramNodeSizeChanged?.Invoke(diagramNode, newSize);
    }
}
