using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A specialization of the model entity concept that references a Roslyn symbol.
    /// Capable of finding related Roslyn symbols in the Roslyn API.
    /// </summary>
    public interface IRoslynBasedModelEntity : IModelEntity
    {
        INamedTypeSymbol RoslynSymbol { get; }

        IEnumerable<RelatedRoslynSymbols> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol roslynSymbol);
    }
}