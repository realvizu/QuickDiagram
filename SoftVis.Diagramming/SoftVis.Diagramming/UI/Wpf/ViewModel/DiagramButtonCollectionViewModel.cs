using System.Collections.ObjectModel;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates and manages the diagram button viewmodels.
    /// </summary>
    internal class DiagramButtonCollectionViewModel
    {
        private readonly DiagramButtonViewModelFactory _miniButtonViewModelFactory;

        public ObservableCollection<DiagramButtonViewModelBase> MiniButtonViewModels { get; }

        public DiagramButtonCollectionViewModel(IDiagramBehaviourProvider diagramBehaviourProvider)
        {
            _miniButtonViewModelFactory = new DiagramButtonViewModelFactory(diagramBehaviourProvider,
                DiagramDefaults.ButtonRadius, DiagramDefaults.ButtonOverlapParentBy);

            MiniButtonViewModels = new ObservableCollection<DiagramButtonViewModelBase>();
            CreateMiniButtons();
        }

        public void AssignMiniButtonsTo(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            foreach (var miniButtonViewModel in MiniButtonViewModels)
                miniButtonViewModel.AssociateWith(diagramShapeViewModel);
        }

        public bool AreMiniButtonsAssignedTo(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            return MiniButtonViewModels.Any(i => i.AssociatedDiagramShapeViewModel == diagramShapeViewModel);
        }

        public void HideMiniButtons()
        {
            foreach (var miniButtonViewModel in MiniButtonViewModels)
                miniButtonViewModel.Hide();
        }

        private void CreateMiniButtons()
        {
            foreach (var miniButtonViewModel in _miniButtonViewModelFactory.CreateButtons())
                MiniButtonViewModels.Add(miniButtonViewModel);
        }
    }
}
