using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model is a container for model entities.
    /// </summary>
    public interface IModel
    {
        IEnumerable<IModelEntity> Entities { get; }
        IEnumerable<IModelRelationship> Relationships { get; }

        IEnumerable<IModelEntity> GetRelatedEntities(IModelEntity entity, RelationshipSpecification relationshipSpecification);
    }
}
