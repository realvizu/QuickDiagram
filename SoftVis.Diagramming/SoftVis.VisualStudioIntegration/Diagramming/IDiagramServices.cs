using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines diagram operations for the application commands package.
    /// </summary>
    public interface IDiagramServices
    {
        void ShowModelEntity(IRoslynBasedModelEntity modelEntity);
        void ShowModelEntityWithRelatedEntities(IRoslynBasedModelEntity modelEntity);
        void Clear();
    }
}
