using Codartis.SoftVis.Graphs.Immutable;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Represents a named item in the model.
    /// Immutable.
    /// Can have a payload, that is, an arbitrary extra object.
    /// </summary>
    public interface IModelNode : IImmutableVertex<ModelNodeId>
    {
        [NotNull] string Name { get; }
        ModelNodeStereotype Stereotype { get; }
        [CanBeNull]object Payload { get; }

        [NotNull]
        IModelNode WithName([NotNull] string newName);
        [NotNull]
        IModelNode WithPayload([CanBeNull] object newPayload);
    }
}