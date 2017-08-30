using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn struct symbol.
    /// </summary>
    internal class RoslynStructNode : RoslynTypeNode
    {
        internal RoslynStructNode(ModelNodeId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, ModelNodeStereotypes.Struct)
        {
        }

        protected override IRoslynModelNode CreateInstance(ModelNodeId id, ISymbol newSymbol)
            => new RoslynStructNode(id, EnsureNamedTypeSymbol(newSymbol));

        public override IEnumerable<RelatedSymbolPair> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.ImplementedInterface)
                foreach (var implementedSymbolRelation in GetImplementedInterfaces(NamedTypeSymbol))
                    yield return implementedSymbolRelation;
        }
    }
}
