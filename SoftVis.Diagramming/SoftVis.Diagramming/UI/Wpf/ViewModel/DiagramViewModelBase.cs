using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for those view models that store a model and a diagram.
    /// </summary>
    public abstract class DiagramViewModelBase: ViewModelBase
    {
        protected readonly IReadOnlyModel ReadOnlyModel;
        protected readonly IDiagram Diagram;

        protected DiagramViewModelBase(IReadOnlyModel readOnlyModel, IDiagram diagram)
        {
            ReadOnlyModel = readOnlyModel;
            Diagram = diagram;
        }
    }
}
