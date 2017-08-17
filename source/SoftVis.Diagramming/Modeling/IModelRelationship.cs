namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A relationship is a directed link between two model nodes: a source and a target.
    /// It also has a stereotype.
    /// </summary>
    /// <remarks>
    /// For bidirectional relationships the model contains two directed relationships.
    /// </remarks>
    public interface IModelRelationship : IModelItem
    {
        /// <summary>
        /// The relationship starts from this node.
        /// </summary>
        IModelNode Source { get; }

        /// <summary>
        /// The relationship points to this node.
        /// </summary>
        IModelNode Target { get; }

        /// <summary>
        /// The type of the relationship.
        /// </summary>
        ModelRelationshipStereotype Stereotype { get; }
    }
}
