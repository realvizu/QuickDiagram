namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model entity is model item that can have relationships to other model entites.
    /// Eg. a class.
    /// </summary>
    public interface IModelEntity : IModelItem
    {
        /// <summary>
        /// Display name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Fully qualified name.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Description of the entity.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Provides a fixed set of entity categories.
        /// </summary>
        ModelEntityClassifier Classifier { get; }

        /// <summary>
        /// Provides an extensible set of entity categories.
        /// </summary>
        ModelEntityStereotype Stereotype { get; }

        /// <summary>
        /// Specifies the source of the model information for this entity.
        /// </summary>
        ModelOrigin Origin { get; }

        /// <summary>
        /// The relative importance of this model entity compared to others. Used for layout.
        /// Higher value means more important.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Returns a value indicating whether this entity is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Updates the name and desription of the entity.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullName"></param>
        /// <param name="description"></param>
        void UpdateName(string name, string fullName, string description);
    }
}
