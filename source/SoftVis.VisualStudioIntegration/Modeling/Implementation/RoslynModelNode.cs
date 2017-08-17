using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model nodes that represent a Roslyn symbol.
    /// Immutable.
    /// </summary>
    internal abstract class RoslynModelNode : ModelNodeBase, IRoslynModelNode
    {
        public ISymbol RoslynSymbol { get; }

        protected RoslynModelNode(ModelItemId id, ISymbol roslynSymbol, ModelNodeStereotype stereotype)
            : base(id, roslynSymbol.GetName(), stereotype, roslynSymbol.GetOrigin())
        {
            RoslynSymbol = roslynSymbol ?? throw new ArgumentNullException(nameof(roslynSymbol));
        }

        public bool SymbolEquals(ISymbol roslynSymbol) => RoslynSymbol.SymbolEquals(roslynSymbol);

        public virtual IEnumerable<RelatedSymbolPair> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null) => Enumerable.Empty<RelatedSymbolPair>();

        protected static IEnumerable<Compilation> GetCompilations(Workspace workspace)
        {
            return workspace?.CurrentSolution?.Projects.Select(i => i?.GetCompilationAsync().Result).Where(i => i != null) 
                ?? Enumerable.Empty<Compilation>();
        }
    }
}