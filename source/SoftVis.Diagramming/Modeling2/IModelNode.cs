using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// Represents a named item in the model, e.g. a package, a type, a member.
    /// Model nodes form tree hierarchies, so nodes can have child nodes.
    /// </summary>
    public interface IModelNode
    {
        IEnumerable<IModelNode> ChildNodes { get; }

        string DisplayName { get; }
        string FullName { get; }
        string Description { get; }

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
