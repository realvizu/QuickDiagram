using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates and manages the diagram button viewmodels.
    /// </summary>
    internal class DiagramShapeButtonCollectionViewModel : DiagramViewModelBase
    { 
        public ObservableCollection<DiagramShapeButtonViewModelBase> DiagramButtonViewModels { get; }

        public DiagramShapeButtonCollectionViewModel(IDiagram diagram)
            : base(diagram)
        {
            DiagramButtonViewModels = new ObservableCollection<DiagramShapeButtonViewModelBase>(CreateButtons());
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

        private IEnumerable<DiagramShapeButtonViewModelBase> CreateButtons()
        {
            yield return new CloseShapeButtonViewModel(Diagram);

            foreach (var entityRelationType in Diagram.GetEntityRelationTypes())
                yield return new ShowRelatedNodeButtonViewModel(Diagram, entityRelationType);
        }
    }
}
