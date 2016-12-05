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

        public event EventHandler<IModelEntity> EntityAdded;
        public event EventHandler<IModelRelationship> RelationshipAdded;
        public event EventHandler<IModelEntity> EntityRemoved;
        public event EventHandler<IModelRelationship> RelationshipRemoved;
        public event Action<IModelEntity, string, string> EntityRenamed;

        public Model()
        {
            _graph = new ModelGraph();

            _graph.VertexAdded += i => EntityAdded?.Invoke(this, i);
            _graph.VertexRemoved += i => EntityRemoved?.Invoke(this, i);
            _graph.EdgeAdded += i => RelationshipAdded?.Invoke(this, i);
            _graph.EdgeRemoved += i => RelationshipRemoved?.Invoke(this, i);
        }

        public IEnumerable<IModelEntity> Entities => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        public virtual void AddEntity(IModelEntity entity) => _graph.AddVertex(entity);
        public virtual void AddRelationship(ModelRelationship relationship) => _graph.AddEdge(relationship);
        public virtual void RemoveEntity(IModelEntity entity) => _graph.RemoveVertex(entity);
        public virtual void RemoveRelationship(ModelRelationship relationship) => _graph.RemoveEdge(relationship);

        public virtual void UpdateEntity(IModelEntity entity, string name, string fullName)
        {
            entity.UpdateName(name, fullName);
            EntityRenamed?.Invoke(entity, name, fullName);
        }

        public IModelRelationship GetRelationship(IModelEntity source, IModelEntity target, ModelRelationshipType type)
        {
            return Relationships.FirstOrDefault(i => i.Source == source && i.Target == target && i.Type == type);
        }

        public virtual IEnumerable<ModelEntityStereotype> GetModelEntityStereotypes()
        {
            yield return ModelEntityStereotype.None;
        }

        public virtual IEnumerable<ModelRelationshipStereotype> GetModelRelationshipStereotypes()
        {
            yield return ModelRelationshipStereotype.None;
        }

        public IEnumerable<IModelRelationship> GetRelationships(IModelEntity entity)
        {
            return Relationships.Where(i => i.Source == entity || i.Target == entity);
        }

        public IEnumerable<IModelEntity> GetRelatedEntities(IModelEntity entity, EntityRelationType relationType, 
            bool recursive = false)
        {
            if (!_graph.ContainsVertex(entity))
                yield break;

            var relatedEntities = relationType.Direction == EntityRelationDirection.Incoming
                ? _graph.InEdges(entity).Where(i => i.Type == relationType.Type).Select(i => i.Source).Distinct()
                : _graph.OutEdges(entity).Where(i => i.Type == relationType.Type).Select(i => i.Target).Distinct();

            foreach (var relatedEntity in relatedEntities.ToList())
            {
                yield return relatedEntity;

                // TODO: loop detection?
                if (recursive)
                    foreach (var nextRelatedEntity in GetRelatedEntities(relatedEntity, relationType, recursive: true))
                        yield return nextRelatedEntity;
            }
        }

        public virtual ModelRelationship AddRelationshipIfNotExists(ModelEntity source, ModelEntity target,
            ModelRelationshipType type)
        {
            var modelRelationship = GetRelationship(source, target, type) as ModelRelationship;
            if (modelRelationship == null)
            {
                var newModelRelationship = new ModelRelationship(source, target, type);
                AddRelationship(newModelRelationship);
            }
            return modelRelationship;
        }
    }
}
