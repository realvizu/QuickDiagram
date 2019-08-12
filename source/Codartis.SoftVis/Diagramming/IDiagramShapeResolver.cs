using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Resolves diagram concepts from model concepts.
    /// </summary>
    public interface IDiagramShapeResolver
    {
        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);

        IDiagramNode GetDiagramNode(ModelNodeId id);
    }
}
