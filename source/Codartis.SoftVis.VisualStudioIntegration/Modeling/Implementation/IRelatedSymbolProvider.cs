using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    public interface IRelatedSymbolProvider
    {
        [NotNull]
        [ItemNotNull]
        Task<IEnumerable<RelatedSymbolPair>> GetRelatedSymbolsAsync(
            [NotNull] ISymbol symbol,
            DirectedModelRelationshipType? directedModelRelationshipType = null);
    }
}