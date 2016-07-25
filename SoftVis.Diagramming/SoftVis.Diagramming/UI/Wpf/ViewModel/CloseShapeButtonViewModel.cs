using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button for removing a shape from the diagram.
    /// </summary>
    internal class CloseShapeButtonViewModel : DiagramButtonViewModelBase
    {
        public CloseShapeButtonViewModel(IReadOnlyModel readOnlyModel, IDiagram diagram,
            double buttonRadius, RectRelativeLocation buttonLocation)
            : base(readOnlyModel, diagram, buttonRadius, buttonLocation)
        {
            IsEnabled = true;
        }

        protected override void OnClick() => AssociatedDiagramShapeViewModel.Remove();
    }
}
