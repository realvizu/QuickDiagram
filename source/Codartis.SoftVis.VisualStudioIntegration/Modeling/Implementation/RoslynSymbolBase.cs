using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model nodes that represent a Roslyn symbol.
    /// Immutable.
    /// </summary>
    internal abstract class RoslynSymbolBase : IRoslynSymbol
    {
        public ISymbol UnderlyingSymbol { get; }

        protected RoslynSymbolBase([NotNull] ISymbol roslynSymbol)
        {
            UnderlyingSymbol = roslynSymbol;
        }

        public bool SymbolEquals(ISymbol roslynSymbol) => UnderlyingSymbol.SymbolEquals(roslynSymbol);

        public virtual Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(
            IHostModelProvider hostModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            return Task.FromResult(Enumerable.Empty<RelatedSymbolPair>());
        }

        protected static async Task<IEnumerable<Compilation>> GetCompilationsAsync(Workspace workspace)
        {
            var enumerable = workspace?.CurrentSolution?.Projects?.Where(i => i != null);
            var compilations = await enumerable.SelectAsync(async i => await i.GetCompilationAsync());
            return compilations.WhereNotNull();
        }
    }
}