using System.Collections.Generic;
using Codartis.Util;
using JetBrains.Annotations;

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
        [NotNull] [ItemNotNull] IEnumerable<IModelNode> Nodes { get; }
        [NotNull] [ItemNotNull] IEnumerable<IModelRelationship> Relationships { get; }

        bool ContainsNode(ModelNodeId nodeId);
        bool ContainsRelationship(ModelRelationshipId relationshipId);

        Maybe<IModelNode> TryGetNode(ModelNodeId nodeId);
        Maybe<IModelNode> TryGetParentNode(ModelNodeId modelNodeId);
        Maybe<IModelNode> TryGetNodeByPayload([NotNull] object payload);

        Maybe<IModelRelationship> TryGetRelationship(ModelRelationshipId relationshipId);
        Maybe<IModelRelationship> TryGetRelationshipByPayload([NotNull] object payload);

        [NotNull]
        [ItemNotNull]
        IEnumerable<IModelNode> GetRelatedNodes(
            ModelNodeId nodeId,
            DirectedModelRelationshipType directedModelRelationshipType,
            bool recursive = false);

        [NotNull]
        [ItemNotNull]
        IEnumerable<IModelRelationship> GetRelationships(ModelNodeId nodeId);

        ModelEvent AddNode(
            [NotNull] string name,
            ModelNodeStereotype stereotype,
            [CanBeNull] object payload = null,
            ModelNodeId? parentNodeId = null);

        ModelEvent UpdateNode([NotNull] IModelNode updatedNode);

        ModelEvent RemoveNode(ModelNodeId nodeId);

        ModelEvent AddRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype stereotype,
            [CanBeNull] object payload = null);

        // TODO: UpdateRelationshipPayload ?

        ModelEvent RemoveRelationship(ModelRelationshipId relationshipId);

        ModelEvent Clear();
    }
}