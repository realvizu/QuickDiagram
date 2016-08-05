using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Top level view model of the diagram control.
    /// </summary>
    public class DiagramViewModel : DiagramViewModelBase
    {
        private Rect _diagramContentRect;

        public event DiagramImageRequestedEventHandler DiagramImageExportRequested;

        public DiagramViewportViewModel DiagramViewportViewModel { get; }
        public BubbleSelectorViewModel<IModelEntity> RelatedEntitySelectorViewModel { get; }

        public DiagramViewModel(IDiagram diagram, double minZoom, double maxZoom, double initialZoom)
            :base(diagram)
        {
            DiagramViewportViewModel = new DiagramViewportViewModel(diagram, minZoom, maxZoom, initialZoom);

            RelatedEntitySelectorViewModel = new BubbleSelectorViewModel<IModelEntity>(new Size(200, 100));
            RelatedEntitySelectorViewModel.ItemSelected += AddModelEntityToDiagram;

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

        private void SubscribeToViewportEvents()
        {
            DiagramViewportViewModel.EntitySelectorRequested += ShowRelatedEntitySelector;
            DiagramViewportViewModel.ViewportChanged += HideRelatedEntitySelector;
        }

        private void ShowRelatedEntitySelector(Point attachPointInScreenSpace, HandleOrientation handleOrientation,
            IEnumerable<IModelEntity> modelEntities)
        {
            DiagramViewportViewModel.PinDecoration();
            RelatedEntitySelectorViewModel.Show(attachPointInScreenSpace, handleOrientation, modelEntities);
        }

        private void HideRelatedEntitySelector()
        {
            RelatedEntitySelectorViewModel.Hide();
            DiagramViewportViewModel.UnpinDecoration();
        }

        public void ZoomToContent()
        {
            DiagramViewportViewModel.ZoomToContent();
        }

        public void GetDiagramImage(double dpi, Action<BitmapSource> imageCreatedCallback)
        {
            DiagramImageExportRequested?.Invoke(dpi, imageCreatedCallback);
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += (o, e) => UpdateDiagramContentRect();
            Diagram.ShapeMoved += (o, e) => UpdateDiagramContentRect();
            Diagram.ShapeRemoved += (o, e) => UpdateDiagramContentRect();
            Diagram.Cleared += (o, e) => UpdateDiagramContentRect();
        }

        private void UpdateDiagramContentRect()
        {
            DiagramContentRect = Diagram.ContentRect.ToWpf();
        }

        private void AddModelEntityToDiagram(IModelEntity selectedEntity)
        {
            Diagram.ShowItem(selectedEntity);
        }
    }
}
