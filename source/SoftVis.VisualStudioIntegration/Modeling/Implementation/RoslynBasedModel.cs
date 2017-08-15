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
        public IReadOnlyList<IRoslynBasedModelNode> RoslynBasedEntities => Entities.OfType<IRoslynBasedModelNode>().ToArray();

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

        public IRoslynBasedModelNode GetOrAddEntity(Func<IRoslynBasedModelNode, bool> entityPredicate, Func<IRoslynBasedModelNode> createEntityFunc)
        {
            return base.GetOrAddEntity(i => entityPredicate((IRoslynBasedModelNode) i), createEntityFunc) as IRoslynBasedModelNode;
        }

        public IRoslynBasedModelNode GetOrAddEntity(INamedTypeSymbol roslynSymbol, Func<IRoslynBasedModelNode> createEntityFunc)
        {
            return this.GetOrAddEntity(i => i.SymbolEquals(roslynSymbol), createEntityFunc);
        }

        public IRoslynBasedModelNode GetEntityBySymbol(INamedTypeSymbol namedTypeSymbol)
        {
            return RoslynBasedEntities.FirstOrDefault(i => i.SymbolEquals(namedTypeSymbol));
        }

        public IRoslynBasedModelNode GetEntityByLocation(Location location)
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

        public void UpdateEntity(IRoslynBasedModelNode node, INamedTypeSymbol roslynSymbol)
        {
            node.RoslynSymbol = roslynSymbol;
            base.UpdateEntity(node, 
                roslynSymbol.GetName(), 
                roslynSymbol.GetFullName(),
                roslynSymbol.GetDescription());
        }
    }
}
