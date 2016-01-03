using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Abstract base class for models.
    /// </summary>
    public abstract class ModelBase : IModel
    {
        public abstract IEnumerable<IModelEntity> Entities { get; }
        public abstract IEnumerable<IModelRelationship> Relationships { get; }

        public IEnumerable<IModelEntity> GetRelatedEntities(IModelEntity entity, RelationshipSpecification relationshipSpecification)
        {
            return relationshipSpecification.Direction == ModelRelationshipDirection.Incoming
                ? entity.IncomingRelationships.Where(i => MatchesSpecification(i, relationshipSpecification)).Select(i => i.Source)
                : entity.OutgoingRelationships.Where(i => MatchesSpecification(i, relationshipSpecification)).Select(i => i.Target);
        }

        private static bool MatchesSpecification(IModelRelationship relationship, RelationshipSpecification relationshipSpecification)
        {
            return relationship.IsOfType(relationshipSpecification.Type, relationshipSpecification.Stereotype);
        }
    }
}
