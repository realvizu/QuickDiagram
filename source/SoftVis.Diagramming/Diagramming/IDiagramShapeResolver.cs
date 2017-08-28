using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Resolves diagram concepts from model concepts.
    /// </summary>
    public interface IDiagramShapeResolver
    {
        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);

        IDiagramNode GetDiagramNodeById(ModelNodeId id);
    }
}
