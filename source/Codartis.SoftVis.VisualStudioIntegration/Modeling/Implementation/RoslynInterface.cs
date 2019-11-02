using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn interface symbol.
    /// </summary>
    internal sealed class RoslynInterface : RoslynTypeBase
    {
        internal RoslynInterface([NotNull] INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol)
        {
        }

        public override async Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(
            IHostModelProvider hostModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            var result = Enumerable.Empty<RelatedSymbolPair>();

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedModelRelationshipTypes.BaseType)
                result = result.Concat(GetBaseInterfaces(NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedModelRelationshipTypes.Subtype)
                result = result.Concat(await GetDerivedInterfacesAsync(hostModelProvider, NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedModelRelationshipTypes.ImplementerType)
                result = result.Concat(await GetImplementingTypesAsync(hostModelProvider, NamedTypeSymbol));

            return result;
        }

        [NotNull]
        private static IEnumerable<RelatedSymbolPair> GetBaseInterfaces([NotNull] INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementedInterfaceSymbol in interfaceSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RelatedSymbolPair(interfaceSymbol, implementedInterfaceSymbol, DirectedModelRelationshipTypes.BaseType);
        }
    }
}