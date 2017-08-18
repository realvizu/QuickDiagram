using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for those view models that store a diagram reference.
    /// </summary>
    public abstract class DiagramViewModelBase : ViewModelBase
    {
        protected readonly IArrangedDiagram Diagram;

        protected DiagramViewModelBase(IArrangedDiagram diagram)
        {
            Diagram = diagram;
        }

        protected IModelProvider ModelProvider => Diagram.ModelProvider;
    }
}
