using System;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for relationships where a model node contains another one.
    /// </summary>
    [Immutable]
    public abstract class ContainmentRelationshipBase : ModelRelationship
    {
        protected ContainmentRelationshipBase(ModelRelationshipId id, IModelNode source, IModelNode target)
            : base(id, source, target, ModelRelationshipStereotype.Containment)
        {
        }
    }
}
