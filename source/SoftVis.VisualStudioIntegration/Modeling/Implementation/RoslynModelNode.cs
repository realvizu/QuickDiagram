using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model nodes that represent a Roslyn symbol.
    /// Immutable.
    /// </summary>
    internal abstract class RoslynModelNode : ModelNode, IRoslynModelNode
    {
        public ISymbol RoslynSymbol { get; }

        protected RoslynModelNode(ModelNodeId id, ISymbol roslynSymbol, ModelNodeStereotype stereotype)
            : base(id, roslynSymbol.GetName(), stereotype, roslynSymbol.GetOrigin())
        {
            RoslynSymbol = roslynSymbol ?? throw new ArgumentNullException(nameof(roslynSymbol));
        }

        public bool SymbolEquals(ISymbol roslynSymbol) => RoslynSymbol.SymbolEquals(roslynSymbol);

        public virtual Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(
            IRoslynModelProvider roslynModelProvider,
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

        public IRoslynModelNode UpdateRoslynSymbol(ISymbol newSymbol)
        {
            return CreateInstance(Id, newSymbol);
        }

        protected abstract IRoslynModelNode CreateInstance(ModelNodeId id, ISymbol newSymbol);
    }
}