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

        public override IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol roslynSymbol)
        {
            EnsureSymbolTypeKind(roslynSymbol, TypeKind.Class);

            foreach (var baseSymbolRelation in GetBaseTypes(roslynSymbol))
                yield return baseSymbolRelation;

            foreach (var derivedSymbolRelation in GetDerivedTypes(roslynModelProvider, roslynSymbol))
                yield return derivedSymbolRelation;

            foreach (var implementedSymbolRelation in GetImplementedInterfaces(roslynSymbol))
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
