using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Top level view model of the diagram control.
    /// </summary>
    public class DiagramViewModel : DiagramViewModelBase
    {
        private Rect _diagramContentRect;

        public DiagramViewportViewModel DiagramViewportViewModel { get; }
        public RelatedEntityListBoxViewModel RelatedEntityListBoxViewModel { get; }
        public AutoHidePopupTextViewModel PopupTextViewModel { get; }

        public DiagramViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            : base(diagram)
        {
            DiagramViewportViewModel = new DiagramViewportViewModel(diagram, minZoom, maxZoom, initialZoom);

            RelatedEntityListBoxViewModel = new RelatedEntityListBoxViewModel();
            RelatedEntityListBoxViewModel.ItemSelected += OnRelatedEntitySelected;

            PopupTextViewModel = new AutoHidePopupTextViewModel();

            SubscribeToDiagramEvents();
            SubscribeToViewportEvents();
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

        public void ZoomToContent()
        {
            DiagramViewportViewModel.ZoomToContent();
        }

        public void ShowPopupMessage(string text, TimeSpan hideAfter = default(TimeSpan))
        {
            PopupTextViewModel.Text = text;
            PopupTextViewModel.AutoHideAfter = hideAfter;
            PopupTextViewModel.Show();
        }

        private void SubscribeToViewportEvents()
        {
            DiagramViewportViewModel.InputReceived += OnViewportInputReceived;
            DiagramViewportViewModel.ShowEntitySelectorRequested += OnShowRelatedEntitySelectorRequested;
            DiagramViewportViewModel.DiagramShapeRemoveRequested += OnDiagramShapeRemoveRequested;
        }

        private void OnDiagramShapeRemoveRequested(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            if (RelatedEntityListBoxViewModel.OwnerDiagramShape == diagramShapeViewModel)
                OnViewportInputReceived();
        }

        private void OnShowRelatedEntitySelectorRequested(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel, IEnumerable<IModelEntity> modelEntities)
        {
            DiagramViewportViewModel.PinDecoration();
            RelatedEntityListBoxViewModel.Show(diagramNodeButtonViewModel, modelEntities);
        }

        private void OnViewportInputReceived()
        {
            HideAllWidgets();
        }

        private void OnCleared()
        {
            HideAllWidgets();
            UpdateDiagramContentRect();
        }

        private void OnRelatedEntitySelected(IModelEntity selectedEntity)
        {
            Diagram.ShowItem(selectedEntity);

            var remainingEntities = RelatedEntityListBoxViewModel.Items.Except(selectedEntity.ToEnumerable()).ToList();

            if (remainingEntities.Any())
                RelatedEntityListBoxViewModel.Items = remainingEntities;
            else
                HideAllWidgets();
        }

        private void HideAllWidgets()
        {
            DiagramViewportViewModel.UnpinDecoration();
            RelatedEntityListBoxViewModel.Hide();
            PopupTextViewModel.Hide();
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += i => UpdateDiagramContentRect();
            Diagram.ShapeRemoved += i => UpdateDiagramContentRect();
            Diagram.DiagramCleared += OnCleared;
            Diagram.NodeSizeChanged += (i, j, k) => UpdateDiagramContentRect();
            Diagram.NodeTopLeftChanged += (i, j, k) => UpdateDiagramContentRect();
            Diagram.ConnectorRouteChanged += (i, j, k) => UpdateDiagramContentRect();
        }

        private void UpdateDiagramContentRect()
        {
            DiagramContentRect = Diagram.ContentRect.ToWpf();
        }
    }
}
