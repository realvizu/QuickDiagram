using System.Collections.ObjectModel;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates and manages the minibutton viewmodels.
    /// </summary>
    internal class MiniButtonCollectionViewModel
    {
        private readonly MiniButtonViewModelFactory _miniButtonViewModelFactory;

        public ObservableCollection<MiniButtonViewModelBase> MiniButtonViewModels { get; }

        public MiniButtonCollectionViewModel(IDiagramBehaviourProvider diagramBehaviourProvider)
        {
            _miniButtonViewModelFactory = new MiniButtonViewModelFactory(diagramBehaviourProvider,
                DiagramDefaults.MiniButtonRadius, DiagramDefaults.MiniButtonOverlapParentBy);

            MiniButtonViewModels = new ObservableCollection<MiniButtonViewModelBase>();
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
