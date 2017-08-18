namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Represents a named item in the model, e.g. a package, a type, a member.
    /// </summary>
    public interface IModelNode : IModelItem
    {
        string Name { get; }

        /// <summary>
        /// The type of the model node.
        /// </summary>
        ModelNodeStereotype Stereotype { get; }

        /// <summary>
        /// Specifies the source of the model information for this node.
        /// </summary>
        ModelOrigin Origin { get; }

        /// <summary>
        /// Layout arranges child nodes based on the highest priority parent node.
        /// Higher value means higher priority.
        /// </summary>
        int LayoutPriority { get; }
    }
}
