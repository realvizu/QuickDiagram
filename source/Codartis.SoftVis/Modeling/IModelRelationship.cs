using System;
using Codartis.SoftVis.Graphs.Immutable;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A relationship is a directed link between two model nodes: a source and a target.
    /// It also has a stereotype.
    /// </summary>
    /// <remarks>
    /// For bidirectional relationships the model contains two directed relationships.
    /// </remarks>
    [Immutable]
    public interface IModelRelationship 
        : IUpdatableImmutableEdge<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId>
    {
        ModelRelationshipStereotype Stereotype { get; }
    }
}
