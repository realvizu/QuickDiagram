using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract base class for those view models that can observe model and diagram changes.
    /// </summary>
    public abstract class ModelObserverViewModelBase : ViewModelBase
    {
        protected readonly IModelService ModelService;
        protected readonly IDiagramService DiagramService;

        protected ModelObserverViewModelBase(IModelService modelService, IDiagramService diagramService)
        {
            ModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
            DiagramService = diagramService ?? throw new ArgumentNullException(nameof(diagramService));
        }
    }
}
