using System.Collections.Generic;
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
        public ObservableCollection<DiagramShapeButtonViewModelBase> DiagramButtonViewModels { get; }

        public DiagramShapeButtonCollectionViewModel(IReadOnlyModel model, IDiagram diagram,
            IDiagramBehaviourProvider diagramBehaviourProvider)
            : base(model, diagram)
        {
            DiagramButtonViewModels = new ObservableCollection<DiagramShapeButtonViewModelBase>(
                CreateButtons(model, diagram, diagramBehaviourProvider));
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

        private static IEnumerable<DiagramShapeButtonViewModelBase> CreateButtons(
            IReadOnlyModel model, IDiagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider)
        {
            yield return new CloseShapeButtonViewModel(model, diagram);

            foreach (var descriptor in diagramBehaviourProvider.GetRelatedEntityButtonDescriptors())
                yield return new ShowRelatedNodeButtonViewModel(model, diagram, descriptor);
        }
    }
}
