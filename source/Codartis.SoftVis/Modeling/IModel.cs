using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model consists of nodes and relationships between nodes.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// Nodes can contain other nodes, but that's represented as a <see cref="ModelRelationshipStereotype.Containment"/> relationship
    /// (not as a tree of node objects).
    /// </remarks>
    public interface IModel
    {
        IEnumerable<IModelNode> Nodes { get; }
        IEnumerable<IModelRelationship> Relationships { get; }
        IEnumerable<IModelNode> RootNodes { get; }

        IModelNode GetNode(ModelNodeId nodeId);
        bool TryGetNode(ModelNodeId nodeId, out IModelNode node);

        IModelRelationship GetRelationship(ModelRelationshipId relationshipId);
        bool TryGetRelationship(ModelRelationshipId relationshipId, out IModelRelationship relationship);

        IEnumerable<IModelNode> GetChildNodes(ModelNodeId nodeId);

        IEnumerable<IModelNode> GetRelatedNodes(
            ModelNodeId nodeId,
            DirectedModelRelationshipType directedModelRelationshipType,
            bool recursive = false);

        IEnumerable<IModelRelationship> GetRelationships(ModelNodeId nodeId);

        IModel AddNode(IModelNode node);
        IModel RemoveNode(ModelNodeId nodeId);
        IModel ReplaceNode(IModelNode newNode);
        IModel AddRelationship(IModelRelationship relationship);
        IModel RemoveRelationship(ModelRelationshipId relationshipId);
        IModel Clear();
    }
}