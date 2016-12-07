using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Util.Roslyn;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model created from Roslyn symbols.
    /// </summary>
    internal class RoslynBasedModel : Model
    {
        public IEnumerable<RoslynBasedModelEntity> RoslynBasedEntities => Entities.OfType<RoslynBasedModelEntity>();

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

        public RoslynBasedModelEntity GetModelEntity(INamedTypeSymbol namedTypeSymbol)
        {
            return RoslynBasedEntities.FirstOrDefault(i => namedTypeSymbol.SymbolEquals(i.RoslynSymbol));
        }

        public void UpdateEntity(IRoslynBasedModelEntity entity, INamedTypeSymbol roslynSymbol)
        {
            entity.RoslynSymbol = roslynSymbol;
            base.UpdateEntity(entity, roslynSymbol.GetMinimallyQualifiedName(), roslynSymbol.GetFullyQualifiedName());
        }

        public IRoslynBasedModelEntity FindEntityByLocation(INamedTypeSymbol roslynSymbol)
        {
            var symbolLocation = roslynSymbol.Locations.FirstOrDefault()?.GetMappedLineSpan();
            if (symbolLocation == null)
                return null;

            foreach (var roslynBasedModelEntity in RoslynBasedEntities)
            {
                var entityLocation = roslynBasedModelEntity.RoslynSymbol?.Locations.FirstOrDefault()?.GetMappedLineSpan();
                if (entityLocation != null && entityLocation.Value.Overlaps(symbolLocation.Value))
                    return roslynBasedModelEntity;
            }

            return null;
        }
    }
}
