using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates UI service instances.
    /// </summary>
    public interface IUiServiceFactory
    {
        IUiService Create(IModelService modelService, IDiagramService diagramService,
             double minZoom, double maxZoom, double initialZoom);
    }
}
