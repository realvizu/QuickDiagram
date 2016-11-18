using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Top level view model of the diagram control.
    /// </summary>
    public class DiagramViewModel : DiagramViewModelBase
    {
        private Rect _diagramContentRect;
        private Rect _exportImageRect;

        public event DiagramImageRequestedEventHandler DiagramImageExportRequested;

        public DiagramViewportViewModel DiagramViewportViewModel { get; }
        public RelatedEntityListBoxViewModel RelatedEntityListBoxViewModel { get; }

        public DiagramViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            : base(diagram)
        {
            DiagramViewportViewModel = new DiagramViewportViewModel(diagram, minZoom, maxZoom, initialZoom);

            RelatedEntityListBoxViewModel = new RelatedEntityListBoxViewModel();
            RelatedEntityListBoxViewModel.ItemSelected += OnRelatedEntitySelected;

            SubscribeToDiagramEvents();
            SubscribeToViewportEvents();
        }

        public Rect DiagramContentRect
        {
            get { return _diagramContentRect; }
            set
            {
                _diagramContentRect = value;
                OnPropertyChanged();
            }
        }

        public Rect ExportImageRect
        {
            get { return _exportImageRect; }
            set
            {
                _exportImageRect = value;
                OnPropertyChanged();
            }
        }

        private void SubscribeToViewportEvents()
        {
            DiagramViewportViewModel.ShowEntitySelectorRequested += ShowRelatedEntitySelector;
            DiagramViewportViewModel.HideEntitySelectorRequested += HideRelatedEntitySelector;
            DiagramViewportViewModel.DiagramShapeRemoveRequested += OnDiagramShapeRemoveRequested;
        }

        private void OnDiagramShapeRemoveRequested(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            if (RelatedEntityListBoxViewModel.OwnerDiagramShape == diagramShapeViewModel)
                HideRelatedEntitySelector();
        }

        private void ShowRelatedEntitySelector(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel, IEnumerable<IModelEntity> modelEntities)
        {
            DiagramViewportViewModel.PinDecoration();
            RelatedEntityListBoxViewModel.Show(diagramNodeButtonViewModel, modelEntities);
        }

        private void HideRelatedEntitySelector()
        {
            RelatedEntityListBoxViewModel.Hide();
            DiagramViewportViewModel.UnpinDecoration();
        }

        public void ZoomToContent()
        {
            DiagramViewportViewModel.ZoomToContent();
        }

        public void GetDiagramImage(double dpi, double margin, Action<BitmapSource> imageCreatedCallback)
        {
            ExportImageRect = CalculateExportImageRect(DiagramContentRect, margin);
            if (!IsValidForImageExport(ExportImageRect))
                throw new InvalidOperationException($"ExportImageRect is invalid: {ExportImageRect}");

            var exportRectInDiagramImageControl = new Rect(ExportImageRect.Size);
            DiagramImageExportRequested?.Invoke(exportRectInDiagramImageControl, dpi, imageCreatedCallback);
        }

        private static bool IsValidForImageExport(Rect rect)
        {
            var size = rect.Size;

            return size.Width > 0 
                && size.Height > 0 
                && !double.IsInfinity(size.Width) 
                && !double.IsInfinity(size.Height);
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += i => UpdateDiagramContentRect();
            Diagram.ShapeRemoved += i => UpdateDiagramContentRect();
            Diagram.Cleared += OnCleared;
            Diagram.NodeSizeChanged += (i, j, k) => UpdateDiagramContentRect();
            Diagram.NodeTopLeftChanged += (i, j, k) => UpdateDiagramContentRect();
            Diagram.ConnectorRouteChanged += (i, j, k) => UpdateDiagramContentRect();
        }

        private void OnCleared()
        {
            RelatedEntityListBoxViewModel.Hide();
            UpdateDiagramContentRect();
        }

        private void UpdateDiagramContentRect()
        {
            DiagramContentRect = Diagram.ContentRect.ToWpf();
        }

        private void OnRelatedEntitySelected(IModelEntity selectedEntity)
        {
            Diagram.ShowItem(selectedEntity);

            var remainingEntities = RelatedEntityListBoxViewModel.Items.Except(selectedEntity.ToEnumerable()).ToList();

            if (remainingEntities.Any())
                RelatedEntityListBoxViewModel.Items = remainingEntities;
            else
                HideRelatedEntitySelector();
        }

        private static Rect CalculateExportImageRect(Rect diagramContextRect, double margin)
        {
            var diagramContentSize = diagramContextRect.Size;
            var size = new Size(diagramContentSize.Width + 2 * margin, diagramContentSize.Height + 2 * margin);
            var location = new Point(diagramContextRect.Left - margin, diagramContextRect.Top - margin);
            return new Rect(location, size);
        }
    }
}
