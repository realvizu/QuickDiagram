using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Service
{
    /// <summary>
    /// Per-diagram plugin.
    /// </summary>
    public interface IDiagramPlugin
    {
        void Initialize(IReadOnlyModelStore modelStore, IDiagramStore diagramStore);
    }
}
