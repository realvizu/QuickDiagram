using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Per-diagram plugin that reacts to model and/or diagram events and performs custom logic.
    /// </summary>
    public interface IDiagramPlugin
    {
        void Initialize(IModelService modelService, IDiagramService diagramService);
    }
}
