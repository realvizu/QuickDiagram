using Codartis.SoftVis.Graphs.Immutable;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Represents a named item in the model, e.g. a package, a type, a member.
    /// Immutable.
    /// </summary>
    public interface IModelNode : IReplaceableImmutableVertex<ModelNodeId>
    {
        string Name { get; }

        ModelNodeStereotype Stereotype { get; }

        ModelOrigin Origin { get; }
    }
}
