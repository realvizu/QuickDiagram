using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Creates UI service instances.
    /// </summary>
    public interface IUiServiceFactory
    {
        IUiService Create(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
             double minZoom, double maxZoom, double initialZoom);
    }
}
