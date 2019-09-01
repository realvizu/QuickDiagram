using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Defines diagram-related operations.
    /// </summary>
    public interface IDiagramService : IDiagramStore
    {
        //IDiagramNode ShowModelNode(IModelNode modelNode);
        //void HideModelNode(ModelNodeId modelNodeId);

        //IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IModelNode> modelNodes,
        //    CancellationToken cancellationToken = default,
        //    IIncrementalProgress progress = null);

        //void ShowModelRelationship(IModelRelationship modelRelationship);
        //void HideModelRelationship(ModelRelationshipId modelRelationshipId);

        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}
