using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Top level view model of the diagram control.
    /// </summary>
    public class DiagramViewModel : DiagramViewModelBase, IDisposable
    {
        private Rect _diagramContentRect;

        public DiagramViewportViewModel DiagramViewportViewModel { get; }
        public RelatedNodeListBoxViewModel RelatedNodeListBoxViewModel { get; }
        public AutoHidePopupTextViewModel PopupTextViewModel { get; }

        public DelegateCommand PreviewMouseDownCommand { get; }
        public DelegateCommand MouseDownCommand { get; }

        public event Action<IDiagramShape> ShowSourceRequested;
        public event Action<IReadOnlyList<IModelNode>> ShowModelItemsRequested;

        public DiagramViewModel(IArrangedDiagram diagram, DiagramShapeViewModelFactoryBase diagramShapeViewModelFactory,
            double minZoom, double maxZoom, double initialZoom)
            : base(diagram)
        {
            DiagramViewportViewModel = new DiagramViewportViewModel(diagram, diagramShapeViewModelFactory, minZoom, maxZoom, initialZoom);

            RelatedNodeListBoxViewModel = new RelatedNodeListBoxViewModel(diagram);
            RelatedNodeListBoxViewModel.ItemSelected += OnRelatedNodeSelected;
            RelatedNodeListBoxViewModel.Items.CollectionChanged += OnRelatedNodeCollectionChanged;

            PopupTextViewModel = new AutoHidePopupTextViewModel();

            PreviewMouseDownCommand = new DelegateCommand(OnAnyMouseDownEvent);
            MouseDownCommand = new DelegateCommand(OnUnhandledMouseDownEvent);

            SubscribeToDiagramEvents();
            SubscribeToViewportEvents();
        }

        public void Dispose()
        {
            RelatedNodeListBoxViewModel.ItemSelected -= OnRelatedNodeSelected;
            RelatedNodeListBoxViewModel.Items.CollectionChanged -= OnRelatedNodeCollectionChanged;
            RelatedNodeListBoxViewModel.Dispose();

            UnsubscribeFromDiagramEvents();
            UnsubscribeFromViewportEvents();

            DiagramViewportViewModel.Dispose();
        }

        public IEnumerable<DiagramNodeViewModelBase> DiagramNodeViewModels => DiagramViewportViewModel.DiagramNodeViewModels;
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

        public void FollowDiagramNodes(IReadOnlyList<IDiagramNode> diagramNodes)
        {
            var autoMoveMode = Diagram.Nodes.Count > diagramNodes.Count
                ? ViewportAutoMoveMode.Contain
                : ViewportAutoMoveMode.Center;

            DiagramViewportViewModel.SetFollowDiagramNodesMode(autoMoveMode);
            DiagramViewportViewModel.FollowDiagramNodes(diagramNodes);
        }

        public void StopFollowingDiagramNodes() => DiagramViewportViewModel.StopFollowingDiagramNodes();

        public void KeepDiagramCentered()
        {
            DiagramViewportViewModel.SetFollowDiagramNodesMode(ViewportAutoMoveMode.Center);
            DiagramViewportViewModel.FollowDiagramNodes(Diagram.Nodes);
        }

        public void ZoomToContent() => DiagramViewportViewModel.ZoomToContent();
        public void ZoomToRect(Rect rect) => DiagramViewportViewModel.ZoomToRect(rect);
        public void EnsureRectIsVisible(Rect rect) => DiagramViewportViewModel.EnsureRectIsVisible(rect);
        public bool IsDiagramContentVisible() => DiagramViewportViewModel.IsDiagramContentVisible();

        public void ShowPopupMessage(string text, TimeSpan hideAfter = default(TimeSpan))
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
            DiagramViewportViewModel.DiagramShapeRemoveRequested += OnDiagramShapeRemoveRequested;
            DiagramViewportViewModel.ShowSourceRequested += OnShowSourceRequested;
        }

        private void UnsubscribeFromViewportEvents()
        {
            DiagramViewportViewModel.ViewportManipulation -= OnViewportManipulation;
            DiagramViewportViewModel.RelatedNodeSelectorRequested -= OnRelatedNodeSelectorRequested;
            DiagramViewportViewModel.ShowRelatedNodesRequested -= OnShowRelatedNodesRequested;
            DiagramViewportViewModel.DiagramShapeRemoveRequested -= OnDiagramShapeRemoveRequested;
            DiagramViewportViewModel.ShowSourceRequested -= OnShowSourceRequested;
        }

        private void OnDiagramShapeRemoveRequested(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            DiagramViewportViewModel.StopFollowingDiagramNodes();
            if (RelatedNodeListBoxViewModel.OwnerDiagramShape == diagramShapeViewModel)
                HideRelatedNodeListBox();
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
                    var diagramNodes = Diagram.ShowModelItems(modelNodes).OfType<IDiagramNode>().ToArray();
                    FollowDiagramNodes(diagramNodes);
                    break;
                default:
                    HideRelatedNodeListBox();
                    ShowModelItemsRequested?.Invoke(modelNodes);
                    break;
            }
        }

        private void OnRelatedNodeSelected(IModelNode selectedNode)
        {
            StopFollowingDiagramNodes();
            Diagram.ShowModelItem(selectedNode);
        }

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            ShowSourceRequested?.Invoke(diagramShape);
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

        private void OnCleared()
        {
            HideAllWidgets();
            UpdateDiagramContentRect();
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

        private void UpdateDiagramContentRect()
        {
            DiagramContentRect = Diagram.ContentRect.ToWpf();
        }

        private void OnDiagramShapeAdded(IDiagramShape diagramShape) => UpdateDiagramContentRect();
        private void OnDiagramShapeRemoved(IDiagramShape diagramShape) => UpdateDiagramContentRect();
        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D oldSize, Size2D newSize) => UpdateDiagramContentRect();
        private void OnDiagramNodeCenterChanged(IDiagramNode diagramNode, Point2D oldCenter, Point2D newCenter) => UpdateDiagramContentRect();
        private void OnDiagramConnectorRouteChanged(IDiagramConnector diagramConnector, Route oldRoute, Route newRoute) => UpdateDiagramContentRect();

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += OnDiagramShapeAdded;
            Diagram.ShapeRemoved += OnDiagramShapeRemoved;
            Diagram.NodeSizeChanged += OnDiagramNodeSizeChanged;
            Diagram.NodeCenterChanged += OnDiagramNodeCenterChanged;
            Diagram.ConnectorRouteChanged += OnDiagramConnectorRouteChanged;
            Diagram.DiagramCleared += OnCleared;
        }

        private void UnsubscribeFromDiagramEvents()
        {
            Diagram.ShapeAdded -= OnDiagramShapeAdded;
            Diagram.ShapeRemoved -= OnDiagramShapeRemoved;
            Diagram.NodeSizeChanged -= OnDiagramNodeSizeChanged;
            Diagram.NodeCenterChanged -= OnDiagramNodeCenterChanged;
            Diagram.ConnectorRouteChanged -= OnDiagramConnectorRouteChanged;
            Diagram.DiagramCleared -= OnCleared;
        }
    }
}
