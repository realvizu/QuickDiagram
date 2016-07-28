using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model entity created from a Roslyn struct symbol.
    /// </summary>
    internal class RoslynBasedStruct : RoslynBasedModelEntity
    {
        internal RoslynBasedStruct(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Struct)
        {
        }

        public override int Priority => 3;

        public override IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol roslynSymbol)
        {
            EnsureSymbolTypeKind(roslynSymbol, TypeKind.Struct);

            foreach (var implementedSymbolRelation in GetImplementedInterfaces(roslynSymbol))
                yield return implementedSymbolRelation;
        }
    }
}
