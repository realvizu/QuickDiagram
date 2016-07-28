namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model relationship is a directed, typed connection between two model entites.
    /// </summary>
    public interface IModelRelationship : IModelItem
    {
        IModelEntity Source { get; }
        IModelEntity Target { get; }

        /// <summary>
        /// Provides a fixed set of relationship categories.
        /// </summary>
        ModelRelationshipType Type { get; }

        /// <summary>
        /// Provides an extensible set of relationship categories.
        /// </summary>
        ModelRelationshipStereotype Stereotype { get; }

        bool IsOfType(ModelRelationshipTypeSpecification typeSpecification);
    }
}
