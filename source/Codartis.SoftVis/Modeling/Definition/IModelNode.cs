using Codartis.SoftVis.Graphs.Immutable;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Represents a named item in the model, e.g. a package, a type, a member.
    /// Immutable.
    /// </summary>
    public interface IModelNode : IImmutableVertex<ModelNodeId>
    {
        string Name { get; }

        ModelNodeStereotype Stereotype { get; }

        ModelOrigin Origin { get; }
    }
}
