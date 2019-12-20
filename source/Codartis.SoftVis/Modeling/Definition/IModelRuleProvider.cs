using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Defines the rules that models must adhere to.
    /// </summary>
    public interface IModelRuleProvider
    {
        /// <summary>
        /// Returns a value indicating whether the given relationship stereotype is valid between the given source and target node.
        /// </summary>
        bool IsRelationshipTypeValid(
            ModelRelationshipStereotype modelRelationshipStereotype,
            [NotNull] IModelNode source,
            [NotNull] IModelNode target);
    }
}