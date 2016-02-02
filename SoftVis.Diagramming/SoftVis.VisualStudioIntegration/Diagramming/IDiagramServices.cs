using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines diagram operations for the application commands package.
    /// </summary>
    public interface IDiagramServices
    {
        void ShowModelEntity(IModelEntity modelEntity);
        void ShowModelEntityWithRelatedEntities(IModelEntity modelEntity);
        void ClearDiagram();
    }
}
