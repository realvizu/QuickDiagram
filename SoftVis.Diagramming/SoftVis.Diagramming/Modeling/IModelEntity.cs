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
        /// Provides a fixed set of entity categories.
        /// </summary>
        ModelEntityClassifier Classifier { get; }

        /// <summary>
        /// Provides an extensible set of entity categories.
        /// </summary>
        ModelEntityStereotype Stereotype { get; }

        /// <summary>
        /// The relative importance of this model entity compared to others. Used for layout.
        /// Higher value means more important.
        /// </summary>
        int Priority { get; }

        bool IsAbstract { get; }
    }
}
