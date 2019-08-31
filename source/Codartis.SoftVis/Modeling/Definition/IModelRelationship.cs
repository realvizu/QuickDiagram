using Codartis.SoftVis.Graphs.Immutable;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// A relationship is a directed link between two model nodes: a source and a target.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// For bidirectional relationships the model contains two directed relationships.
    /// </remarks>
    public interface IModelRelationship
        : IImmutableEdge<ModelNodeId, IModelRelationship, ModelRelationshipId>
    {
        ModelRelationshipStereotype Stereotype { get; }

        /// <summary>
        /// An arbitrary extra object associated with this model node.
        /// Must be immutable.
        /// </summary>
        [CanBeNull]
        object Payload { get; }

        [NotNull]
        IModelRelationship WithPayload([CanBeNull] object newPayload);
    }
}