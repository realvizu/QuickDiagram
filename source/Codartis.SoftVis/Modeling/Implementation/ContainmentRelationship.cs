using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for relationships where a model node contains another one.
    /// </summary>
    public abstract class ContainmentRelationshipBase : ModelRelationship
    {
        protected ContainmentRelationshipBase(ModelRelationshipId id, IModelNode source, IModelNode target)
            : base(id, source, target, ModelRelationshipStereotype.Containment)
        {
        }
    }
}
