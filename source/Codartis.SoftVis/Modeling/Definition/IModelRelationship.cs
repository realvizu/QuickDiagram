using Codartis.SoftVis.Graphs.Immutable;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// A relationship is a directed link between two model nodes: a source and a target.
    /// It also has a stereotype and a payload, which is an arbitrary extra object.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// For bidirectional relationships the model contains two directed relationships.
    /// </remarks>
    public interface IModelRelationship
        : IImmutableEdge<ModelNodeId, IModelRelationship, ModelRelationshipId>
    {
        ModelRelationshipStereotype Stereotype { get; }
        [CanBeNull] object Payload { get; }

        [NotNull]
        IModelRelationship WithPayload([CanBeNull] object newPayload);
    }
}