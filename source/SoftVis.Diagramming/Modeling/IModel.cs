using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model consists of nodes and relationships between nodes.
    /// The nodes form one or more tree hierarchies (e.g. packages contain other packages and types).
    /// Immutable.
    /// </summary>
    public interface IModel
    {
        IEnumerable<IModelNode> Nodes { get; }
        IEnumerable<IModelRelationship> Relationships { get; }
        IEnumerable<IModelNode> RootNodes { get; }

        bool NodeExists(IModelNode node);
        bool RelationshipExists(IModelRelationship relationship);
        bool PathExists(IModelNode sourceNode, IModelNode targetNode);

        IModelNode GetNodeById(ModelNodeId nodeId);

        IEnumerable<IModelNode> GetChildNodes(IModelNode node);
        IEnumerable<IModelNode> GetRelatedNodes(IModelNode node, 
            DirectedModelRelationshipType directedModelRelationshipType, bool recursive = false);

        IEnumerable<IModelRelationship> GetRelationships(IModelNode node);

        IModel AddNode(IModelNode node);
        IModel RemoveNode(IModelNode node);
        IModel ReplaceNode(IModelNode oldNode, IModelNode newNode);
        IModel AddRelationship(IModelRelationship relationship);
        IModel RemoveRelationship(IModelRelationship relationship);
        IModel Clear();
    }
}
