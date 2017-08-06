namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// A relationship is a directed link between two model nodes: a source and a target.
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
    }
}
