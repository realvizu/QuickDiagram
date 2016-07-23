using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.WorkspaceContext;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Abstract base class for model entities created from Roslyn symbols.
    /// </summary>
    public abstract class RoslynBasedModelEntity : ModelEntity
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

        public virtual IEnumerable<RelatedRoslynSymbols> FindRelatedSymbols(IWorkspaceServices workspaceService, INamedTypeSymbol roslynSymbol)
            => Enumerable.Empty<RelatedRoslynSymbols>();

        protected static void EnsureSymbolTypeKind(INamedTypeSymbol symbol, params TypeKind[] expectedTypeKinds)
        {
            if (expectedTypeKinds.Any(i => symbol.TypeKind == i))
                return;

            throw new InvalidOperationException($"Unexpected symbol type: {symbol.TypeKind}");
        }
    }
}