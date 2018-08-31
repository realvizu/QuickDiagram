using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public override Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            var result = Enumerable.Empty<RelatedSymbolPair>();

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedRelationshipTypes.ImplementedInterface)
                result = result.Concat(GetImplementedInterfaces(NamedTypeSymbol));

            return Task.FromResult(result);
        }
    }
}