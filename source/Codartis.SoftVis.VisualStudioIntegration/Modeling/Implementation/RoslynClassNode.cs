using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn class symbol.
    /// </summary>
    internal class RoslynClassNode : RoslynTypeNode
    {
        internal RoslynClassNode(ModelNodeId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, ModelNodeStereotypes.Class)
        {
        }

        public override bool IsAbstract => NamedTypeSymbol.IsAbstract;

        protected override IRoslynModelNode CreateInstance(ModelNodeId id, ISymbol newSymbol)
            => new RoslynClassNode(id, EnsureNamedTypeSymbol(newSymbol));

        public override async Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            var result = Enumerable.Empty<RelatedSymbolPair>();

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.BaseType)
                result = result.Concat(GetBaseTypes(NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.Subtype)
                result = result.Concat(await GetDerivedTypesAsync(roslynModelProvider, NamedTypeSymbol));

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.ImplementedInterface)
                result = result.Concat(GetImplementedInterfaces(NamedTypeSymbol));

            return result;
        }

        private static IEnumerable<RelatedSymbolPair> GetBaseTypes(INamedTypeSymbol roslynSymbol)
        {
            var baseSymbol = roslynSymbol.BaseType;
            if (baseSymbol?.TypeKind == TypeKind.Class)
                yield return new RelatedSymbolPair(roslynSymbol, baseSymbol, DirectedRelationshipTypes.BaseType);
        }
    }
}
