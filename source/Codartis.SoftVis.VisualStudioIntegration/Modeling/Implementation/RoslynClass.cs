using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn class symbol.
    /// </summary>
    internal sealed class RoslynClass : RoslynTypeBase
    {
        internal RoslynClass([NotNull] INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol)
        {
        }

        public override bool IsAbstract => NamedTypeSymbol.IsAbstract;

        public override async Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(
            IHostModelProvider hostModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            var result = Enumerable.Empty<RelatedSymbolPair>();

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedModelRelationshipTypes.BaseType)
                result = result.Concat(GetBaseTypes(NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedModelRelationshipTypes.Subtype)
                result = result.Concat(await GetDerivedTypesAsync(hostModelProvider, NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedModelRelationshipTypes.ImplementedInterface)
                result = result.Concat(GetImplementedInterfaces(NamedTypeSymbol));

            return result;
        }

        [NotNull]
        private static IEnumerable<RelatedSymbolPair> GetBaseTypes([NotNull] INamedTypeSymbol roslynSymbol)
        {
            var baseSymbol = roslynSymbol.BaseType;
            if (baseSymbol?.TypeKind == TypeKind.Class)
                yield return new RelatedSymbolPair(roslynSymbol, baseSymbol, DirectedModelRelationshipTypes.BaseType);
        }
    }
}