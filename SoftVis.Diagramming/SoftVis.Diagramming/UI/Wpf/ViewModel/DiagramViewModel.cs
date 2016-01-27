using System.Linq;
using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public class DiagramViewModel : ViewModelBase
    {
        private readonly IModel _model;
        private readonly Diagram _diagram;
        private readonly IDiagramBehaviourProvider _diagramBehaviourProvider;

        public DiagramViewportViewModel DiagramViewportViewModel { get; }
        public EntitySelectorViewModel RelatedEntitySelectorViewModel { get; }
        public ICommand ShowRelatedEntitySelectorCommand { get; }
        public ICommand HideRelatedEntitySelectorCommand { get; }

        public DiagramViewModel(IModel model, Diagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider,
            double minZoom, double maxZoom, double initialZoom)
        {
            _model = model;
            _diagram = diagram;
            _diagramBehaviourProvider = diagramBehaviourProvider;

            DiagramViewportViewModel = new DiagramViewportViewModel(diagram, diagramBehaviourProvider, minZoom, maxZoom, initialZoom);
            RelatedEntitySelectorViewModel = new EntitySelectorViewModel(new Size(200, 100));
            ShowRelatedEntitySelectorCommand = new DelegateCommand<DiagramButtonActivatedEventArgs>(ShowRelationshipSelector);
            HideRelatedEntitySelectorCommand = new DelegateCommand(HideRelationshipSelector);
        }

        private void ShowRelationshipSelector(DiagramButtonActivatedEventArgs e)
        {
            var relatedEntities = _model.GetRelatedEntities(e.ModelEntity, e.RelationshipSpecification).ToList();
            RelatedEntitySelectorViewModel.Show(e.AttachPoint, e.HandleOrientation, relatedEntities);
        }

        private void HideRelationshipSelector()
        {
            RelatedEntitySelectorViewModel.Hide();
        }

        public void ZoomToContent()
        {
            DiagramViewportViewModel.ZoomToContent();
        }
    }
}
