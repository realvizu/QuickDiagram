using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model created from Roslyn symbols.
    /// </summary>
    internal class RoslynBasedModel : Model
    {
        public IReadOnlyList<IRoslynBasedModelEntity> RoslynBasedEntities => Entities.OfType<IRoslynBasedModelEntity>().ToArray();

        public override IEnumerable<ModelEntityStereotype> GetModelEntityStereotypes()
        {
            foreach (var modelEntityStereotype in base.GetModelEntityStereotypes())
                yield return modelEntityStereotype;

            yield return ModelEntityStereotypes.Class;
            yield return ModelEntityStereotypes.Interface;
            yield return ModelEntityStereotypes.Struct;
            yield return ModelEntityStereotypes.Enum;
            yield return ModelEntityStereotypes.Delegate;
        }

        public override IEnumerable<ModelRelationshipStereotype> GetModelRelationshipStereotypes()
        {
            foreach (var modelRelationshipStereotype in base.GetModelRelationshipStereotypes())
                yield return modelRelationshipStereotype;

            yield return ModelRelationshipStereotypes.Implementation;
        }

        public IRoslynBasedModelEntity GetOrAddEntity(Func<IRoslynBasedModelEntity, bool> entityPredicate, Func<IRoslynBasedModelEntity> createEntityFunc)
        {
            return base.GetOrAddEntity(i => entityPredicate((IRoslynBasedModelEntity) i), createEntityFunc) as IRoslynBasedModelEntity;
        }

        public IRoslynBasedModelEntity GetOrAddEntity(INamedTypeSymbol roslynSymbol, Func<IRoslynBasedModelEntity> createEntityFunc)
        {
            return this.GetOrAddEntity(i => i.SymbolEquals(roslynSymbol), createEntityFunc);
        }

        public IRoslynBasedModelEntity GetEntityBySymbol(INamedTypeSymbol namedTypeSymbol)
        {
            return RoslynBasedEntities.FirstOrDefault(i => i.SymbolEquals(namedTypeSymbol));
        }

        public IRoslynBasedModelEntity GetEntityByLocation(Location location)
        {
            if (location == null)
                return null;

            var fileLinePositionSpan = location.GetMappedLineSpan();

            foreach (var roslynBasedModelEntity in RoslynBasedEntities)
            {
                var entityLocation = roslynBasedModelEntity.RoslynSymbol?.Locations.FirstOrDefault()?.GetMappedLineSpan();
                if (entityLocation != null && entityLocation.Value.Overlaps(fileLinePositionSpan))
                    return roslynBasedModelEntity;
            }

            return null;
        }

        public void UpdateEntity(IRoslynBasedModelEntity entity, INamedTypeSymbol roslynSymbol)
        {
            entity.RoslynSymbol = roslynSymbol;
            base.UpdateEntity(entity, 
                roslynSymbol.GetName(), 
                roslynSymbol.GetFullName(),
                roslynSymbol.GetDescription());
        }
    }
}
