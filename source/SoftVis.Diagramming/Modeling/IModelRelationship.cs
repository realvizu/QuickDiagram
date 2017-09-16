using Codartis.SoftVis.Graphs.Immutable;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A relationship is a directed link between two model nodes: a source and a target.
    /// It also has a stereotype.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// For bidirectional relationships the model contains two directed relationships.
    /// </remarks>
    public interface IModelRelationship : IReplaceableImmutableEdge<IModelNode, ModelRelationshipId, IModelRelationship>
    {
        ModelRelationshipStereotype Stereotype { get; }

        bool IsNodeRelated(IModelNode modelNode, DirectedModelRelationshipType directedModelRelationshipType);
    }
}
