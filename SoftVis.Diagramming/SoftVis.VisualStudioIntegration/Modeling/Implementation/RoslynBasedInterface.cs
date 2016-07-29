using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model entity created from a Roslyn interface symbol.
    /// </summary>
    internal class RoslynBasedInterface : RoslynBasedModelEntity
    {
        internal RoslynBasedInterface(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Interface)
        {
        }

        public override int Priority => 2;

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="relatedEntitySpecification">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        public override IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            RelatedEntitySpecification? relatedEntitySpecification = null)
        {
            if (RelatedEntitySpecifications.BaseType.IsSpecifiedBy(relatedEntitySpecification))
                foreach (var baseSymbolRelation in GetBaseInterfaces(RoslynSymbol))
                    yield return baseSymbolRelation;

            if (RelatedEntitySpecifications.Subtype.IsSpecifiedBy(relatedEntitySpecification))
                foreach (var derivedSymbolRelation in GetDerivedInterfaces(roslynModelProvider, RoslynSymbol))
                    yield return derivedSymbolRelation;

            if (RoslynRelatedEntitySpecifications.ImplementerType.IsSpecifiedBy(relatedEntitySpecification))
                foreach (var implementingSymbolRelation in GetImplementingTypes(roslynModelProvider, RoslynSymbol))
                    yield return implementingSymbolRelation;
        }

        private static IEnumerable<RoslynSymbolRelation> GetBaseInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementedInterfaceSymbol in interfaceSymbol.AllInterfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RoslynSymbolRelation(interfaceSymbol, implementedInterfaceSymbol, RelatedEntitySpecifications.BaseType);
        }
    }
}
