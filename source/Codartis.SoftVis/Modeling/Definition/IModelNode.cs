using Codartis.SoftVis.Graphs.Immutable;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Represents a named item in the model.
    /// Immutable.
    /// </summary>
    public interface IModelNode : IImmutableVertex<ModelNodeId>
    {
        [NotNull] string Name { get; }
        ModelNodeStereotype Stereotype { get; }

        /// <summary>
        /// An arbitrary extra object associated with this model node.
        /// Must be immutable.
        /// </summary>
        [CanBeNull]
        object Payload { get; }

        [NotNull]
        IModelNode WithName([NotNull] string newName);

        [NotNull]
        IModelNode WithPayload([CanBeNull] object newPayload);
    }
}