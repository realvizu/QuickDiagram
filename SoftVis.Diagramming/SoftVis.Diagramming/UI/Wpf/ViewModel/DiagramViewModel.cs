using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
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
        public RelatedEntityListBoxViewModel RelatedEntityListBoxViewModel { get; }
        public AutoHidePopupTextViewModel PopupTextViewModel { get; }

        public DelegateCommand PreviewMouseDownCommand { get; }
        public DelegateCommand MouseDownCommand { get; }

        public event Action<IDiagramShape> ShowSourceRequested;
        public event Action<List<IModelEntity>> ShowModelItemsRequested;

        public DiagramViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            : base(diagram)
        {
            DiagramViewportViewModel = new DiagramViewportViewModel(diagram, minZoom, maxZoom, initialZoom);

            RelatedEntityListBoxViewModel = new RelatedEntityListBoxViewModel(diagram);
            RelatedEntityListBoxViewModel.ItemSelected += OnRelatedEntitySelected;
            RelatedEntityListBoxViewModel.Items.CollectionChanged += OnRelatedEntityCollectionChanged;

            PopupTextViewModel = new AutoHidePopupTextViewModel();

            PreviewMouseDownCommand = new DelegateCommand(OnAnyMouseDownEvent);
            MouseDownCommand = new DelegateCommand(OnUnhandledMouseDownEvent);

            SubscribeToDiagramEvents();
            SubscribeToViewportEvents();
        }

        public void Dispose()
        {
            RelatedEntityListBoxViewModel.ItemSelected -= OnRelatedEntitySelected;
            RelatedEntityListBoxViewModel.Items.CollectionChanged -= OnRelatedEntityCollectionChanged;
            RelatedEntityListBoxViewModel.Dispose();

            UnsubscribeFromDiagramEvents();
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

        public void FollowDiagramNodes(IEnumerable<IDiagramNode> diagramNodes) => DiagramViewportViewModel.FollowDiagramNodes(diagramNodes);
        public void StopFollowingDiagramNodes() => DiagramViewportViewModel.StopFollowingDiagramNodes();
        public void ZoomToContent() => DiagramViewportViewModel.ZoomToContent();
        public void ZoomToRect(Rect rect) => DiagramViewportViewModel.ZoomToRect(rect);
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
            DiagramViewportViewModel.ShowEntitySelectorRequested += OnShowRelatedEntitySelectorRequested;
            DiagramViewportViewModel.ShowRelatedEntitiesRequested += OnShowRelatedEntitiesRequested;
            DiagramViewportViewModel.DiagramShapeRemoveRequested += OnDiagramShapeRemoveRequested;
            DiagramViewportViewModel.ShowSourceRequested += OnShowSourceRequested;
        }

        private void UnsubscribeFromViewportEvents()
        {
            DiagramViewportViewModel.ViewportManipulation -= OnViewportManipulation;
            DiagramViewportViewModel.ShowEntitySelectorRequested -= OnShowRelatedEntitySelectorRequested;
            DiagramViewportViewModel.ShowRelatedEntitiesRequested -= OnShowRelatedEntitiesRequested;
            DiagramViewportViewModel.DiagramShapeRemoveRequested -= OnDiagramShapeRemoveRequested;
            DiagramViewportViewModel.ShowSourceRequested -= OnShowSourceRequested;
        }

        private void OnDiagramShapeRemoveRequested(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            DiagramViewportViewModel.StopFollowingDiagramNodes();
            if (RelatedEntityListBoxViewModel.OwnerDiagramShape == diagramShapeViewModel)
                HideRelatedEntityListBox();
        }

        private void OnShowRelatedEntitySelectorRequested(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel, IEnumerable<IModelEntity> modelEntities)
        {
            DiagramViewportViewModel.PinDecoration();
            RelatedEntityListBoxViewModel.Show(diagramNodeButtonViewModel, modelEntities);
        }

        private void OnShowRelatedEntitiesRequested(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel, List<IModelEntity> modelEntities)
        {
            switch (modelEntities.Count)
            {
                case 0:
                    return;
                case 1:
                    var diagramNodes = Diagram.ShowItems(modelEntities);
                    FollowDiagramNodes(diagramNodes.OfType<IDiagramNode>());
                    break;
                default:
                    HideRelatedEntityListBox();
                    ShowModelItemsRequested?.Invoke(modelEntities);
                    break;
            }
        }

        private void OnRelatedEntitySelected(IModelEntity selectedEntity)
        {
            DiagramViewportViewModel.StopFollowingDiagramNodes();
            Diagram.ShowItem(selectedEntity);
        }

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            ShowSourceRequested?.Invoke(diagramShape);
        }

        private void OnRelatedEntityCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    if (!RelatedEntityListBoxViewModel.Items.Any())
                        HideRelatedEntityListBox();
                    break;
            }
        }

        private void OnAnyMouseDownEvent()
        {
            HidePopupText();
        }

        private void OnUnhandledMouseDownEvent()
        {
            HideRelatedEntityListBox();
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

        private void HideRelatedEntityListBox()
        {
            DiagramViewportViewModel.UnpinDecoration();
            RelatedEntityListBoxViewModel.Hide();
        }

        private void HidePopupText()
        {
            PopupTextViewModel.Hide();
        }

        private void HideAllWidgets()
        {
            HideRelatedEntityListBox();
            HidePopupText();
        }

        private void UpdateDiagramContentRect()
        {
            DiagramContentRect = Diagram.ContentRect.ToWpf();
        }

        private void OnDiagramShapeAdded(IDiagramShape diagramShape) => UpdateDiagramContentRect();
        private void OnDiagramShapeRemoved(IDiagramShape diagramShape) => UpdateDiagramContentRect();
        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D oldSize, Size2D newSize) => UpdateDiagramContentRect();
        private void OnDiagramNodeTopLeftChanged(IDiagramNode diagramNode, Point2D oldTopLeft, Point2D newTopLeft) => UpdateDiagramContentRect();
        private void OnDiagramConnectorRouteChanged(IDiagramConnector diagramConnector, Route oldRoute, Route newRoute) => UpdateDiagramContentRect();

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += OnDiagramShapeAdded;
            Diagram.ShapeRemoved += OnDiagramShapeRemoved;
            Diagram.NodeSizeChanged += OnDiagramNodeSizeChanged;
            Diagram.NodeTopLeftChanged += OnDiagramNodeTopLeftChanged;
            Diagram.ConnectorRouteChanged += OnDiagramConnectorRouteChanged;
            Diagram.DiagramCleared += OnCleared;
        }

        private void UnsubscribeFromDiagramEvents()
        {
            Diagram.ShapeAdded -= OnDiagramShapeAdded;
            Diagram.ShapeRemoved -= OnDiagramShapeRemoved;
            Diagram.NodeSizeChanged -= OnDiagramNodeSizeChanged;
            Diagram.NodeTopLeftChanged -= OnDiagramNodeTopLeftChanged;
            Diagram.ConnectorRouteChanged -= OnDiagramConnectorRouteChanged;
            Diagram.DiagramCleared -= OnCleared;
        }
    }
}
