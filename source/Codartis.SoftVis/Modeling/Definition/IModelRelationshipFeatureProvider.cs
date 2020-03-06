using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Provides characteristic information about model relationship types.
    /// </summary>
    public interface IModelRelationshipFeatureProvider
    {
        /// <summary>
        /// returns a value indicating whether this type of relationship is transitive.
        /// </summary>
        bool IsTransitive(ModelRelationshipStereotype stereotype);

        /// <summary>
        /// Returns a key that identifies the group of relationships which are considered transitive.
        /// </summary>
        [NotNull]
        string GetTransitivityPartitionKey(ModelRelationshipStereotype stereotype);
    }
}