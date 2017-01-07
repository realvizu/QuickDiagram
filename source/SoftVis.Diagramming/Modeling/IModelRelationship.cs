using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model relationship is a directed, typed connection between two model entites.
    /// </summary>
    public interface IModelRelationship : IModelItem, IEquatable<IModelRelationship>
    {
        IModelEntity Source { get; }
        IModelEntity Target { get; }

        /// <summary>
        /// Provides a fixed set of relationship categories.
        /// </summary>
        ModelRelationshipClassifier Classifier { get; }

        /// <summary>
        /// Provides an extensible set of relationship categories.
        /// </summary>
        ModelRelationshipStereotype Stereotype { get; }

        /// <summary>
        /// Specifies the relationship type (classification + stereotype).
        /// </summary>
        ModelRelationshipType Type { get; }
    }
}
