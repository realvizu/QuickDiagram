using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements the model that the diagrams are based upon.
    /// </summary>
    [DebuggerDisplay("EntityCount={_graph.VertexCount}, RelationshipCount={_graph.EdgeCount}")]
    public class Model : IReadOnlyModel
    {
        private readonly ModelGraph _graph;

        public event Action<IModelEntity> EntityAdded;
        public event Action<IModelRelationship> RelationshipAdded;
        public event Action<IModelEntity> EntityRemoved;
        public event Action<IModelRelationship> RelationshipRemoved;
        public event Action<IModelEntity, string, string, string> EntityRenamed;
        public event Action ModelCleared;

        public Model()
        {
            _graph = new ModelGraph();

            _graph.VertexAdded += i => EntityAdded?.Invoke(i);
            _graph.VertexRemoved += i => EntityRemoved?.Invoke(i);
            _graph.EdgeAdded += i => RelationshipAdded?.Invoke(i);
            _graph.EdgeRemoved += i => RelationshipRemoved?.Invoke(i);
            _graph.Cleared += (i, j) => ModelCleared?.Invoke();
        }

        public IReadOnlyList<IModelEntity> Entities => _graph.Vertices;
        public IReadOnlyList<IModelRelationship> Relationships => _graph.Edges;

        public virtual IEnumerable<ModelEntityStereotype> GetModelEntityStereotypes()
        {
            yield return ModelEntityStereotype.None;
        }

        public virtual IEnumerable<ModelRelationshipStereotype> GetModelRelationshipStereotypes()
        {
            yield return ModelRelationshipStereotype.None;
        }

        public IReadOnlyList<IModelRelationship> GetRelationships(IModelEntity entity)
        {
            return Relationships.Where(i => i.Source == entity || i.Target == entity).ToArray();
        }

        public IReadOnlyList<IModelEntity> GetRelatedEntities(IModelEntity entity, EntityRelationType relationType, bool recursive = false)
        {
            return _graph.GetConnectedVertices(entity,
                (otherEntity, relationship) => relationship.Type == relationType.Type &&
                relationship.IsEntityInRelationship(otherEntity, relationType.Direction),
                recursive);
        }

        public virtual IModelEntity GetOrAddEntity(Func<IModelEntity, bool> entityPredicate, Func<IModelEntity> createEntityFunc)
            => _graph.GetOrAddVertex(i => entityPredicate(i), createEntityFunc).Result;

        public virtual IModelEntity GetOrAddEntity(IModelEntity entity)
            => this.GetOrAddEntity(i => i.Equals(entity), () => entity);

        public virtual ModelRelationship GetOrAddRelationship(ModelRelationship relationship)
            => _graph.GetOrAddEdge(i => i == relationship, () => relationship).Result;

        public virtual ModelRelationship GetOrAddRelationship(IModelEntity sourceEntity, IModelEntity targetEntity, ModelRelationshipType relationType)
        {
            var relationship = new ModelRelationship(sourceEntity, targetEntity, relationType);
            return this.GetOrAddRelationship(relationship);
        }

        public virtual void RemoveEntity(IModelEntity entity) => _graph.RemoveVertex(entity);
        public virtual void RemoveRelationship(ModelRelationship relationship) => _graph.RemoveEdge(relationship);
        public void Clear() => _graph.Clear();

        public virtual void UpdateEntity(IModelEntity entity, string name, string fullName, string description)
        {
            entity.UpdateName(name, fullName, description);
            EntityRenamed?.Invoke(entity, name, fullName, description);
        }
    }
}
