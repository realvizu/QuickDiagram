using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Creates diagram service instances.
    /// </summary>
    public interface IDiagramServiceFactory
    {
        IDiagramService Create(IReadOnlyModelStore modelStore);
    }
}
