using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model created from Roslyn symbols.
    /// </summary>
    internal class RoslynBasedModel : Model
    {
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
            return Entities.OfType<RoslynBasedModelEntity>()
                .FirstOrDefault(i => namedTypeSymbol.SymbolEquals(i.RoslynSymbol));
        }
    }
}
