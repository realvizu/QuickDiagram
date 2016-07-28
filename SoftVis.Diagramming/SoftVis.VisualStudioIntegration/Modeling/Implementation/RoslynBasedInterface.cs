using System.Collections.Generic;
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

        public override IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol roslynSymbol)
        {
            EnsureSymbolTypeKind(roslynSymbol, TypeKind.Interface);

            foreach (var baseSymbolRelation in GetBaseInterfaces(roslynSymbol))
                yield return baseSymbolRelation;

            foreach (var derivedSymbolRelation in GetDerivedInterfaces(roslynModelProvider, roslynSymbol))
                yield return derivedSymbolRelation;

            foreach (var implementingSymbolRelation in GetImplementingTypes(roslynModelProvider, roslynSymbol))
                yield return implementingSymbolRelation;
        }

        private static IEnumerable<RoslynSymbolRelation> GetBaseInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementedInterfaceSymbol in interfaceSymbol.Interfaces)
                yield return new RoslynSymbolRelation(interfaceSymbol, implementedInterfaceSymbol, RelatedEntitySpecifications.BaseType);
        }
    }
}
