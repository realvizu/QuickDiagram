using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for those view models that store a model and a diagram.
    /// </summary>
    public abstract class DiagramViewModelBase: ViewModelBase
    {
        protected readonly IModel Model;
        protected readonly Diagram Diagram;

        protected DiagramViewModelBase(IModel model, Diagram diagram)
        {
            Model = model;
            Diagram = diagram;
        }
    }
}
