namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// Represents a named item in the model, e.g. a package, a type, a member.
    /// </summary>
    public interface IModelNode : IModelItem
    {
        string Name { get; }

        /// <summary>
        /// Specifies the source of the model information for this node.
        /// </summary>
        ModelOrigin Origin { get; }

        /// <summary>
        /// The priority of this model node. Used for layout.
        /// Higher value means higher priority.
        /// </summary>
        int Priority { get; }
    }
}
