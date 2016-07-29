using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model entity created from a Roslyn class symbol.
    /// </summary>
    internal class RoslynBasedClass : RoslynBasedModelEntity
    {
        internal RoslynBasedClass(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Class)
        {
        }

        public override int Priority => 4;
        public override bool IsAbstract => RoslynSymbol.IsAbstract;

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
                foreach (var baseSymbolRelation in GetBaseTypes(RoslynSymbol))
                    yield return baseSymbolRelation;

            if (RelatedEntitySpecifications.Subtype.IsSpecifiedBy(relatedEntitySpecification))
                foreach (var derivedSymbolRelation in GetDerivedTypes(roslynModelProvider, RoslynSymbol))
                    yield return derivedSymbolRelation;

            if (RoslynRelatedEntitySpecifications.ImplementedInterface.IsSpecifiedBy(relatedEntitySpecification))
                foreach (var implementedSymbolRelation in GetImplementedInterfaces(RoslynSymbol))
                    yield return implementedSymbolRelation;
        }

        private static IEnumerable<RoslynSymbolRelation> GetBaseTypes(INamedTypeSymbol roslynSymbol)
        {
            var baseSymbol = roslynSymbol.BaseType;
            if (baseSymbol != null)
                yield return new RoslynSymbolRelation(roslynSymbol, baseSymbol, RelatedEntitySpecifications.BaseType);
        }
    }
}
