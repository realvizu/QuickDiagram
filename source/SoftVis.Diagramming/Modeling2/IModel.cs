using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// A read-only view of a model. 
    /// A model consists of nodes and relationships between nodes.
    /// The nodes form one or more tree hierarchies (e.g. packages contain other packages and types).
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// The collection of all model nodes.
        /// </summary>
        IEnumerable<IModelNode> Nodes { get; }

        /// <summary>
        /// The collection of relationships that exist between nodes.
        /// </summary>
        IEnumerable<IModelRelationship> Relationships { get; }

        /// <summary>
        /// The collection of model nodes that are hierarchy roots.
        /// </summary>
        IEnumerable<IModelNode> RootNodes { get; }

        /// <summary>
        /// Returns the children of a node.
        /// </summary>
        /// <param name="parentNodeId">Identifies the parent node.</param>
        /// <returns>The collection of child nodes. Can be empty.</returns>
        IEnumerable<IModelNode> GetChildNodes(ModelItemId parentNodeId);

        /// <summary>
        /// Returns a model node by its id. Throws if not found.
        /// </summary>
        /// <param name="modelNodeId">The id of the model node.</param>
        /// <returns>A model node.</returns>
        IModelNode GetModelNode(ModelItemId modelNodeId);

        /// <summary>
        /// Returns all relationships attached to the given node (as either a source or target node).
        /// </summary>
        /// <param name="modelNodeId">The id of the model node.</param>
        /// <returns>A read-only collection of relationships.</returns>
        IEnumerable<IModelRelationship> GetRelationships(ModelItemId modelNodeId);

        /// <summary>
        /// Returns those nodes that are related to the given node with the given type of relationship.
        /// </summary>
        /// <param name="modelNodeId">The id of the model node.</param>
        /// <param name="modelRelationshipType">A directed relationship type.</param>
        /// <param name="recursive">True means that nodes are recursively traversed. False returns only immediately related nodes.</param>
        /// <returns>A read-only collection of nodes.</returns>
        IEnumerable<IModelNode> GetRelatedNodes(ModelItemId modelNodeId, DirectedModelRelationshipType modelRelationshipType, bool recursive = false);
    }
}
