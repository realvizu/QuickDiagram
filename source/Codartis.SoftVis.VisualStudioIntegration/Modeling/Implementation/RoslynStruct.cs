using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn struct symbol.
    /// </summary>
    internal sealed class RoslynStruct : RoslynTypeBase
    {
        internal RoslynStruct([NotNull] INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol)
        {
        }

        public override Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(
            IHostModelProvider hostModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            var result = Enumerable.Empty<RelatedSymbolPair>();

            if (directedModelRelationshipType == null || directedModelRelationshipType == DirectedModelRelationshipTypes.ImplementedInterface)
                result = result.Concat(GetImplementedInterfaces(NamedTypeSymbol));

            return Task.FromResult(result);
        }
    }
}