using System.Collections.Generic;

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
        /// Provides a fixed set of entity categories.
        /// </summary>
        ModelEntityType Type { get; }

        /// <summary>
        /// Provides an extensible set of entity categories.
        /// </summary>
        ModelEntityStereotype Stereotype { get; }

        /// <summary>
        /// All relationships within the model whose source entity is this.
        /// </summary>
        IEnumerable<IModelRelationship> OutgoingRelationships { get; }

        /// <summary>
        /// All relationships within the model whose target entity is this.
        /// </summary>
        IEnumerable<IModelRelationship> IncomingRelationships { get; }
    }
}
