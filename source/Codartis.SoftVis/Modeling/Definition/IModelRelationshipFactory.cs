using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Creates model relationships.
    /// </summary>
    public interface IModelRelationshipFactory
    {
        [NotNull]
        IModelRelationship CreateRelationship(
            [NotNull] IModelNode source,
            [NotNull] IModelNode target,
            [NotNull] ModelRelationshipStereotype stereotype);
    }
}