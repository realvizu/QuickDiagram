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

        public virtual void AddEntity(ModelEntity entity) => _graph.AddVertex(entity);
        public virtual void AddRelationship(ModelRelationship relationship) => _graph.AddEdge(relationship);

        public IModelRelationship GetRelationship(IModelEntity source, IModelEntity target, ModelRelationshipTypeSpecification typeSpecification)
        {
            return Relationships.FirstOrDefault(i => i.Source == source && i.Target == target &&
                i.Type == typeSpecification.Type && i.Stereotype == typeSpecification.Stereotype);
        }

        public IEnumerable<IModelRelationship> GetRelationships(IModelEntity entity)
        {
            return Relationships.Where(i => i.Source == entity || i.Target == entity);
        }

        public IEnumerable<IModelEntity> GetRelatedEntities(IModelEntity entity,
            RelatedEntitySpecification specification, bool recursive = false)
        {
            var typeSpecification = specification.TypeSpecification;

            var relatedEntities = specification.Direction == EntityRelationDirection.Incoming
                ? _graph.InEdges(entity).Where(i => i.IsOfType(typeSpecification)).Select(i => i.Source).Distinct()
                : _graph.OutEdges(entity).Where(i => i.IsOfType(typeSpecification)).Select(i => i.Target).Distinct();

            foreach (var relatedEntity in relatedEntities.ToList())
            {
                yield return relatedEntity;

                // TODO: loop detection?
                if (recursive)
                    foreach (var nextRelatedEntity in GetRelatedEntities(relatedEntity, specification, recursive: true))
                        yield return nextRelatedEntity;
            }
        }

        public virtual ModelRelationship AddRelationshipIfNotExists(ModelEntity source, ModelEntity target,
            ModelRelationshipTypeSpecification typeSpecification)
        {
            var modelRelationship = GetRelationship(source, target, typeSpecification) as ModelRelationship;
            if (modelRelationship == null)
            {
                var newModelRelationship = new ModelRelationship(source, target, typeSpecification.Type, typeSpecification.Stereotype);
                AddRelationship(newModelRelationship);
            }
            return modelRelationship;
        }
    }
}
