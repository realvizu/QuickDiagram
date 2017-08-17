using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn struct symbol.
    /// </summary>
    internal class RoslynStructNode : RoslynTypeNode
    {
        internal RoslynStructNode(ModelItemId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, RoslynModelNodeStereotype.Struct)
        {
        }

        public override int LayoutPriority => 3;

        public override IEnumerable<RelatedSymbolPair> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.ImplementedInterface)
                foreach (var implementedSymbolRelation in GetImplementedInterfaces(NamedTypeSymbol))
                    yield return implementedSymbolRelation;
        }
    }
}
