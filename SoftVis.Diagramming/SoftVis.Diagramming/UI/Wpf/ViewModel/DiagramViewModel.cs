using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Top level view model of the diagram control.
    /// </summary>
    public class DiagramViewModel : DiagramViewModelBase
    {
        private readonly IDiagramBehaviourProvider _diagramBehaviourProvider;
        private Rect _diagramContentRect;

        public event Action<double> DiagramImageExportRequested;

        public DiagramViewportViewModel DiagramViewportViewModel { get; }
        public EntitySelectorViewModel RelatedEntitySelectorViewModel { get; }
        public BitmapSourceDelegateCommand ExportDiagramImageCommand { get; }
        public ICommand ShowRelatedEntitySelectorCommand { get; }
        public ICommand HideRelatedEntitySelectorCommand { get; }

        public DiagramViewModel(IModel model, Diagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider,
            double minZoom, double maxZoom, double initialZoom)
            :base(model, diagram)
        {
            _diagramBehaviourProvider = diagramBehaviourProvider;

            DiagramViewportViewModel = new DiagramViewportViewModel(model, diagram, diagramBehaviourProvider, minZoom, maxZoom, initialZoom);
            RelatedEntitySelectorViewModel = new EntitySelectorViewModel(new Size(200, 100));
            ExportDiagramImageCommand = new BitmapSourceDelegateCommand(CopyDiagramImageToClipboard);
            ShowRelatedEntitySelectorCommand = new DelegateCommand(ShowRelationshipSelector);
            HideRelatedEntitySelectorCommand = new DelegateCommand(HideRelationshipSelector);

            SubscribeToDiagramEvents();
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

        private void ShowRelationshipSelector()
        {
            //var relatedEntities = _model.GetRelatedEntities(e.ModelEntity, e.RelationshipSpecification).ToList();
            //RelatedEntitySelectorViewModel.Show(e.AttachPoint, e.HandleOrientation, relatedEntities);
        }

        private void HideRelationshipSelector()
        {
            RelatedEntitySelectorViewModel.Hide();
        }

        public void ZoomToContent()
        {
            DiagramViewportViewModel.ZoomToContent();
        }

        public void CopyToClipboard(double dpi)
        {
            DiagramImageExportRequested?.Invoke(dpi);
        }

        private void CopyDiagramImageToClipboard(BitmapSource bitmapSource)
        {
            Clipboard.SetImage(bitmapSource);
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
    }
}
