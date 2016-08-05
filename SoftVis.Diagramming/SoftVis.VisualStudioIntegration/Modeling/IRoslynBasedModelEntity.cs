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
        /// <summary>
        /// Gets the Roslyn symbol associated with this entity.
        /// </summary>
        INamedTypeSymbol RoslynSymbol { get; }

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="entityRelationType">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider, 
            EntityRelationType? entityRelationType = null);
    }
}