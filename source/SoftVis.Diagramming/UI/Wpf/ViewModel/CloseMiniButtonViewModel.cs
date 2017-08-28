using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button for removing a shape from the diagram.
    /// </summary>
    public class CloseMiniButtonViewModel : MiniButtonViewModelBase
    {
        public CloseMiniButtonViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore)
            : base(modelStore, diagramStore, "Remove")
        {
            IsEnabled = true;
        }

        /// <summary>
        /// For one-off buttons the placement key is the type of the viewmodel.
        /// </summary>
        public override object PlacementKey => this.GetType();

        protected override void OnClick()
        {
            if (HostViewModel is DiagramNodeViewModelBase diagramNodeViewModel)
                diagramNodeViewModel.Remove();
        }
    }
}
