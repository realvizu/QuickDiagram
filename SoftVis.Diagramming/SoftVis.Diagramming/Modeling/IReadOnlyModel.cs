using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A read-only view of a model.
    /// A model contains entities and their relationships.
    /// </summary>
    public interface IReadOnlyModel
    {
        IReadOnlyList<IModelEntity> Entities { get; }
        IReadOnlyList<IModelRelationship> Relationships { get; }

        event Action<IModelEntity> EntityAdded;
        event Action<IModelEntity> EntityRemoved;
        event Action<IModelRelationship> RelationshipAdded;
        event Action<IModelRelationship> RelationshipRemoved;
        event Action<IModelEntity, string, string> EntityRenamed;
        event Action ModelCleared;

        IEnumerable<ModelEntityStereotype> GetModelEntityStereotypes();
        IEnumerable<ModelRelationshipStereotype> GetModelRelationshipStereotypes();

        IReadOnlyList<IModelRelationship> GetRelationships(IModelEntity entity);
        IReadOnlyList<IModelEntity> GetRelatedEntities(IModelEntity entity, EntityRelationType relationType, bool recursive = false);
    }
}
