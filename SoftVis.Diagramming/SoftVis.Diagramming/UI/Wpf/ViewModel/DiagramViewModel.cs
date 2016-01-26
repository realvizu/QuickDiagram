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
        public IModel Model { get; }
        public Diagram Diagram { get; }
        public IDiagramBehaviourProvider DiagramBehaviourProvider { get; }

        private DiagramViewportViewModel _diagramViewportViewModel;
        public EntitySelectorViewModel RelatedEntitySelectorViewModel { get; }
        public ICommand ShowRelatedEntitySelectorCommand { get; }
        public ICommand HideRelatedEntitySelectorCommand { get; }

        public DiagramViewModel(IModel model, Diagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider,
            double minZoom, double maxZoom, double initialZoom)
        {
            Model = model;
            Diagram = diagram;
            DiagramBehaviourProvider = diagramBehaviourProvider;

            DiagramViewportViewModel = new DiagramViewportViewModel(diagram, diagramBehaviourProvider, minZoom, maxZoom, initialZoom);
            RelatedEntitySelectorViewModel = new EntitySelectorViewModel(new Size(200, 100));
            ShowRelatedEntitySelectorCommand = new DelegateCommand<DiagramButtonActivatedEventArgs>(ShowRelationshipSelector);
            HideRelatedEntitySelectorCommand = new DelegateCommand(HideRelationshipSelector);
        }

        public DiagramViewportViewModel DiagramViewportViewModel
        {
            get { return _diagramViewportViewModel; }
            set
            {
                _diagramViewportViewModel = value;
                OnPropertyChanged();
            }
        }

        private void ShowRelationshipSelector(DiagramButtonActivatedEventArgs e)
        {
            var relatedEntities = Model.GetRelatedEntities(e.ModelEntity, e.RelationshipSpecification).ToList();
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
