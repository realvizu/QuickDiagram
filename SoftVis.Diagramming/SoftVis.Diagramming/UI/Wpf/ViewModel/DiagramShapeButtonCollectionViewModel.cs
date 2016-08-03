using System.Collections.ObjectModel;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates and manages the diagram button viewmodels.
    /// </summary>
    internal class DiagramShapeButtonCollectionViewModel : DiagramViewModelBase
    {
        private readonly DiagramShapeButtonViewModelFactory _diagramShapeButtonViewModelFactory;

        public ObservableCollection<DiagramShapeButtonViewModelBase> DiagramButtonViewModels { get; }

        public DiagramShapeButtonCollectionViewModel(IReadOnlyModel model, IDiagram diagram,
            IDiagramBehaviourProvider diagramBehaviourProvider)
            : base(model, diagram)
        {
            _diagramShapeButtonViewModelFactory = new DiagramShapeButtonViewModelFactory(model, diagram,
                diagramBehaviourProvider, DiagramDefaults.ButtonRadius, DiagramDefaults.ButtonOverlapParentBy);

            DiagramButtonViewModels = new ObservableCollection<DiagramShapeButtonViewModelBase>();
            CreateButtons();
        }

        public void AssignButtonsTo(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            foreach (var buttonViewModel in DiagramButtonViewModels)
                buttonViewModel.AssociateWith(diagramShapeViewModel);
        }

        public bool AreButtonsAssignedTo(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            return DiagramButtonViewModels.Any(i => i.AssociatedDiagramShapeViewModel == diagramShapeViewModel);
        }

        public void HideButtons()
        {
            foreach (var buttonViewModel in DiagramButtonViewModels)
                buttonViewModel.Hide();
        }

        private void CreateButtons()
        {
            foreach (var buttonViewModel in _diagramShapeButtonViewModelFactory.CreateButtons())
                DiagramButtonViewModels.Add(buttonViewModel);
        }
    }
}
