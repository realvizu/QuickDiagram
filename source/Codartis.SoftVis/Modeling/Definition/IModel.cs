using System.Collections.Generic;
using Codartis.Util;

namespace Codartis.SoftVis.Modeling.Definition
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

        IModelNode GetNode(ModelNodeId nodeId);
        Maybe<IModelNode> TryGetNode(ModelNodeId nodeId);
        Maybe<IModelNode> TryGetParentNode(ModelNodeId modelNodeId);

        IModelRelationship GetRelationship(ModelRelationshipId relationshipId);
        Maybe<IModelRelationship> TryGetRelationship(ModelRelationshipId relationshipId);

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