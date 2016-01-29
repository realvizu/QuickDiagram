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

        public IEnumerable<IModelEntity> GetRelatedEntities(IModelEntity entity, RelationshipSpecification specification)
        {
            var typeSpecification = specification.TypeSpecification;

            return specification.Direction == ModelRelationshipDirection.Incoming
                ? entity.IncomingRelationships.Where(i => i.IsOfType(typeSpecification)).Select(i => i.Source)
                : entity.OutgoingRelationships.Where(i => i.IsOfType(typeSpecification)).Select(i => i.Target);
        }
    }
}
