using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling2;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn interface symbol.
    /// </summary>
    internal class RoslynInterfaceNode : RoslynTypeNode
    {
        internal RoslynInterfaceNode(ModelItemId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, NodeStereotype.Interface)
        {
        }

        public override int LayoutPriority => 2;

        public override IEnumerable<RelatedSymbolPair> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.BaseType)
                foreach (var baseSymbolRelation in GetBaseInterfaces(NamedTypeSymbol))
                    yield return baseSymbolRelation;

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.Subtype)
                foreach (var derivedSymbolRelation in GetDerivedInterfaces(roslynModelProvider, NamedTypeSymbol))
                    yield return derivedSymbolRelation;

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.ImplementerType)
                foreach (var implementingSymbolRelation in GetImplementingTypes(roslynModelProvider, NamedTypeSymbol))
                    yield return implementingSymbolRelation;
        }

        private static IEnumerable<RelatedSymbolPair> GetBaseInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementedInterfaceSymbol in interfaceSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RelatedSymbolPair(interfaceSymbol, implementedInterfaceSymbol, DirectedRelationshipTypes.BaseType);
        }
    }
}
