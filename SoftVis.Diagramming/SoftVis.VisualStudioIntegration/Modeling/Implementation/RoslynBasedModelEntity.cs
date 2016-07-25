using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling.Implementation;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model entities created from Roslyn symbols.
    /// Capable of finding related symbols in the Roslyn model (API).
    /// </summary>
    public abstract class RoslynBasedModelEntity : ModelEntity, IRoslynBasedModelEntity
    {
        public INamedTypeSymbol RoslynSymbol { get; }

        protected RoslynBasedModelEntity(INamedTypeSymbol roslynSymbol, TypeKind typeKind)
            :base(roslynSymbol.GetMinimallyQualifiedName(), typeKind.ToModelEntityType(), typeKind.ToModelEntityStereotype())
        {
            if (roslynSymbol.TypeKind != typeKind)
                throw new ArgumentException($"{roslynSymbol.Name} must be a {typeKind}.");

            RoslynSymbol = roslynSymbol;
        }

        public string Id => RoslynSymbol.GetKey();

        public virtual IEnumerable<RelatedRoslynSymbols> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol roslynSymbol)
            => Enumerable.Empty<RelatedRoslynSymbols>();

        protected static void EnsureSymbolTypeKind(INamedTypeSymbol symbol, params TypeKind[] expectedTypeKinds)
        {
            if (expectedTypeKinds.Any(i => symbol.TypeKind == i))
                return;

            throw new InvalidOperationException($"Unexpected symbol type: {symbol.TypeKind}");
        }
    }
}