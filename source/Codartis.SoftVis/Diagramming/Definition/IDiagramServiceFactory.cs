using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Creates diagram service instances.
    /// </summary>
    public interface IDiagramServiceFactory
    {
        IDiagramService Create(IModelService modelService);
    }
}
