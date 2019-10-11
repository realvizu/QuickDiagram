using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI.Wpf.Commands;
using Codartis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Top level view model of the diagram control.
    /// </summary>
    public class DiagramViewModel : ModelObserverViewModelBase
    {
        private IDiagram _lastDiagram;
        private Rect _diagramContentRect;

        protected IDiagramShapeUiFactory DiagramShapeUiFactory { get; }

        public DiagramViewportViewModel DiagramViewportViewModel { get; }
        public RelatedNodeListBoxViewModel RelatedNodeListBoxViewModel { get; }
        public AutoHidePopupTextViewModel PopupTextViewModel { get; }

        public DelegateCommand PreviewMouseDownCommand { get; }
        public DelegateCommand MouseDownCommand { get; }

        public event ShowModelItemsEventHandler ShowModelItemsRequested;
        public event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged;
        public event Action<IDiagramNode, Size2D> DiagramNodePayloadAreaSizeChanged;
        public event Action<IDiagramNode> DiagramNodeInvoked;
        public event Action<IDiagramNode> RemoveDiagramNodeRequested;

        public DiagramViewModel(
            IModelService modelService,
            IDiagramService diagramService,
            IDiagramShapeUiFactory diagramShapeUiFactory,
            double minZoom,
            double maxZoom,
            double initialZoom)
            : base(modelService, diagramService)
        {
            DiagramShapeUiFactory = diagramShapeUiFactory;

            DiagramViewportViewModel = new DiagramViewportViewModel(
                ModelService,
                DiagramService,
                diagramShapeUiFactory,
                minZoom,
                maxZoom,
                initialZoom);

            RelatedNodeListBoxViewModel = new RelatedNodeListBoxViewModel(ModelService, DiagramService);
            RelatedNodeListBoxViewModel.ItemSelected += OnRelatedNodeSelected;
            RelatedNodeListBoxViewModel.Items.CollectionChanged += OnRelatedNodeCollectionChanged;

            PopupTextViewModel = new AutoHidePopupTextViewModel();

            PreviewMouseDownCommand = new DelegateCommand(OnAnyMouseDownEvent);
            MouseDownCommand = new DelegateCommand(OnUnhandledMouseDownEvent);

            DiagramService.DiagramChanged += OnDiagramChanged;

            SubscribeToViewportEvents();

            _lastDiagram = DiagramService.LatestDiagram;
        }

        public override void Dispose()
        {
            base.Dispose();

            RelatedNodeListBoxViewModel.ItemSelected -= OnRelatedNodeSelected;
            RelatedNodeListBoxViewModel.Items.CollectionChanged -= OnRelatedNodeCollectionChanged;
            RelatedNodeListBoxViewModel.Dispose();

            DiagramService.DiagramChanged -= OnDiagramChanged;

            UnsubscribeFromViewportEvents();

            DiagramViewportViewModel.Dispose();
        }

        public IEnumerable<DiagramNodeViewModel> DiagramNodeViewModels => DiagramViewportViewModel.DiagramNodeViewModels;
        public IEnumerable<DiagramConnectorViewModel> DiagramConnectorViewModelsModels => DiagramViewportViewModel.DiagramConnectorViewModels;

        public Rect DiagramContentRect
        {
            get { return _diagramContentRect; }
            set
            {
                _diagramContentRect = value;
                OnPropertyChanged();
            }
        }

        public void FollowDiagramNodes(IReadOnlyList<ModelNodeId> nodeIds)
        {
            var autoMoveMode = _lastDiagram.Nodes.Count > nodeIds.Count
                ? ViewportAutoMoveMode.Contain
                : ViewportAutoMoveMode.Center;

            DiagramViewportViewModel.SetFollowDiagramNodesMode(autoMoveMode);
            DiagramViewportViewModel.FollowDiagramNodes(nodeIds);
        }

        public void StopFollowingDiagramNodes()
        {
            DiagramViewportViewModel.StopFollowingDiagramNodes();
        }

        public void KeepDiagramCentered()
        {
            DiagramViewportViewModel.SetFollowDiagramNodesMode(ViewportAutoMoveMode.Center);
            DiagramViewportViewModel.FollowDiagramNodes(_lastDiagram.Nodes.Select(i => i.Id).ToArray());
        }

        public void ZoomToContent() => DiagramViewportViewModel.ZoomToContent();
        public void ZoomToRect(Rect rect) => DiagramViewportViewModel.ZoomToRect(rect);
        public void EnsureRectIsVisible(Rect rect) => DiagramViewportViewModel.EnsureRectIsVisible(rect);
        public bool IsDiagramContentVisible() => DiagramViewportViewModel.IsDiagramContentVisible();

        public void ShowPopupMessage(string text, TimeSpan hideAfter = default)
        {
            PopupTextViewModel.Text = text;
            PopupTextViewModel.AutoHideAfter = hideAfter;
            PopupTextViewModel.Show();
        }

        private void SubscribeToViewportEvents()
        {
            DiagramViewportViewModel.ViewportManipulation += OnViewportManipulation;
            DiagramViewportViewModel.RelatedNodeSelectorRequested += OnRelatedNodeSelectorRequested;
            DiagramViewportViewModel.ShowRelatedNodesRequested += OnShowRelatedNodesRequested;
            DiagramViewportViewModel.RemoveDiagramNodeRequested += OnRemoveDiagramNodeRequested;
            DiagramViewportViewModel.DiagramNodeInvoked += OnDiagramNodeInvoked;
            DiagramViewportViewModel.DiagramNodeSizeChanged += OnDiagramNodeSizeChanged;
            DiagramViewportViewModel.DiagramNodePayloadAreaSizeChanged += OnDiagramNodePayloadAreaSizeChanged;
        }

        private void UnsubscribeFromViewportEvents()
        {
            DiagramViewportViewModel.ViewportManipulation -= OnViewportManipulation;
            DiagramViewportViewModel.RelatedNodeSelectorRequested -= OnRelatedNodeSelectorRequested;
            DiagramViewportViewModel.ShowRelatedNodesRequested -= OnShowRelatedNodesRequested;
            DiagramViewportViewModel.RemoveDiagramNodeRequested -= OnRemoveDiagramNodeRequested;
            DiagramViewportViewModel.DiagramNodeInvoked -= OnDiagramNodeInvoked;
            DiagramViewportViewModel.DiagramNodeSizeChanged -= OnDiagramNodeSizeChanged;
            DiagramViewportViewModel.DiagramNodePayloadAreaSizeChanged -= OnDiagramNodePayloadAreaSizeChanged;
        }

        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D newSize)
        {
            if (newSize.IsDefined)
                DiagramNodeSizeChanged?.Invoke(diagramNode, newSize);
        }

        private void OnDiagramNodePayloadAreaSizeChanged(IDiagramNode diagramNode, Size2D newSize)
        {
            if (newSize.IsDefined)
                DiagramNodePayloadAreaSizeChanged?.Invoke(diagramNode, newSize);
        }

        private void OnRemoveDiagramNodeRequested(DiagramNodeViewModel diagramNodeViewModel)
        {
            DiagramViewportViewModel.StopFollowingDiagramNodes();
            if (RelatedNodeListBoxViewModel.OwnerDiagramShape == diagramNodeViewModel)
                HideRelatedNodeListBox();

            RemoveDiagramNodeRequested?.Invoke(diagramNodeViewModel.DiagramNode);
        }

        private void OnRelatedNodeSelectorRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
        {
            DiagramViewportViewModel.PinDecoration();
            RelatedNodeListBoxViewModel.Show(ownerButton, modelNodes);
        }

        private void OnShowRelatedNodesRequested(RelatedNodeMiniButtonViewModel ownerButton, IReadOnlyList<IModelNode> modelNodes)
        {
            switch (modelNodes.Count)
            {
                case 0:
                    return;
                case 1:
                    ShowModelItemsRequested?.Invoke(modelNodes, followNewDiagramNodes: true);
                    break;
                default:
                    HideRelatedNodeListBox();
                    ShowModelItemsRequested?.Invoke(modelNodes, followNewDiagramNodes: true);
                    break;
            }
        }

        private void OnRelatedNodeSelected(IModelNode modelNode)
        {
            StopFollowingDiagramNodes();
            ShowModelItemsRequested?.Invoke(new[] { modelNode }, followNewDiagramNodes: false);
        }

        private void OnDiagramNodeInvoked(IDiagramNode diagramNode)
        {
            DiagramNodeInvoked?.Invoke(diagramNode);
        }

        private void OnRelatedNodeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    if (!RelatedNodeListBoxViewModel.Items.Any())
                        HideRelatedNodeListBox();
                    break;
            }
        }

        private void OnAnyMouseDownEvent()
        {
            HidePopupText();
        }

        private void OnUnhandledMouseDownEvent()
        {
            HideRelatedNodeListBox();
        }

        private void OnViewportManipulation()
        {
            HideAllWidgets();
        }

        private void HideRelatedNodeListBox()
        {
            DiagramViewportViewModel.UnpinDecoration();
            RelatedNodeListBoxViewModel.Hide();
        }

        private void HidePopupText()
        {
            PopupTextViewModel.Hide();
        }

        private void HideAllWidgets()
        {
            HideRelatedNodeListBox();
            HidePopupText();
        }

        private void UpdateDiagramContentRect(IDiagram diagram)
        {
            DiagramContentRect = diagram.Rect.ToWpf();
        }

        private void OnDiagramChanged(DiagramEvent @event)
        {
            _lastDiagram = @event.NewDiagram;

            if (_lastDiagram.IsEmpty)
                HideAllWidgets();

            UpdateDiagramContentRect(@event.NewDiagram);
        }
    }
}