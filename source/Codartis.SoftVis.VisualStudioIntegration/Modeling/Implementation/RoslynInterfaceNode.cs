using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn interface symbol.
    /// </summary>
    internal class RoslynInterfaceNode : RoslynTypeNode
    {
        internal RoslynInterfaceNode(ModelNodeId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, ModelNodeStereotypes.Interface)
        {
        }

        protected override IRoslynModelNode CreateInstance(ModelNodeId id, ISymbol newSymbol)
            => new RoslynInterfaceNode(id, EnsureNamedTypeSymbol(newSymbol));

        public override async Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            var result = Enumerable.Empty<RelatedSymbolPair>();

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.BaseType)
                result = result.Concat(GetBaseInterfaces(NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.Subtype)
                result = result.Concat(await GetDerivedInterfacesAsync(roslynModelProvider, NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.ImplementerType)
                result = result.Concat(await GetImplementingTypesAsync(roslynModelProvider, NamedTypeSymbol));

            return result;
        }

        private static IEnumerable<RelatedSymbolPair> GetBaseInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementedInterfaceSymbol in interfaceSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RelatedSymbolPair(interfaceSymbol, implementedInterfaceSymbol, DirectedRelationshipTypes.BaseType);
        }
    }
}
