using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button for removing a shape from the diagram.
    /// </summary>
    public class CloseShapeButtonViewModel : DiagramShapeButtonViewModelBase
    {
        public CloseShapeButtonViewModel(IArrangedDiagram diagram)
            : base(diagram)
        {
            IsEnabled = true;
        }

        /// <summary>
        /// For one-off buttons the placement key is the type of the viewmodel.
        /// </summary>
        public override object PlacementKey => this.GetType();

        protected override void OnClick() => AssociatedDiagramShapeViewModel.Remove();
    }
}
