using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI.Wpf.ViewModels;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for those view models that can observe model and diagram changes.
    /// </summary>
    public abstract class ModelObserverViewModelBase : ViewModelBase
    {
        [NotNull] protected readonly IModelService ModelService;
        [NotNull] protected readonly IDiagramService DiagramService;

        protected ModelObserverViewModelBase(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService)
        {
            ModelService = modelService;
            DiagramService = diagramService;
        }
    }
}