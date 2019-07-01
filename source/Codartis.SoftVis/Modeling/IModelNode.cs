using Codartis.SoftVis.Graphs.Immutable;
using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Represents a named item in the model, e.g. a package, a type, a member.
    /// Immutable.
    /// </summary>
    [Immutable]
    public interface IModelNode : IUpdatableImmutableVertex<ModelNodeId>
    {
        string Name { get; }

        ModelNodeStereotype Stereotype { get; }

        ModelOrigin Origin { get; }
    }
}
