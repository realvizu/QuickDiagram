using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model created from Roslyn symbols.
    /// </summary>
    internal class RoslynBasedModel : IModel
    {
        private readonly Dictionary<string, RoslynBasedModelEntity> _entities;
        private readonly Dictionary<string, ModelRelationship> _relationships;

        internal RoslynBasedModel()
        {
            _entities = new Dictionary<string, RoslynBasedModelEntity>();
            _relationships = new Dictionary<string, ModelRelationship>();
        }

        public IEnumerable<IModelEntity> Entities => _entities.Values;
        public IEnumerable<IModelRelationship> Relationships => _relationships.Values;

        public RoslynBasedModelEntity GetOrAddEntity(INamedTypeSymbol namedTypeSymbol)
        {
            return GetModelEntity(namedTypeSymbol) 
                ?? CreateAndAddModelEntity(namedTypeSymbol);
        }

        public ModelRelationship GetOrAddRelationship(INamedTypeSymbol sourceSymbol, INamedTypeSymbol targetSymbol,
            ModelRelationshipType type, ModelRelationshipStereotype stereotype = null)
        {
            var sourceEntity = GetOrAddEntity(sourceSymbol);
            var targetEntity = GetOrAddEntity(targetSymbol);

            return GetModelRelationship(sourceEntity, targetEntity, type, stereotype) 
                ?? CreateAndAddModelRelationship(sourceEntity, targetEntity, type, stereotype);
        }

        private RoslynBasedModelEntity GetModelEntity(INamedTypeSymbol namedTypeSymbol)
        {
            var key = namedTypeSymbol.GetKey();
            RoslynBasedModelEntity entity;
            return _entities.TryGetValue(key, out entity) ? entity : null;
        }

        private ModelRelationship GetModelRelationship(
            RoslynBasedModelEntity sourceEntity, RoslynBasedModelEntity targetEntity,
            ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            var key = GetRelationshipKey(sourceEntity, targetEntity, type, stereotype);
            ModelRelationship relationship;
            return _relationships.TryGetValue(key, out relationship) ? relationship : null;
        }

        private RoslynBasedModelEntity CreateAndAddModelEntity(INamedTypeSymbol namedTypeSymbol)
        {
            RoslynBasedModelEntity newEntity;

            switch (namedTypeSymbol.TypeKind)
            {
                case (TypeKind.Class):
                    newEntity = new RoslynBasedClass(namedTypeSymbol);
                    break;
                case (TypeKind.Interface):
                    newEntity = new RoslynBasedInterface(namedTypeSymbol);
                    break;
                case (TypeKind.Struct):
                    newEntity = new RoslynBasedStruct(namedTypeSymbol);
                    break;
                case (TypeKind.Enum):
                    newEntity = new RoslynBasedEnum(namedTypeSymbol);
                    break;
                case (TypeKind.Delegate):
                    newEntity = new RoslynBasedDelegate(namedTypeSymbol);
                    break;
                default:
                    throw new ArgumentException($"Unexpected TypeKind: {namedTypeSymbol.TypeKind}.");
            }

            _entities.Add(namedTypeSymbol.GetKey(), newEntity);

            return newEntity;
        }

        private ModelRelationship CreateAndAddModelRelationship(
            RoslynBasedModelEntity sourceEntity, RoslynBasedModelEntity targetEntity,
            ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            var newRelationship = new ModelRelationship(sourceEntity, targetEntity, type, stereotype);

            sourceEntity.AddOutgoingRelationship(newRelationship);
            targetEntity.AddIncomingRelationship(newRelationship);

            var key = GetRelationshipKey(sourceEntity, targetEntity, type, stereotype);
            _relationships.Add(key, newRelationship);

            return newRelationship;
        }

        private static string GetRelationshipKey(
            RoslynBasedModelEntity sourceEntity, RoslynBasedModelEntity targetEntity,
            ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            return $"{sourceEntity.Id}--({type}/{stereotype})-->{targetEntity.Id}";
        }
    }
}
