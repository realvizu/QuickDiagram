using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model is a container for model entities and relationships.
    /// </summary>
    public interface IModel
    {
        IEnumerable<IModelEntity> Entities { get; }
        IEnumerable<IModelRelationship> Relationships { get; }

        event EventHandler<IModelEntity> EntityAdded;
        event EventHandler<IModelRelationship> RelationshipAdded;
        event EventHandler<IModelEntity> EntityRemoved;
        event EventHandler<IModelRelationship> RelationshipRemoved;

        IEnumerable<IModelEntity> GetRelatedEntities(IModelEntity entity, RelationshipSpecification specification);
    }
}
