using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button for removing a shape from the diagram.
    /// </summary>
    internal class CloseShapeButtonViewModel : DiagramButtonViewModelBase
    {
        public CloseShapeButtonViewModel(IModel model, Diagram diagram,
            double buttonRadius, RectRelativeLocation buttonLocation)
            : base(model, diagram, buttonRadius, buttonLocation, i => i.Remove())
        {
            IsEnabled = true;
        }
    }
}
