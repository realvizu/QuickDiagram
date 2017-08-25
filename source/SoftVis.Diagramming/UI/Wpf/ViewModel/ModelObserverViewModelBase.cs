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
        protected readonly IReadOnlyModelStore ModelStore;
        protected readonly IReadOnlyDiagramStore DiagramStore;

        protected ModelObserverViewModelBase(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore)
        {
            ModelStore = modelStore ?? throw new ArgumentNullException(nameof(modelStore));
            DiagramStore = diagramStore ?? throw new ArgumentNullException(nameof(diagramStore));
        }
    }
}
